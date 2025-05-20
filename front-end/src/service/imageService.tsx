import axios from "axios";

const BASE_URL = import.meta.env.VITE_API_BASE_URL;

export const uploadImageToFolder = async (
    imageFile: File,
    folderName: string = "cloudInfo",
    onUploadProgress?: (progress: number) => void
): Promise<any> => {
    const formData = new FormData();
    formData.append("file", imageFile);
    formData.append("folder", folderName);

    try {
        console.log(`Đang gửi yêu cầu POST đến: ${BASE_URL}/CloudinaryApi/upload_to_folder`);
        console.log(`File: ${imageFile.name}, Type: ${imageFile.type}, Size: ${imageFile.size} bytes`);
        console.log(`Folder: ${folderName}`);

        const response = await axios.post(
            `${BASE_URL}/CloudinaryApi/upload_to_folder`,
            formData,
            {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
                onUploadProgress: (progressEvent) => {
                    if (onUploadProgress && progressEvent.total) {
                        const percent = Math.round((progressEvent.loaded * 100) / progressEvent.total);
                        onUploadProgress(percent);
                    } else if (onUploadProgress) {
                        onUploadProgress(-1);
                    }
                }
            }
        );
        console.log("Phản hồi từ server:", response);
        return response;
    } catch (error) {
        console.error("Lỗi khi upload ảnh:", error);
        throw error;
    }
};

export const getImagesFromFolder = async (
    folderName: string = "cloudInfo",
): Promise<any> => {
    try {
        const response = await axios.get(`${BASE_URL}/CloudinaryApi/folder/${folderName}`);
        return response.data;
    } catch (error) {
        throw error;
    }
};