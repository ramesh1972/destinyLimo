namespace DestinyLimoServer.Common.Uploader
{
    public static class UploadFilePaths
    {
        public const string ProfilePicture = "profile-pictures";
        public const string Content = "content";
        public const string TrainingMaterial = "training-material";
        public const string Other = "others";
    }

    public class FileUploader
    {
        // get the root uploads folder name from configuratio
        private string _rootUploadsFolderName = "";

        private readonly ILogger _logger;

        private readonly IConfiguration _configuration = null!;

        public FileUploader(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;

            _configuration = configuration;

            _rootUploadsFolderName = _configuration.GetSection("Uploads").GetValue<string>("Destination")!;
        }

        public string GetFilePathName(string folderName, string fileName)
        {
            // get relative path to web root
            string folderPath = folderName;
            string filePathName = folderPath + "/" + fileName;

            return filePathName;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogError("File is empty.");
                    return null!;
                }

                string folderPath = Path.Combine(_rootUploadsFolderName, folderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, file.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the file.");
                return null!;
            }
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the file.");
                return false;
            }
        }
    }
}