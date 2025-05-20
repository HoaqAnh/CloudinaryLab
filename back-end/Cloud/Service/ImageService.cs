using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Cloudinary.Settings;

namespace Cloudinary.Service
{
    public class ImageService : IImageService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;
        private readonly ILogger<ImageService> _logger;

        public ImageService(IOptions<CloudinarySetting> options, ILogger<ImageService> logger)
        {
            if (options?.Value == null)
                throw new ArgumentNullException(nameof(options), "Cloudinary settings are not configured");

            var account = new Account(
                options.Value.CloudName,
                options.Value.APIKey,
                options.Value.APISecret
            );

            _cloudinary = new CloudinaryDotNet.Cloudinary(account);
            _cloudinary.Api.Secure = true;
            _logger = logger;
            _logger.LogInformation("ImageService initialized with CloudName: {CloudName}", options.Value.CloudName);
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            if (result.Result == "ok")
            {
                return result;
            }
            else
            {
                _logger.LogError($"Failed to delete image with publicId: {publicId}. Error: {result.Error?.Message}");
                return new DeletionResult
                {
                    Result = "error",
                    Error = result.Error
                };
            }
        }

        public async Task<GetResourceResult?> GetImageAsync(string publicId)
        {
            var getParams = new GetResourceParams(publicId);
            var result = await _cloudinary.GetResourceAsync(getParams);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return result;
            }
            else
            {
                _logger.LogError($"Failed to get image with publicId: {publicId}. Error: {result.Error?.Message}");
                return null;
            }
        }

        public async Task<ImageUploadResult?> UpdateImageAsync(string publicId, IFormFile newFile)
        {
            var updateParams = new ImageUploadParams
            {
                File = new FileDescription(newFile.FileName, newFile.OpenReadStream()),
                PublicId = publicId,
                Overwrite = true
            };
            var result = await _cloudinary.UploadAsync(updateParams);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return result;
            }
            else
            {
                _logger.LogError($"Failed to update image with publicId: {publicId}. Error: {result.Error?.Message}");
                return null;
            }
        }

        public async Task<ImageUploadResult?> UploadImageAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = $"uploads/{Guid.NewGuid()}"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult;
            }
            else
            {
                _logger.LogError($"Failed to upload image. Error: {uploadResult.Error?.Message}");
                return null;
            }
        }
        public async Task<ListResourcesResult?> GetImagesFromFolderAsync(string folderName)
        {
            var listParams = new ListResourcesByPrefixParams()
            {
                Prefix = folderName, // Lấy các tài nguyên có tiền tố là tên thư mục
                Type = "upload",     // Chỉ lấy các file đã upload
                MaxResults = 500     // Giới hạn số lượng kết quả (tối đa 500 mỗi request)
            };
            var result = await _cloudinary.ListResourcesAsync(listParams);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("Successfully retrieved {Count} resources from folder {FolderName}.", result.Resources?.Count() ?? 0, folderName);
                return result;
            }
            else
            {
                _logger.LogError($"Failed to get images from folder: {folderName}. Error: {result.Error?.Message}");
                return null;
            }
        }
        public async Task<ImageUploadResult?> UploadImageToFolderAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogError("No file provided or file is empty.");
                return null;
            }

            // Set default folder if none provided
            folderName = string.IsNullOrWhiteSpace(folderName) ? "default_folder" : folderName.Trim();

            using var stream = file.OpenReadStream();
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            var publicId = $"{fileNameWithoutExtension}_{Guid.NewGuid()}";

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = publicId,
                Folder = folderName, // Cloudinary will create the folder if it doesn't exist
                Overwrite = false // Set to true if you want to overwrite existing files with the same PublicId
            };

            try
            {
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("Image uploaded successfully. PublicId: '{PublicId}', Folder: '{Folder}'",
                        uploadResult.PublicId, folderName);
                    return uploadResult;
                }
                else
                {
                    _logger.LogError("Failed to upload image to folder '{Folder}'. Error: {ErrorMessage}",
                        folderName, uploadResult.Error?.Message);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading image to folder '{Folder}'.", folderName);
                return null;
            }
        }
    }
}