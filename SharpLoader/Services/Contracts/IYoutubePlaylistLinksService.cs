using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLoader.Services.Contracts
{
    public interface IYoutubePlaylistLinksService
    {
        string[] ExtractPlaylistLinks(string playlistUrl);
    }
}
