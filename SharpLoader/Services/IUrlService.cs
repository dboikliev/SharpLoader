namespace SharpLoader.Services
{
    public interface IUrlService
    {
        bool IsValidUrl(string videoUrl);
        string GetDomainFromUrl(string url);
    }
}
