import { useState, type JSX, type ChangeEvent, useEffect } from 'react';
import Item from './component/item';
import { useUploadImage } from './hooks/useImageUpload';
import { useFetchImages } from './hooks/useFetchImages';
import './App.css';

const App = (): JSX.Element => {
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [fileName, setFileName] = useState<string>("Không có file được chọn.");
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [uploadSuccessTrigger, setUploadSuccessTrigger] = useState(0);

  const FOLDER_NAME = "cloudInfo";

  const {
    isUploading,
    uploadProgress,
    error: uploadError,
    uploadImage
  } = useUploadImage();

  type CloudImage = {
    publicId: string;
    secureUrl: string;
  };

  const {
    images: fetchedImages,
    isLoading: isLoadingImages,
    error: fetchError,
    fetchImages
  } = useFetchImages() as {
    images: CloudImage[];
    isLoading: boolean;
    error: string | null;
    fetchImages: (folderName: string) => void;
  };

  useEffect(() => {
    fetchImages(FOLDER_NAME);
  }, [fetchImages, FOLDER_NAME, uploadSuccessTrigger]);

  const handleImageChange = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      if (imagePreview) {
        URL.revokeObjectURL(imagePreview);
      }
      const previewUrl = URL.createObjectURL(file);
      setImagePreview(previewUrl);
      setFileName(file.name);
      setSelectedFile(file);
    } else {
      if (imagePreview) {
        URL.revokeObjectURL(imagePreview);
      }
      setImagePreview(null);
      setFileName("Không có file được chọn.");
      setSelectedFile(null);
    }
  };

  const handleUpload = async () => {
    if (!selectedFile) {
      alert("Vui lòng chọn một ảnh trước khi tải lên.");
      return;
    }
    try {
      const res = await uploadImage(selectedFile, FOLDER_NAME);

      if (res && res.data && res.data.secureUrl) {
        setUploadSuccessTrigger(prevTrigger => prevTrigger + 1);
        if (imagePreview) {
          URL.revokeObjectURL(imagePreview);
        }
        setImagePreview(null);
        setFileName("Không có file được chọn.");
        setSelectedFile(null);
        const fileInput = document.getElementById('imageUpload') as HTMLInputElement;
        if (fileInput) {
          fileInput.value = "";
        }
      } else if (res) {
        console.warn("Upload may have succeeded, but the response data or secure_url is missing:", res);
      }
    } catch (err) {
      console.error("Upload failed in App.tsx handler:", err);
    }
  };

  const handleRemoveImage = () => {
    if (imagePreview) {
      URL.revokeObjectURL(imagePreview);
    }
    setImagePreview(null);
    setFileName("Không có file được chọn.");
    setSelectedFile(null);
    const fileInput = document.getElementById('imageUpload') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = "";
    }
  };

  return (
    <div className="min-h-screen h-screen flex flex-col font-sans bg-slate-900 text-slate-200 overflow-hidden">
      <header className="bg-slate-800/70 backdrop-blur-md text-slate-50 py-4 px-6 shadow-lg sticky top-0 z-50 flex-shrink-0">
        {/* Responsive text size for header */}
        <h1 className="text-xl sm:text-2xl font-semibold">Cloudinary Lab</h1>
      </header>

      <main className="flex-grow flex flex-col md:flex-row overflow-hidden">
        {/* Left Panel: Upload */}
        <div className="w-full md:w-1/2 p-4 sm:p-6 md:p-8 flex flex-col items-center justify-center bg-slate-800 shadow-lg md:rounded-r-xl overflow-y-auto custom-scrollbar-dark">
          {/* Responsive text size and margin for upload section title */}
          <h1 className="text-2xl sm:text-3xl font-bold text-indigo-400 mb-4 sm:mb-6 text-center">Xem trước & Tải ảnh lên</h1>
          <div className="w-full max-w-md h-64 sm:h-80 md:h-96 bg-slate-700 rounded-lg mb-6 flex items-center justify-center overflow-hidden border-2 border-dashed border-slate-600 flex-shrink-0">
            {imagePreview ? (
              <img
                src={imagePreview}
                alt="Xem trước ảnh"
                className="object-contain h-full w-full"
                loading="lazy"
              />
            ) : (
              <p className="text-slate-400 text-center p-4">
                Hình ảnh xem trước sẽ hiển thị ở đây
              </p>
            )}
          </div>
          <div className="w-full max-w-md space-y-4">
            <p className="text-sm text-slate-400 text-center truncate" title={fileName}>
              {fileName}
            </p>
            <div>
              <label
                htmlFor="imageUpload"
                className="w-full flex items-center justify-center px-6 py-3 border border-transparent rounded-md shadow-sm text-base font-medium text-white bg-indigo-500 hover:bg-indigo-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-slate-800 focus:ring-indigo-500 cursor-pointer transition-colors duration-150"
              >
                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0L8 8m4-4v12" />
                </svg>
                {imagePreview ? "Chọn ảnh khác" : "Chọn ảnh"}
              </label>
              <input
                type="file"
                id="imageUpload"
                className="hidden"
                accept="image/*,.heic,.heif,.webp"
                onChange={handleImageChange}
              />
            </div>

            {imagePreview && (
              <div className="flex flex-col sm:flex-row space-y-3 sm:space-y-0 sm:space-x-3">
                <button
                  onClick={handleUpload}
                  disabled={isUploading || !selectedFile}
                  className={`w-full sm:flex-1 flex items-center justify-center px-6 py-3 border border-transparent rounded-md shadow-sm text-base font-medium text-white transition-colors duration-150 ${isUploading || !selectedFile ? 'bg-green-700/60 cursor-not-allowed' : 'bg-green-500 hover:bg-green-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-slate-800 focus:ring-green-500'}`}
                >
                  {isUploading ? (
                    <>
                      <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                      </svg>
                      Đang tải lên... {uploadProgress > 0 ? `${uploadProgress}%` : ''}
                    </>
                  ) : (
                    <>
                      <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M7 16a4 4 0 01-4-4V7a4 4 0 014-4h10a4 4 0 014 4v5a4 4 0 01-4 4H7zM7 16V7m0 9H5m2 0h10m2 0h-2M7 7H5m2 0h10m2 0h-2m0-5H7m0 0h.01" /></svg>
                      Tải lên
                    </>
                  )}
                </button>
                <button
                  onClick={handleRemoveImage}
                  disabled={isUploading}
                  className={`w-full sm:flex-1 flex items-center justify-center px-6 py-3 border border-transparent rounded-md shadow-sm text-base font-medium text-white transition-colors duration-150 ${isUploading ? 'bg-red-700/50 cursor-not-allowed' : 'bg-red-500 hover:bg-red-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-slate-800 focus:ring-red-500'}`}
                >
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
                  Gỡ ảnh
                </button>
              </div>
            )}
            {isUploading && uploadProgress > 0 && (
              <div className="w-full bg-slate-600 rounded h-2.5 mt-2">
                <div className="bg-green-400 h-2.5 rounded" style={{ width: `${uploadProgress}%` }} />
              </div>
            )}

            {(uploadError || fetchError) && (
              <div className="text-red-400 text-sm mt-2 p-3 bg-red-900/30 rounded border border-red-700/50">
                {uploadError && <p>Lỗi upload: {uploadError}</p>}
                {fetchError && <p>Lỗi tải danh sách ảnh: {fetchError}</p>}
              </div>
            )}
          </div>
        </div>

        {/* Right Panel: Image List */}
        <div className="w-full md:w-1/2 p-4 sm:p-6 md:p-8 flex flex-col items-center bg-slate-800 md:bg-slate-900 overflow-y-auto custom-scrollbar-dark">
          {/* Responsive text size and margin for image list title */}
          <h2 className="text-xl sm:text-2xl font-semibold text-slate-300 mb-4 sm:mb-6 text-center flex-shrink-0">DANH SÁCH ẢNH ĐÃ TẢI LÊN</h2>
          {isLoadingImages && <p className="text-slate-400">Đang tải danh sách ảnh...</p>}
          {/* Responsive grid for images */}
          <div className="w-full grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 md:gap-6">
            {fetchedImages.length > 0 ? (
              fetchedImages.map((image) => (
                <Item key={image.publicId} imageUrl={image.secureUrl} />
              ))
            ) : (
              !isLoadingImages && !fetchError && <p className="col-span-full text-center text-slate-400">Chưa có ảnh nào được tải lên.</p>
            )}
          </div>
        </div>
      </main>
    </div>
  );
};

export default App;