using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceQuotes.Application.Interfaces;

namespace ServiceQuotes.Infrastructure.Services;

public class S3BucketService : IS3BucketService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<S3BucketService> _logger;
    public S3BucketService(IConfiguration configuration, ILogger<S3BucketService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> UploadFileToS3(byte[] file, string fileName)
    {

        try
        {
            string? profileName = _configuration["AWS:ProfileName"];
            string? configuredBucketName = _configuration["AWS:BucketName"];

            string? bucketName = !string.IsNullOrEmpty(configuredBucketName) ? configuredBucketName : Environment.GetEnvironmentVariable("AWS:BucketName");

            AmazonS3Client s3Client;
            Amazon.RegionEndpoint regionEndpoint;

            if (!string.IsNullOrEmpty(profileName))
            {
                var (ssoCredentials, region) = LoadSsoCredentials(_configuration["AWS:ProfileName"]);
                s3Client = new AmazonS3Client(ssoCredentials, region);
                regionEndpoint = region;
            }
            else
            {
                var temporaryCredentials = await GetCredentialsAsync();
                var region = Environment.GetEnvironmentVariable("AWS:Region");

                regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region);
                s3Client = new AmazonS3Client(temporaryCredentials, regionEndpoint);
            }

            var memoryStream = new MemoryStream(file);

            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileName,
                InputStream = memoryStream,
                ContentType = "application/pdf",
            };

            var response = await s3Client.PutObjectAsync(request);

            string fileUrl = $"https://{bucketName}.s3.{regionEndpoint.SystemName}.amazonaws.com/{fileName}";

            return fileUrl;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError("Error when put object on S3: {Message}.", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("An exception has occurred {Message}.", ex.Message);
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

        return (credentials, profileInfo.Region);
    }

    private static async Task<AWSCredentials> GetCredentialsAsync()
    {
        var accessKey = Environment.GetEnvironmentVariable("AWS:AccessKey");
        var secretKey = Environment.GetEnvironmentVariable("AWS:SecretKey");
        var roleArn = Environment.GetEnvironmentVariable("AWS:RoleArn");

        var credentials = new BasicAWSCredentials(accessKey, secretKey);

        var assumeRoleRequest = new AssumeRoleRequest
        {
            RoleArn = roleArn,
            RoleSessionName = "WebAppSession"
        };

        var stsClient = new AmazonSecurityTokenServiceClient(credentials, Amazon.RegionEndpoint.USEast2);

        var assumeRoleResponse = await stsClient.AssumeRoleAsync(assumeRoleRequest);

        var tempCredentials = assumeRoleResponse.Credentials;

        return new SessionAWSCredentials(tempCredentials.AccessKeyId, tempCredentials.SecretAccessKey, tempCredentials.SessionToken);
    }
}
