using QuanLySanBong.Service.File;
using IOFile = System.IO.File; // Để tránh trùng tên với IFormFile

public class FileService : IFileService
{
    private readonly string _imageFolderPath;
    private readonly string _baseUrl;

    public FileService(IConfiguration configuration)
    {
        _imageFolderPath = Path.GetFullPath(configuration["FileStorage:ImageFolderPath"] ?? "wwwroot/images");
        _baseUrl = configuration["FileStorage:BaseUrl"] ?? "https://localhost:8007/images";

        if (!Directory.Exists(_imageFolderPath))
        {
            Directory.CreateDirectory(_imageFolderPath);
        }
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Invalid file");

        string originalFileName = Path.GetFileName(file.FileName);
        string filePath = Path.Combine(_imageFolderPath, originalFileName);

        // 🔹 Nếu file đã tồn tại, thêm số vào cuối tên file để tránh ghi đè
        int count = 1;
        string newFilePath = filePath;
        while (IOFile.Exists(newFilePath))
        {
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
            string extension = Path.GetExtension(originalFileName);
            newFilePath = Path.Combine(_imageFolderPath, $"{fileNameWithoutExt}_{count}{extension}");
            count++;
        }

        try
        {
            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
        catch (Exception ex)
        {
            throw new IOException("Error saving file", ex);
        }

        // 🔹 Trả về URL file đã lưu
        string savedFileName = Path.GetFileName(newFilePath);
        return $"{_baseUrl}/{savedFileName}";
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;

        var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
        var fullPath = Path.Combine(_imageFolderPath, fileName);

        if (IOFile.Exists(fullPath))
        {
            await Task.Run(() => IOFile.Delete(fullPath));
        }
    }

    public async Task DeleteFilesAsync(List<string> fileUrls)
    {
        if (fileUrls == null || fileUrls.Count == 0) return;

        foreach (var fileUrl in fileUrls)
        {
            await DeleteFileAsync(fileUrl);
        }
    }

    public string GetFileUrl(string fileName)
    {
        return $"{_baseUrl}/{fileName}";
    }
}
