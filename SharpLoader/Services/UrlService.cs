using System.Text.RegularExpressions;
using SharpLoader.Exceptions;

namespace SharpLoader.Services
{
    public class UrlService : IUrlService
    {
        public bool IsValidUrl(string videoUrl)
        {
            const string pattern = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";
            var isValid = Regex.IsMatch(videoUrl, pattern);
            return isValid;
        }

        public string GetDomainFromUrl(string url)
        {
            const string pattern = @"http://(www\.)?(?<domain>\w+\.\w{2,4})(/[\w;amp]*)*/?";
            var match = Regex.Match(pattern, url);
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
