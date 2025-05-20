import { type JSX } from "react";

interface ItemProps {
    imageUrl: string;
    altText?: string;
}

const Item = ({ imageUrl, altText = "Hình ảnh từ Cloudinary" }: ItemProps): JSX.Element => {

    return (
        <div className="item bg-slate-700 p-2 rounded-lg shadow-md flex flex-col items-center space-y-3 transition-transform duration-300 ease-in-out hover:scale-110 hover:z-10 relative">
            <img
                src={imageUrl}
                alt={altText}
                className="rounded-md object-cover w-full h-60"
                onError={(e) => {
                    const target = e.target as HTMLImageElement;
                    target.onerror = null;
                    target.src = "https://placehold.co/300x200/334155/94A3B8?text=Lỗi+Tải+Ảnh";
                    target.alt = "Lỗi tải hình ảnh";
                }}
            />
        </div>
    );
};

export default Item;