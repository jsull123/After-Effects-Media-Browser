using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Windows.Media.Imaging;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace MediaManager2
{
    public class VideoMetadata
    {
        public int FrameRate { get; set; }
        public int TotalFrames { get; set; }
        public int ConvertedPixelWidth { get; set; }
        public int ConvertedPixelHeight { get; set; }
    }

    public static class MediaCache
    {
        private static readonly string _cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "smm", "MediaCache");
        private static readonly int MaxThreads = 5;

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(MaxThreads);

        private static IProgress<(int, int)> _fileProgress;
        private static IProgress<(int, int)> _frameProgress;

        private static readonly HashSet<string> _workingFiles = new HashSet<string>();
        private static int _totalFrames;
        private static int _processedFrames;
        private static int _totalFiles;
        private static int _processedFiles;

        private static int _activeJobs = 0;

        static MediaCache()
        {
            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }
        }

        /// <summary>
        /// Creates cache for the provided files.
        /// </summary>
        /// <param name="files">The collection of files to cache.</param>
        /// <param name="fileProgress">Progress indicator for file caching.</param>
        /// <param name="frameProgress">Progress indicator for frame caching.</param>
        public static async Task CreateAsync(IEnumerable<FileItem> files, IProgress<(int, int)> fileProgress = null, IProgress<(int, int)> frameProgress = null)
        {
            lock (_workingFiles)
            {
                _activeJobs++;
            }

            try
            {
                _fileProgress = fileProgress ?? new Progress<(int, int)>();
                _frameProgress = frameProgress ?? new Progress<(int, int)>();

                var uncachedMedia = files.Where(file => IsUncached(file)).ToList();

                // Increment totals without resetting for new batches
                Interlocked.Add(ref _totalFiles, uncachedMedia.Count);
                Interlocked.Add(ref _totalFrames, uncachedMedia.Sum(file => EstimateTotalFrames(file)));

                _fileProgress.Report((_processedFiles, _totalFiles));
                _frameProgress.Report((_processedFrames, _totalFrames));

                var tasks = uncachedMedia.Select(ProcessMediaAsync).ToList();
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            finally
            {
                lock (_workingFiles)
                {
                    _activeJobs--;
                }
            }
        }

        private static int EstimateTotalFrames(FileItem file)
        {
            return file.Type switch
            {
                "video" or "gif" => FFmpeg.GetTotalFramesAsync(file.Path).Result,
                "image" => 1,
                _ => 0
            };
        }

        private static bool IsUncached(FileItem file)
        {
            return file.Type switch
            {
                "video" or "gif" => !FindVideo(file.Path, out _, out _) && !_workingFiles.Contains(file.Path),
                "image" => !FindImage(file.Path, out _) && !_workingFiles.Contains(file.Path),
                _ => false
            };
        }

        private static async Task ProcessMediaAsync(FileItem file)
        {
            await Semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                _workingFiles.Add(file.Path);

                if (file.Type == "video" || file.Type == "gif")
                {
                    await CreateCachedVideoAsync(file.Path).ConfigureAwait(false);
                }
                else if (file.Type == "image")
                {
                    await CreateCachedImageAsync(file.Path).ConfigureAwait(false);
                }

                Interlocked.Increment(ref _processedFiles);
                _fileProgress?.Report((_processedFiles, _totalFiles));
            }
            finally
            {
                _workingFiles.Remove(file.Path);
                Semaphore.Release();
            }
        }

        private static async Task CreateCachedImageAsync(string imagePath)
        {
            string cachePath = GetCacheFilePathImage(imagePath);
            if (File.Exists(cachePath))
            {
                File.Delete(cachePath);
            }

            await Task.Run(() =>
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.DecodePixelWidth = 256;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                using var stream = new FileStream(cachePath, FileMode.Create);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);

                Interlocked.Increment(ref _processedFrames);
                _frameProgress?.Report((_processedFrames, _totalFrames));
            }).ConfigureAwait(false);
        }

        private static async Task CreateCachedVideoAsync(string videoPath)
        {
            string cachePath = GetCacheFilePathVideo(videoPath);
            string metadataFilePath = Path.Combine(cachePath, "metadata.json");

            if (Directory.Exists(cachePath))
            {
                Directory.Delete(cachePath, true);
            }

            Directory.CreateDirectory(cachePath);

            VideoMetadata metadata = await FFmpeg.ExtractFramesAsync(videoPath, cachePath, progress =>
            {
                Interlocked.Increment(ref _processedFrames);
                _frameProgress?.Report((_processedFrames, _totalFrames));
            }).ConfigureAwait(false);

            await File.WriteAllTextAsync(metadataFilePath, JsonConvert.SerializeObject(metadata)).ConfigureAwait(false);
        }

        public static string GetCacheFilePathVideo(string videoPath)
        {
            return Path.Combine(_cacheDirectory, HashString(videoPath));
        }

        private static string GetCacheFilePathImage(string imagePath)
        {
            return Path.Combine(_cacheDirectory, $"{HashString(imagePath)}.png");
        }

        public static bool FindImage(string imagePath, out string cacheFilePath)
        {
            cacheFilePath = GetCacheFilePathImage(imagePath);
            return File.Exists(cacheFilePath);
        }

        public static bool FindVideo(string videoPath, out VideoMetadata metadata, out List<string> frameImages)
        {
            metadata = null;
            frameImages = null;

            string cachePath = GetCacheFilePathVideo(videoPath);
            string metadataFilePath = Path.Combine(cachePath, "metadata.json");

            if (Directory.Exists(cachePath) && File.Exists(metadataFilePath))
            {
                try
                {
                    string json = File.ReadAllText(metadataFilePath);
                    metadata = JsonConvert.DeserializeObject<VideoMetadata>(json);
                    frameImages = Directory.GetFiles(cachePath, "*.jpg").OrderBy(f => f).ToList();

                    return metadata != null && frameImages.Count == metadata.TotalFrames;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        public static async Task<long> GetCacheSize()
        {
            if (!Directory.Exists(_cacheDirectory))
                return 0;

            return await Task.Run(() =>
            {
                try
                {
                    return Directory.EnumerateFiles(_cacheDirectory, "*", SearchOption.AllDirectories)
                        .Sum(file => new FileInfo(file).Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error calculating cache size: {ex.Message}");
                    return 0;
                }
            });
        }

        public static async Task ClearCacheWithProgress(IProgress<int> progress)
        {
            var filesToDelete = new List<string>();
            var directoriesToDelete = new List<string>();

            if (Directory.Exists(_cacheDirectory))
            {
                filesToDelete = Directory.EnumerateFiles(_cacheDirectory, "*", SearchOption.AllDirectories).ToList();
                directoriesToDelete = Directory.EnumerateDirectories(_cacheDirectory).ToList();
            }

            int totalItems = filesToDelete.Count + directoriesToDelete.Count;
            int processedItems = 0;

            foreach (var file in filesToDelete)
            {
                await Task.Run(() => File.Delete(file));
                processedItems++;
                progress?.Report((processedItems * 100) / totalItems);
            }

            foreach (var directory in directoriesToDelete)
            {
                await Task.Run(() => Directory.Delete(directory, true));
                processedItems++;
                progress?.Report((processedItems * 100) / totalItems);
            }
        }

        public static async Task ClearCache()
        {
            if (Directory.Exists(_cacheDirectory))
            {
                await Task.Run(() =>
                {
                    foreach (var file in Directory.EnumerateFiles(_cacheDirectory, "*", SearchOption.AllDirectories))
                    {
                        File.Delete(file);
                    }

                    foreach (var directory in Directory.EnumerateDirectories(_cacheDirectory))
                    {
                        Directory.Delete(directory, true);
                    }
                });
            }
        }

        public static bool GetStatus(out double progress)
        {
            lock (_workingFiles)
            {
                if (_activeJobs > 0)
                {        
                    if (_totalFrames > 0 && _totalFiles > 0)
                    {
                        double frameProgress = (double)_processedFrames / _totalFrames;
                        progress = frameProgress * 100;
                    }
                    else
                    {
                        progress = 0;
                    }

                    return true;
                }
                else
                { 
                    ResetProgressTracking();
                    progress = -1;
                    return false;
                }
            }
        }

        private static void ResetProgressTracking()
        {
            _totalFiles = 0;
            _processedFiles = 0;
            _totalFrames = 0;
            _processedFrames = 0;
        }

        private static string HashString(string input)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}
