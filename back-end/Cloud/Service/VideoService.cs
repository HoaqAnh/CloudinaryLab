using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Cloudinary.Settings; // Ensure this namespace is correct for CloudinarySetting
using System;
using System.IO; // For Path.GetFileNameWithoutExtension
using System.Threading.Tasks;
using System.Linq; // For Count()

namespace Cloudinary.Service
{
    public class VideoService : IVideoService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;
        private readonly ILogger<VideoService> _logger;

        public VideoService(IOptions<CloudinarySetting> options, ILogger<VideoService> logger)
        {
            if (options?.Value == null)
            {
                // Logger might be null here if ArgumentNullException is thrown before logger is assigned.
                throw new ArgumentNullException(nameof(options), "Cloudinary settings are not configured.");
            }

            _cloudinary = new CloudinaryDotNet.Cloudinary(new Account(
                options.Value.CloudName,
                options.Value.APIKey,
                options.Value.APISecret
            ));
            _cloudinary.Api.Secure = true; // Recommended to use HTTPS
            _logger = logger;
            _logger.LogInformation("VideoService initialized with CloudName: {CloudName}", options.Value.CloudName);
        }

        public async Task<VideoUploadResult?> UploadVideoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogError("UploadVideoAsync: No file provided or file is empty.");
                return null;
            }

            // Using a default folder for general video uploads
            return await UploadVideoToFolderAsync(file, "default_videos");
        }

        public async Task<VideoUploadResult?> UploadVideoToFolderAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogError("UploadVideoToFolderAsync: No file provided or file is empty.");
                return null;
            }

            folderName = string.IsNullOrWhiteSpace(folderName) ? "default_videos_folder" : folderName.Trim();

            using var stream = file.OpenReadStream();
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            // Create a unique PublicId for the video within that folder
            var publicId = $"{fileNameWithoutExtension}_{Guid.NewGuid()}";

            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = publicId,
                Folder = folderName, // Cloudinary will create the folder if it doesn't exist
                Overwrite = false    // Set to true if you want to overwrite existing files with the same PublicId
            };

            try
            {
                var uploadResult = await _cloudinary.UploadAsync(uploadParams); // Returns VideoUploadResult

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("UploadVideoToFolderAsync: Video uploaded successfully. PublicId: '{PublicId}', Folder: '{Folder}'",
                                           uploadResult.PublicId, folderName);
                    return uploadResult; // Directly return VideoUploadResult
                }
                else
                {
                    _logger.LogError("UploadVideoToFolderAsync: Failed to upload video. PublicId attempted: {PublicId}. Error: {ErrorMessage}, HTTP Status: {StatusCode}",
                                     publicId, uploadResult.Error?.Message, uploadResult.StatusCode);
                    return uploadResult; // Return the result which contains error information
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadVideoToFolderAsync: An error occurred. PublicId attempted: {PublicId}, Folder: {Folder}", publicId, folderName);
                return new VideoUploadResult { Error = new Error { Message = ex.Message }, StatusCode = System.Net.HttpStatusCode.InternalServerError };
            }
        }

        public async Task<DeletionResult> DeleteVideoAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                _logger.LogError("DeleteVideoAsync: PublicId cannot be null or empty.");
                return new DeletionResult { Result = "error", Error = new Error { Message = "PublicId cannot be empty." } };
            }

            var deleteParams = new DeletionParams(publicId.Trim())
            {
                ResourceType = ResourceType.Video // Important: Specify resource type as video
            };

            try
            {
                var result = await _cloudinary.DestroyAsync(deleteParams);
                if (result.Result == "ok")
                {
                    _logger.LogInformation("DeleteVideoAsync: Video deleted successfully. PublicId: {PublicId}", publicId);
                }
                else
                {
                    _logger.LogWarning("DeleteVideoAsync: Failed to delete video. PublicId: {PublicId}. Result: {Result}, Error: {Error}",
                                     publicId, result.Result, result.Error?.Message);
                }
                return result; // Return the result whether success or failure (result contains error info)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteVideoAsync: Exception occurred for PublicId: {PublicId}", publicId);
                return new DeletionResult { Result = "error", Error = new Error { Message = ex.Message } };
            }
        }

        public async Task<VideoUploadResult?> UpdateVideoAsync(string publicId, IFormFile newFile)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                _logger.LogError("UpdateVideoAsync: PublicId cannot be null or empty.");
                return null;
            }
            if (newFile == null || newFile.Length == 0)
            {
                _logger.LogError("UpdateVideoAsync: No file provided or file is empty for PublicId: {PublicId}", publicId);
                return null;
            }

            using var stream = newFile.OpenReadStream();
            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(newFile.FileName, stream),
                PublicId = publicId.Trim(),
                Overwrite = true, // Important when updating
            };

            try
            {
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("UpdateVideoAsync: Video updated successfully. PublicId: '{PublicId}'", publicId);
                    return uploadResult;
                }
                _logger.LogError("UpdateVideoAsync: Failed for PublicId: '{PublicId}'. Error: {ErrorMessage}, HTTP Status: {StatusCode}",
                                 publicId, uploadResult.Error?.Message, uploadResult.StatusCode);
                return uploadResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateVideoAsync: Exception occurred for PublicId: {PublicId}", publicId);
                return new VideoUploadResult { Error = new Error { Message = ex.Message }, StatusCode = System.Net.HttpStatusCode.InternalServerError };
            }
        }

        public async Task<GetResourceResult?> GetVideoAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                _logger.LogWarning("GetVideoAsync: PublicId cannot be null or empty.");
                return null;
            }
            var getParams = new GetResourceParams(publicId.Trim())
            {
                ResourceType = ResourceType.Video // Specify resource type
            };

            try
            {
                var result = await _cloudinary.GetResourceAsync(getParams);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("GetVideoAsync: Retrieved video successfully. PublicId: {PublicId}", publicId);
                    return result;
                }
                _logger.LogWarning("GetVideoAsync: Failed to get video. PublicId: {PublicId}. Status: {StatusCode}, Error: {Error}",
                                 publicId, result.StatusCode, result.Error?.Message);
                return result; // Return result which might contain error details
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetVideoAsync: Exception occurred for PublicId: {PublicId}", publicId);
                return new GetResourceResult { Error = new Error { Message = ex.Message }, StatusCode = System.Net.HttpStatusCode.InternalServerError };
            }
        }

        public async Task<ListResourcesResult?> GetVideoFromFolderAsync(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                _logger.LogWarning("GetVideoFromFolderAsync: FolderName cannot be null or empty.");
                return null;
            }

            // Ensure consistent folder path for querying, especially if subfolders are involved.
            // Cloudinary treats "folder/" and "folder" differently in some prefix searches.
            // Using Folder parameter in upload is generally safer for organization.
            string prefixToSearch = folderName.Trim();
            // if (!prefixToSearch.EndsWith("/"))
            // {
            //     prefixToSearch += "/"; // Be cautious with this; test against your PublicID naming convention.
            // }


            var listParams = new ListResourcesByPrefixParams()
            {
                Prefix = prefixToSearch,
                ResourceType = ResourceType.Video, // Specify resource type
                Type = "upload", // Usually "upload" for user-uploaded assets
                MaxResults = 500 // Adjust or implement pagination if needed
            };
            // Alternatively, if you consistently use the Folder parameter during upload:
            // var listParams = new ListResourcesByFolderParams()
            // {
            //     Folder = folderName.Trim(),
            //     ResourceType = ResourceType.Video,
            //     Type = "upload",
            //     MaxResults = 500
            // };


            try
            {
                var result = await _cloudinary.ListResourcesAsync(listParams); // Use the chosen params
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("GetVideoFromFolderAsync: Retrieved {Count} videos from folder/prefix {FolderName}.",
                                         result.Resources?.Count() ?? 0, folderName);
                    return result;
                }
                _logger.LogWarning("GetVideoFromFolderAsync: Failed for folder/prefix: {FolderName}. Status: {StatusCode}, Error: {Error}",
                                 folderName, result.StatusCode, result.Error?.Message);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetVideoFromFolderAsync: Exception occurred for folder/prefix: {FolderName}", folderName);
                return new ListResourcesResult { Error = new Error { Message = ex.Message }, StatusCode = System.Net.HttpStatusCode.InternalServerError };
            }
        }
    }
}