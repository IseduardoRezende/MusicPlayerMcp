using Microsoft.Extensions.Logging;
using MusicPlayerMcp.Tools;
using NAudio.Wave;

namespace MusicPlayerMcp.Core
{
    public class AudioPlayerCore
    {
        private static WaveOutEvent? _player;
        private static AudioFileReader? _audioFile;

        public static Task PlayAsync(string filePath, ILogger<MusicPlayerTool> logger, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
                return Task.CompletedTask;

            _ = Task.Run(() =>
            {
                try
                {
                    StopInternal();

                    _audioFile = new AudioFileReader(filePath);
                    _player = new WaveOutEvent();

                    _player.Init(_audioFile);

                    _player.PlaybackStopped += (_, _) =>
                    {
                        _audioFile?.Dispose();
                        _audioFile = null;
                    };

                    _player.Play();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Play audio file fail");
                }

            }, cancellationToken);

            return Task.CompletedTask;
        }

        public static Task StopAsync(ILogger<MusicPlayerTool> logger, CancellationToken cancellationToken = default)
        {
            _ = Task.Run(() =>
            {
                try
                {
                    StopInternal();
                    logger.LogInformation("Music stopped");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Stop audio file fail");
                }

            }, cancellationToken);

            return Task.CompletedTask;
        }

        private static void StopInternal()
        {
            _player?.Stop();
            _player?.Dispose();
            _player = null;

            _audioFile?.Dispose();
            _audioFile = null;
        }
    }
}