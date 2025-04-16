namespace HrmBaharu.Application.Services;
public interface ISampleService
{
    Task<string> GetSampleDataAsync(string input, CancellationToken cancellationToken);
}
