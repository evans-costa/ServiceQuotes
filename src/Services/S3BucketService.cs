using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SSO;


namespace ServiceQuotes.Services;

public class S3BucketService : IS3BucketService
{
    private readonly IConfiguration _configuration;
    public S3BucketService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> UploadFileToS3(byte[] file, string fileName)
    {
        var (ssoCredentials, regionEndpoint) = LoadSsoCredentials(_configuration["AWS:ProfileName"]);
        var s3Client = new AmazonS3Client(ssoCredentials, regionEndpoint);

        try
        {
            var memoryStream = new MemoryStream(file);

            var request = new PutObjectRequest
            {
                BucketName = _configuration["AWS:BucketName"],
                Key = fileName,
                InputStream = memoryStream,
                ContentType = "application/pdf",
            };

            var response = await s3Client.PutObjectAsync(request);

            string fileUrl = $"https://{_configuration["AWS:BucketName"]}.s3.{regionEndpoint.SystemName}.amazonaws.com/{fileName}";

            return fileUrl;
        }
        catch (AmazonS3Exception)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static (AWSCredentials, Amazon.RegionEndpoint) LoadSsoCredentials(string? profile)
    {
        var chain = new CredentialProfileStoreChain();
        if (!chain.TryGetProfile(profile, out var profileInfo) || !chain.TryGetAWSCredentials(profile, out var credentials))
        {
            throw new Exception($"Falha ao procurar o perfil {profile}");
        }

        var region = profileInfo.Region;

        return (credentials, region);
    }
}
