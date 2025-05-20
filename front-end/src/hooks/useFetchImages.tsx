import { useState, useCallback } from "react";
import { getImagesFromFolder } from "../service/imageService";

export const useFetchImages = () => {
    const [images, setImages] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const fetchImages = useCallback(async (folderName: string) => {
        setIsLoading(true);
        setError(null);
        try {
            const fetchedImages = await getImagesFromFolder(folderName);
            setImages(fetchedImages);
        } catch (err) {
            setError(`Bạn chưa tải ảnh nào trên Cloudinary`);
            setImages([]);
        } finally {
            setIsLoading(false);
        }
    }, []);

    return {
        images,
        isLoading,
        error,
        fetchImages,
    };
};