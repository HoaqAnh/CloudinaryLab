using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cloudinary.Service
{
    public interface IImageService
    {
        public Task<ImageUploadResult?> UploadImageAsync(IFormFile file);
        public Task<DeletionResult> DeleteImageAsync(string publicId);
        public Task<ImageUploadResult?> UpdateImageAsync(string publicId, IFormFile NewFile);
        public Task<GetResourceResult?> GetImageAsync(string publicId);
        public Task<ListResourcesResult?> GetImagesFromFolderAsync(string folderName);
        public Task<ImageUploadResult?> UploadImageToFolderAsync(IFormFile file, string folderName);
    }
}
