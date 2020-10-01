using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace hvmbot.VKPart
{
    static class AudioDownloader
    {
        static bool isDownloaded = false;

        //Get MP3 Link
        public static Uri DecodeAudioUrl(this Uri audioUrl)
        {
            var segments = audioUrl.Segments.ToList();

            segments.RemoveAt((segments.Count - 1) / 2);
            segments.RemoveAt(segments.Count - 1);

            segments[segments.Count - 1] = segments[segments.Count - 1].Replace("/", ".mp3");

            return new Uri($"{audioUrl.Scheme}://{audioUrl.Host}{string.Join("", segments)}{audioUrl.Query}");
        }

        public static bool DownloadMP3FromUrl(string url, string fileName)
        {
            isDownloaded = false;

            new WebClient().DownloadFile(new Uri(url), fileName);
            isDownloaded = true;

            return isDownloaded;
        }

        public static void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            isDownloaded = true;
        }
    }
}
