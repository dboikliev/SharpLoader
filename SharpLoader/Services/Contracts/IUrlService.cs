namespace SharpLoader.Services.Contracts
{
    public interface IUrlService
    {
        bool IsValidUrl(string videoUrl);
        string GetDomainFromUrl(string url);
    }
}
