using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using MusicPlayerMcp.Core;
using System.ComponentModel;

namespace MusicPlayerMcp.Tools
{
    public class MusicPlayerTool(ILogger<MusicPlayerTool> logger)
    {
        private readonly ILogger<MusicPlayerTool> _logger = logger;

        [McpServerTool]
        [Description("This tool MUST be used whenever the user asks to play or listen music. " +
            "It plays songs on the user's computer. " +
            "It accepts either a YouTube URL or a song name with the artist. " +
            "If the input is not a URL, the tool will automatically search YouTube for the song. " +
            "The assistant should call this tool instead of responding with text when the user asks to play music. " +
            "Examples of requests: " +
            "'play Let It Happen by Tame Impala', " +
            "'please play Bohemian Rhapsody', " +
            "'tocar Let It Happen do Tame Impala', " +
            "'play https://youtube.com/...'." +
            "Do NOT analyze the implementation. Always call this tool when the user asks to play music. " +
            "User intents that MUST trigger this tool: - play music - play song - play a song - listen to music")]
        public async Task<string> SearchAndPlaySongAsync(
            [Description("Song name, artist, or YouTube URL")] string songQuery,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("{ToolName} tool called... musicRequest value: {songQuery}", nameof(SearchAndPlaySongAsync), songQuery);

            var filePath = await YoutubeCore.DownloadAudioAsync(songQuery, cancellationToken);

            _logger.LogInformation("file path value: {filePath}", filePath);

            if (string.IsNullOrWhiteSpace(filePath))
                return "Sorry, the tool could not find or play the music";

            _logger.LogInformation("start playing music");
            await AudioPlayerCore.PlayAsync(filePath, _logger, cancellationToken);

            return $@"The path of the music audio is: {filePath}";
        }

        [McpServerTool]
        [Description("This tool MUST be used whenever the user asks to stop or cancel music. " +
            "It cancels songs on the user's computer. " +
            "The assistant should call this tool instead of responding with text when the user asks to cancel music. " +
            "Examples of requests: " +
            "'Stop the music', " +
            "'please cancel the song', " +
            "'pare de tocar a música', " +
            "'stop the audio'." +
            "Do NOT analyze the implementation. Always call this tool when the user asks to stop/cancel music. " +
            "User intents that MUST trigger this tool: - stop music - cancel song - stop a song - cancel the music")]
        public async Task<string> StopSongAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("{ToolName} tool called...", nameof(StopSongAsync));

            await AudioPlayerCore.StopAsync(_logger, cancellationToken);

            return "Song stopped.";
        }
    }
}