using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;

namespace Cloudinary.Service
{
    public interface IVideoService
    {
        public Task<VideoUploadResult?> UploadVideoAsync(IFormFile file);
        public Task<DeletionResult> DeleteVideoAsync(string publicId);
        public Task<VideoUploadResult?> UpdateVideoAsync(string publicId, IFormFile NewFile);
        public Task<GetResourceResult?> GetVideoAsync(string publicId);
        public Task<ListResourcesResult?> GetVideoFromFolderAsync(string folderName);
        public Task<VideoUploadResult?> UploadVideoToFolderAsync(IFormFile file, string folderName);
    }
}
