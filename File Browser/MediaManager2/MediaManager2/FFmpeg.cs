using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FFmpeg.AutoGen;
using System.Runtime.InteropServices;
using System.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors;

namespace MediaManager2
{
    public static class FFmpeg
    {
        private readonly static int TargetWidth = 480;

#if DEBUG
        private readonly static string ffmpegPath = @"..\..\..\FFmpeg\bin\";
#else
        private readonly static string ffmpegPath = @"FFmpeg\bin\";
#endif

        static FFmpeg()
        {
            ffmpeg.RootPath = ffmpegPath; 
        }
        public static Task<int> GetTotalFramesAsync(string inputFile)
        {
            return Task.Run(() => GetTotalFrames(inputFile));
        }
        public static unsafe int GetTotalFrames(string inputFile)
        {
            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException($"Input file not found: {inputFile}");
            }

            AVFormatContext* formatContext = ffmpeg.avformat_alloc_context();
            if (ffmpeg.avformat_open_input(&formatContext, inputFile, null, null) != 0)
            {
                throw new Exception("Could not open input file.");
            }

            if (ffmpeg.avformat_find_stream_info(formatContext, null) != 0)
            {
                throw new Exception("Could not find stream info.");
            }

            int videoStreamIndex = ffmpeg.av_find_best_stream(formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, null, 0);
            if (videoStreamIndex < 0)
            {
                throw new Exception("Could not find video stream in file.");
            }

            AVStream* videoStream = formatContext->streams[videoStreamIndex];

            if (videoStream->nb_frames > 0)
            {
                int totalFrames = (int)videoStream->nb_frames;
                ffmpeg.avformat_close_input(&formatContext);
                return totalFrames;
            }
            else
            {
                AVCodec* codec = ffmpeg.avcodec_find_decoder(videoStream->codecpar->codec_id);
                AVCodecContext* codecContext = ffmpeg.avcodec_alloc_context3(codec);

                if (ffmpeg.avcodec_parameters_to_context(codecContext, videoStream->codecpar) != 0)
                {
                    throw new Exception("Could not copy codec parameters to context.");
                }

                if (ffmpeg.avcodec_open2(codecContext, codec, null) != 0)
                {
                    throw new Exception("Could not open codec.");
                }

                AVPacket* packet = ffmpeg.av_packet_alloc();
                AVFrame* frame = ffmpeg.av_frame_alloc();
                int frameCount = 0;

                while (ffmpeg.av_read_frame(formatContext, packet) >= 0)
                {
                    if (packet->stream_index == videoStreamIndex)
                    {
                        if (ffmpeg.avcodec_send_packet(codecContext, packet) >= 0)
                        {
                            while (ffmpeg.avcodec_receive_frame(codecContext, frame) >= 0)
                            {
                                frameCount++;
                            }
                        }
                    }
                    ffmpeg.av_packet_unref(packet);
                }

                // Cleanup
                ffmpeg.av_packet_free(&packet);
                ffmpeg.av_frame_free(&frame);
                ffmpeg.avcodec_free_context(&codecContext);
                ffmpeg.avformat_close_input(&formatContext);

                return frameCount;
            }
        }

        public static Task<VideoMetadata> ExtractFramesAsync(string inputFile, string outputFolder, Action<int> progress)
        {
            return Task.Run(() => ExtractFrames(inputFile, outputFolder, progress));
        }

        public static unsafe VideoMetadata ExtractFrames(string inputFile, string outputFolder, Action<int> progress)
        {

            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException($"Input file not found: {inputFile}");
            }

            AVFormatContext* formatContext = ffmpeg.avformat_alloc_context();
            if (ffmpeg.avformat_open_input(&formatContext, inputFile, null, null) != 0)
            {
                throw new Exception("Could not open input file.");
            }

            if (ffmpeg.avformat_find_stream_info(formatContext, null) != 0)
            {
                throw new Exception("Could not find stream info.");
            }

            int videoStreamIndex = ffmpeg.av_find_best_stream(formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, null, 0);
            if (videoStreamIndex < 0)
            {
                throw new Exception("Could not find video stream in file.");
            }

            AVStream* videoStream = formatContext->streams[videoStreamIndex];
            AVCodecParameters* codecParameters = videoStream->codecpar;
            AVCodec* codec = ffmpeg.avcodec_find_decoder(codecParameters->codec_id);
            AVCodecContext* codecContext = ffmpeg.avcodec_alloc_context3(codec);

            if (ffmpeg.avcodec_parameters_to_context(codecContext, codecParameters) != 0)
            {
                throw new Exception("Could not copy codec parameters to context.");
            }

            if (ffmpeg.avcodec_open2(codecContext, codec, null) != 0)
            {
                throw new Exception("Could not open codec.");
            }

            int frameRate = (int)Math.Round((double)videoStream->avg_frame_rate.num / videoStream->avg_frame_rate.den);
            int originalWidth = codecContext->width;
            int originalHeight = codecContext->height;

            int convertedPixelWidth = originalWidth <= TargetWidth ? originalWidth : TargetWidth;
            int convertedPixelHeight = (int)Math.Round((double)convertedPixelWidth / originalWidth * originalHeight);      

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            SwsContext* swsContext = ffmpeg.sws_getContext(
                codecContext->width, codecContext->height, codecContext->pix_fmt,
                convertedPixelWidth, convertedPixelHeight, AVPixelFormat.AV_PIX_FMT_RGB24,
                ffmpeg.SWS_BICUBIC, null, null, null);

            AVFrame* frame = ffmpeg.av_frame_alloc();
            AVFrame* scaledFrame = ffmpeg.av_frame_alloc();

            int bufferSize = ffmpeg.av_image_get_buffer_size(AVPixelFormat.AV_PIX_FMT_RGB24, convertedPixelWidth, convertedPixelHeight, 1);
            byte* buffer = (byte*)ffmpeg.av_malloc((ulong)bufferSize);

            byte_ptrArray4 dstData = new byte_ptrArray4();
            int_array4 dstLinesize = new int_array4();

            ffmpeg.av_image_fill_arrays(ref dstData, ref dstLinesize, buffer, AVPixelFormat.AV_PIX_FMT_RGB24, convertedPixelWidth, convertedPixelHeight, 1);

            for (uint i = 0; i < 4; i++)
            {
                scaledFrame->data[i] = dstData[i];
                scaledFrame->linesize[i] = dstLinesize[i];
            }

            AVPacket* packet = ffmpeg.av_packet_alloc();
            int frameIndex = 0;

            while (ffmpeg.av_read_frame(formatContext, packet) >= 0)
            {
                if (packet->stream_index == videoStreamIndex)
                {
                    if (ffmpeg.avcodec_send_packet(codecContext, packet) >= 0)
                    {
                        while (ffmpeg.avcodec_receive_frame(codecContext, frame) >= 0)
                        {
                            ffmpeg.sws_scale(swsContext, frame->data, frame->linesize, 0, codecContext->height, scaledFrame->data, scaledFrame->linesize);

                            string outputPath = Path.Combine(outputFolder, $"{frameIndex:D4}.jpg");
                            SaveFrameAsImage(outputPath, scaledFrame, convertedPixelWidth, convertedPixelHeight);
                           
                            frameIndex++;
                            progress?.Invoke(frameIndex);
                        }
                    }
                }
                ffmpeg.av_packet_unref(packet);
            }

            ffmpeg.av_free(buffer);
            ffmpeg.av_frame_free(&frame);
            ffmpeg.av_frame_free(&scaledFrame);
            ffmpeg.sws_freeContext(swsContext);
            ffmpeg.avcodec_free_context(&codecContext);
            ffmpeg.avformat_close_input(&formatContext);

            return new VideoMetadata
            {
                FrameRate = frameRate,
                TotalFrames = frameIndex,
                ConvertedPixelWidth = convertedPixelWidth,
                ConvertedPixelHeight = convertedPixelHeight
            };
        }

        private static unsafe void SaveFrameAsImage(string path, AVFrame* frame, int width, int height)
        {
            using (var image = new Image<Rgb24>(width, height))
            {
                for (int y = 0; y < height; y++)
                {
                    byte* sourceRow = frame->data[0] + y * frame->linesize[0];

                    for (int x = 0; x < width; x++)
                    {
                        var pixel = new Rgb24(
                            sourceRow[x * 3 + 0],
                            sourceRow[x * 3 + 1],
                            sourceRow[x * 3 + 2]
                        );
                        image[x, y] = pixel;
                    }
                }

                image.SaveAsJpeg(path);
            }
        }
    }
}
