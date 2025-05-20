// File: src/hooks/useImageUpload.tsx
import { useState } from "react";
import { uploadImageToFolder } from "../service/imageService"; // Ensure this path is correct

export const useUploadImage = () => {
    const [uploadProgress, setUploadProgress] = useState(0);
    const [isUploading, setIsUploading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const uploadImage = async (file: File, folderName?: string) => {
        setIsUploading(true);
        setError(null);
        setUploadProgress(0);
        
        try {
            const res = await uploadImageToFolder(
                file,
                folderName,
                (progress) => setUploadProgress(progress),
            );
            return res;
        } catch (err) {
            setError("Upload failed. Please try again.");
            // Optionally, you could inspect 'err' for more specific messages
            // if (axios.isAxiosError(err) && err.response) {
            //     setError(`Upload failed: ${err.response.data.message || err.message}`);
            // } else {
            //     setError("Upload failed due to an unexpected error.");
            // }
            return undefined;
        } finally {
            setIsUploading(false);
        }
    };

    return {
        uploadImage,
        uploadProgress,
        isUploading,
        error,
    };
};