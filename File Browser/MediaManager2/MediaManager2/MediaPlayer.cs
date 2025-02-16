using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.IO;


namespace MediaManager2
{
    public static class MediaPlayer
    {
        private static readonly Dictionary<string, (VideoMetadata Metadata, List<string> FrameImages)> videoCache = new();
        // frame cache
        public static bool IsPlaying { get; set; } = true;
        private static int videosPlaying = 0;
        private const int MaxCacheSize = 500;
        private static string mouseOn = "";
        private const int FrameSkipTolerance = 45;

        public static async Task PlayMediaAsync(FileItem file, Image imageControl, CancellationToken cancellationToken)
        {
            if (file.Type == "video" || file.Type == "gif")
                await PlayFramesAsync(file.Path, imageControl, cancellationToken);
            else
                await WaitForImageAsync(file.Path, imageControl, cancellationToken);
        }

        public static void Test(string path)
        {
            if (!MediaCache.FindVideo(path, out var metadata, out var frameImages))
                return;

            foreach (var image in frameImages)
            {
                try
                {
                    using (var stream = new FileStream(image, FileMode.Open, FileAccess.Read))
                    {
                        var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        var frame = decoder.Frames[0];
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error testing: {ex.Message}");
                }
            }         
        }
        private static WriteableBitmap InitializeWriteableBitmap(Image videoImage, string firstFramePath)
        {          
            BitmapImage firstFrame = new BitmapImage(new Uri(firstFramePath));

            WriteableBitmap writeableBitmap = new WriteableBitmap(
                firstFrame.PixelWidth,
                firstFrame.PixelHeight,
                firstFrame.DpiX,
                firstFrame.DpiY,
                firstFrame.Format,
                null);
            
            videoImage.Dispatcher.Invoke(() => videoImage.Source = writeableBitmap);
            return writeableBitmap;
        }
        private static async Task PlayFramesAsync(string videoPath, Image videoImage, CancellationToken cancellationToken)
        {
            VideoMetadata metadata;
            List<string> frameImages;

            if (videoCache.TryGetValue(videoPath, out var cacheEntry))
            {
                (metadata, frameImages) = cacheEntry;
            }
            else
            {
                while (!MediaCache.FindVideo(videoPath, out metadata, out frameImages))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    await Task.Delay(100);
                }
                if (videoCache.Count >= MaxCacheSize)
                {
                    var oldestEntry = videoCache.Keys.First();
                    videoCache.Remove(oldestEntry);
                }
                videoCache[videoPath] = (metadata, frameImages);
            }
        
            if (frameImages.Count == 0)
            {
                Console.WriteLine($"No frames found in folder: {frameImages}");
                return;
            }

            WriteableBitmap writeableBitmap = InitializeWriteableBitmap(videoImage, frameImages[0]);
            int frameDelay = 1000 / metadata.FrameRate;

            int frameIndex = 0;

            Interlocked.Increment(ref videosPlaying);
            while (true)
            {
                int frameSkip = (int)(Math.Exp(videosPlaying / FrameSkipTolerance) - 1);
                if (cancellationToken.IsCancellationRequested)
                {       
                    videoImage.Dispatcher.Invoke(() =>
                    {
                        videoImage.Source = null;
                    });
                    
                    Interlocked.Decrement(ref videosPlaying);
                    return;
                }
                if (IsPlaying || mouseOn == videoPath)
                {                   
                    if (!await RenderFrame(frameImages[frameIndex], writeableBitmap))
                    {
                        videoCache.Remove(videoPath);
                        Interlocked.Decrement(ref videosPlaying);
                        return;
                    }

                    if (mouseOn == videoPath)
                        frameIndex = (frameIndex + 1) % frameImages.Count;
                    else
                        frameIndex = (frameIndex + 1 + frameSkip) % frameImages.Count;
                }
                
                try
                {
                    if (mouseOn == videoPath)
                        await Task.Delay(frameDelay, cancellationToken);
                    else
                        await Task.Delay(frameDelay + frameDelay * frameSkip, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    Interlocked.Decrement(ref videosPlaying);
                    return;
                }   
            }
        }
        private static async Task WaitForImageAsync(string imagePath, Image image, CancellationToken cancellationToken)
        {
            string cacheFilePath;
            while (!MediaCache.FindImage(imagePath, out cacheFilePath))
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
  
                await Task.Delay(100);
            }

            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(cacheFilePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                image.Source = bitmap;
            }
            catch (Exception e) {
                Console.WriteLine($"Error writing to image source {e.Message}");
            }    
        }
        private static async Task<bool> RenderFrame(string framePath, WriteableBitmap bitmap)
        {
            try
            {
                using (var stream = new FileStream(framePath, FileMode.Open, FileAccess.Read))
                {
                    var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    var frame = decoder.Frames[0];

                    await bitmap.Dispatcher.InvokeAsync(() =>
                    {
                        bitmap.Lock();
                        frame.CopyPixels(
                            new Int32Rect(0, 0, frame.PixelWidth, frame.PixelHeight),
                            bitmap.BackBuffer,
                            bitmap.BackBufferStride * frame.PixelHeight,
                            bitmap.BackBufferStride);
                        bitmap.AddDirtyRect(new Int32Rect(0, 0, frame.PixelWidth, frame.PixelHeight));
                        bitmap.Unlock();
                    });

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rendering frame: {ex.Message}");
                return false;
            }
            return true;
        }
        public static long GetCacheSize()
        {
            long totalSize = 0;

            foreach (var entry in videoCache)
            {
                var (metadata, frameImages) = entry.Value;
              
                totalSize += sizeof(int)*4;

                foreach (var framePath in frameImages)
                {
                    totalSize += framePath.Length * sizeof(char);
                }
            }

            return totalSize;
        }
        
        public static void MouseEnter(FileItem file) {
            mouseOn = file.Path;
        }
        public static void MouseLeave(FileItem file)
        {
            mouseOn = "";
        }

    }
}
