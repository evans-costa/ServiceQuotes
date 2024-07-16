namespace ServiceQuotes.Services;

public interface IS3BucketService
{
    Task<string> UploadFileToS3(byte[] file, string fileName);
}
