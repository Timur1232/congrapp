using Congrapp.Server.Utils;

namespace Congrapp.Server.Services;

public interface IImageService
{
    public Result<string, string> Save(IFormFile file);
    public Result<FileStream, string> Load(string filePath);
    public Result<string, string> Delete(string filePath);
}

public class ImageService(IConfiguration config) : IImageService
{
    public Result<string, string> Save(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        if (extension != ".jpg" && extension != ".jpeg")
        {
            return Result<string, string>.Err("Only jpg or jpeg files are supported.");
        }
        var fileName = Guid.NewGuid() + extension;
        var filePath = Path.Combine(config["ImagesUploadDir"]!, fileName);

        var stream = new FileStream(filePath, FileMode.Create);
        file.CopyToAsync(stream);
        return Result<string, string>.Ok(filePath);
    }

    public Result<FileStream, string> Load(string filePath)
    {
        var path = Path.Combine(filePath);
        if (!File.Exists(path))
        {
            return Result<FileStream, string>.Err("File does not exist.");
        }

        var imageStream = File.OpenRead(path);
        return Result<FileStream, string>.Ok(imageStream);
    }

    public Result<string, string> Delete(string filePath)
    {
        var path = Path.Combine(filePath);
        
        if (!File.Exists(path))
        {
            return Result<string, string>.Err("File does not exist.");
        }
        
        File.Delete(path);
        return Result<string, string>.Ok(path);
    }
}