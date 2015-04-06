using System.Text.RegularExpressions;
using SharpLoader.Exceptions;
using SharpLoader.Services.Contracts;

namespace SharpLoader.Services.Implementations
{
    public class UrlService : IUrlService
    {
        private const string UrlPattern = @"http://(www\.)?(?<domain>\w+\.\w{2,4})(/[\w&\d:]*)*/?";

        public bool IsValidUrl(string videoUrl)
        {
            var isValid = Regex.IsMatch(videoUrl, UrlPattern);
            return isValid;
        }

        public string GetDomainFromUrl(string url)
        {
            var match = Regex.Match(url, UrlPattern);
            var group = match.Groups["domain"];
            if (group.Success)
            {
                string domain = group.Captures[0].Value;
                return domain;
            }
            throw new InvalidUrlException();
        }
    }
}
