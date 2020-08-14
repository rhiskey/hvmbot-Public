using hvmbot.TelegramPart;
using hvmbot.VKPart;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace hvmbot.Classes
{
    static class TelegramSender
    {
        //await TelegramMusic.SendTextAsync("TestText");
        public static async void SendPhoto2TG(object photoFile)
        {
            Classes.Photo photoObject = (Classes.Photo)photoFile;
            string caption = photoObject.Caption;
            string fileName = photoObject.LocalPath;
            if (caption == null)
                caption = "vk.com/hvmlabel #hvm";
            //await TelegramMusic.SendPhotoAsync(photoUrl, caption);
            await TelegramMusic.SendPhotoFromFileAsync(fileName, caption);
        }

        public static async void SendMusic2TG(object mp3Class)
        {
            MP3 mp3classFile = (MP3)mp3Class;
            string url = mp3classFile.Link;
            string file = mp3classFile.Name;
            string msg = mp3classFile.Message;
            string author = mp3classFile.Artists;
            string title = mp3classFile.Title;
            Uri thumb = mp3classFile.Thumb;

            //string fileName = "music.mp3";
            //bool isDowloaded = false;
            //isDowloaded = AudioDownloader.DownloadMP3FromUrlAsync(url, file);
            //do { Thread.Sleep(100); } while (isDowloaded == false);
            try
            {
                AudioDownloader.DownloadMP3FromUrl(url, file);

                long length = new System.IO.FileInfo(file).Length;//in bytes 

                double fifty_megabytes = 5e+7;

                if (length < fifty_megabytes) //Проверка на размер для телеги
                    await TelegramMusic.SendAudioFromFileAsync(file, msg, author, title, thumb);
                else Console.WriteLine("Слишком большой файл >50MB");
            }
            catch (Exception ex) {Logging.ErrorLogging(ex); Logging.ReadError(); }
            //await TelegramMusic.SendAudioAsync(url);
        }

        public static async void SendPoll2TG(object pollClass)
        {
            TGPoll tGPoll = (TGPoll)pollClass;
            await TelegramMusic.SendPoll(tGPoll.Question, tGPoll.Answers);
        }
        //public static async void SendMusic2TG(Uri mp3URI)
        //{
        //    ////Convert URI TO string to MP3
        //    //await TelegramMusic.SendAudioAsync(mp3Url);
        //}
    }
}
