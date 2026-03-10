using Microsoft.Extensions.Logging;
using MusicPlayerMcp.Tools;
using NAudio.Wave;

namespace MusicPlayerMcp.Core
{
    public static class AudioPlayerCore
    {
        private static WaveOutEvent? _player;
        private static MediaFoundationReader? _audioFile;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public static bool IsPlaying()
        {
            return _player is not null && _audioFile is not null;
        }

        private static void OnStoppedAudioEvent(object? sender, StoppedEventArgs e)
        {
            _ = CleanupAsync();
        }

        public static async Task PlayFromFilePathAsync(string? filePath, ILogger<MusicPlayerTool> logger, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
                return;

            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                StopInternal();

                _audioFile = new MediaFoundationReader(filePath);
                _player = new WaveOutEvent();

                _player.PlaybackStopped += OnStoppedAudioEvent;
                _player.Init(_audioFile);
                _player.Play();

                logger.LogInformation("Streaming started.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fail to open audio stream.");
                StopInternal();
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public static async Task PlayFromUrlAsync(string? url, ILogger<MusicPlayerTool> logger, CancellationToken cancellationToken = default)
        {
            if (!ValidationUtils.IsValidUri(url))
                return;

            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                StopInternal();

                _audioFile = new MediaFoundationReader(url);
                _player = new WaveOutEvent();

                _player.PlaybackStopped += OnStoppedAudioEvent;
                _player.Init(_audioFile);
                _player.Play();

                logger.LogInformation("Streaming started.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fail to open audio stream.");
                StopInternal();
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public static async Task StopAsync(ILogger<MusicPlayerTool> logger, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                StopInternal();
                logger.LogInformation("Streaming stopped.");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public static async Task PauseAsync(ILogger<MusicPlayerTool> logger, CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                if (_player?.PlaybackState == PlaybackState.Playing)
                {
                    _player.Pause();
                    logger.LogInformation("Streaming paused.");
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public static async Task ResumeAsync(ILogger<MusicPlayerTool> logger, CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                if (_player?.PlaybackState == PlaybackState.Paused)
                {
                    _player.Play();
                    logger.LogInformation("Streaming resumed.");
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public static async Task SeekAsync(double minute, ILogger<MusicPlayerTool> logger, CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                if (_audioFile != null)
                {
                    _audioFile.CurrentTime = TimeSpan.FromMinutes(minute);
                    logger.LogInformation("Streaming current time: {Value}min", minute);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private static void StopInternal()
        {
            if (_player != null)
            {
                _player.PlaybackStopped -= OnStoppedAudioEvent;
                _player.Stop();
                _player.Dispose();
                _player = null;
            }

            _audioFile?.Dispose();
            _audioFile = null;
        }

        private static async Task CleanupAsync(CancellationToken cancellationToken = default)
        {
            if (_semaphore.CurrentCount > 0)
            {
                await _semaphore.WaitAsync(cancellationToken);

                try
                {
                    StopInternal();
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }
    }
}