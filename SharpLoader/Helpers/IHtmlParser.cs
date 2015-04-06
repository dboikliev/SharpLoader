using System.Collections.Generic;

namespace SharpLoader.Helpers
{
    public interface IHtmlParser
    {
        Dictionary<string, string> ExtractValues(params string[] parameters);
    }
}
