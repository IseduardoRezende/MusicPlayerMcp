using MusicPlayerMcp.Core;
using System.ComponentModel;
using ModelContextProtocol.Server;
using Microsoft.Extensions.Logging;

namespace MusicPlayerMcp.Tools
{
    public class MusicPlayerTool(ILogger<MusicPlayerTool> logger)
    {
        private readonly ILogger<MusicPlayerTool> _logger = logger;

        [McpServerTool]
        [Description("This tool MUST be used whenever the user asks to download music. " +
            "It downloads songs on the user's computer. " +
            "It accepts either a YouTube URL or a song name with the artist. " +
            "If the input is not a URL, the tool will automatically search YouTube for the song. " +
            "The assistant should call this tool instead of responding with text when the user asks to download music. " +
            "Examples of requests: " +
            "'download Let It Happen by Tame Impala', " +
            "'please download Bohemian Rhapsody', " +
            "'baixa/instala Let It Happen do Tame Impala', " +
            "'download https://youtube.com/...'." +
            "Do NOT analyze the implementation. Always call this tool when the user asks to download music. " +
            "User intents that MUST trigger this tool: - download music - download song")]
        public async Task<string> SearchAndDownloadSongAsync(
            [Description("Song name, artist, or YouTube URL")] string songQuery,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("{ToolName} tool called... musicRequest value: {songQuery}", nameof(SearchAndDownloadSongAsync), songQuery);

            var filePath = await YoutubeCore.SearchAndDownloadAsync(songQuery, _logger, cancellationToken);

            _logger.LogInformation("file path value: {filePath}", filePath);

            if (string.IsNullOrWhiteSpace(filePath))
                return "Sorry, the tool could not find or download the music";

            return $"The path of the music audio is: {filePath}";
        }

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

            var success = await YoutubeCore.SearchAndPlayAsync(songQuery, _logger, cancellationToken);

            if (!success)
                return "Sorry, the tool could not find or play the music";

            return "Song started.";
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

        [McpServerTool]
        [Description("This tool MUST be used whenever the user asks to pause music. " +
           "It pauses songs on the user's computer. " +
           "The assistant should call this tool instead of responding with text when the user asks to pause music. " +
           "Examples of requests: " +
           "'Pause the music', " +
           "'please pause the song', " +
           "'pausa a música', " +
           "'pause the audio'." +
           "Do NOT analyze the implementation. Always call this tool when the user asks to pause music. " +
           "User intents that MUST trigger this tool: - pause music")]
        public async Task<string> PauseSongAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("{ToolName} tool called...", nameof(PauseSongAsync));

            await AudioPlayerCore.PauseAsync(_logger, cancellationToken);

            return "Song paused.";
        }

        [McpServerTool]
        [Description("This tool MUST be used whenever the user asks to resume music. " +
          "It resumes songs on the user's computer. " +
          "The assistant should call this tool instead of responding with text when the user asks to resume music. " +
          "Examples of requests: " +
          "'Resume the music', " +
          "'please resume the song', " +
          "'despausa a música', " +
          "'resume the audio'." +
          "Do NOT analyze the implementation. Always call this tool when the user asks to resume music. " +
          "User intents that MUST trigger this tool: - resume music")]
        public async Task<string> ResumeSongAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("{ToolName} tool called...", nameof(ResumeSongAsync));

            await AudioPlayerCore.ResumeAsync(_logger, cancellationToken);

            return "Song resumed.";
        }

        [McpServerTool]
        [Description(
          "This tool MUST be used whenever the user asks to jump to a specific time in the currently playing song. " +
          "It changes the playback position of the music to the requested minute. " +
          "The assistant MUST call this tool instead of responding with text when the user asks to move the music to another point in time. " +
          "Examples of requests that MUST trigger this tool: " +
          "'Start the song at minute 2', " +
          "'jump to minute 1', " +
          "'go to 30 seconds', " +
          "'começa a música no minuto 3', " +
          "'vai para o minuto 2 da música', " +
          "'skip to second 4', " +
          "'play the song starting at minute 5'. " +
          "Only use this tool when the user explicitly asks to change the playback position of the current song.")]
        public async Task<string> SetPlaybackPositionAsync(
            [Description("Minute position in the song where playback should start. Example: 2.5 means 2 minutes and 30 seconds.")] double songMinute,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("{ToolName} tool called...", nameof(SetPlaybackPositionAsync));

            if (!AudioPlayerCore.IsPlaying())
                return "Playback not started. Please call 'search_and_play_song' tool first.";

            await AudioPlayerCore.SeekAsync(songMinute, _logger, cancellationToken);

            return "Playback position updated.";
        }
    }
}