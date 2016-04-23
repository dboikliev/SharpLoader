namespace SharpLoader.Services.Contracts
{
    public interface IYoutubePlaylistLinksService
    {
        string[] ExtractPlaylistLinks(string playlistUrl);
    }
}
