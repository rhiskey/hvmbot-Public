using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using vkMCBot.Mysql;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;
using VkNet.AudioBypassService.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using hvmbot.TelegramPart;
using VkNet.Enums.Filters;
using System.IO;
using System.Net;
using VkNet.Model.Attachments;
using hvmbot.Classes;
using VkNet.Model.GroupUpdate;

namespace hvmbot.VKPart
{
    class LongPollHandler
    {
        public static VkApi api = new VkApi();
        private static readonly string vkapiToken = Configuration.vkAuth.vkapiToken; //Обновления группы 
        private static readonly ulong groupID = Configuration.vkAuth.groupID;
        private static readonly string vkLogin = Configuration.vkAuth.vkLogin;
        private static readonly string vkPassword = Configuration.vkAuth.vkPassword;
        private readonly static ulong kateMobileAppID = Configuration.vkAuth.kateMobileAppID; //Скачивание музыки и фото

        static List<string> otpList = new List<string>();
        List<string> removeMe = new List<string>();
        static string logfile = "log.txt";

        static KeyboardButtonColor agree = KeyboardButtonColor.Positive;
        public static KeyboardButtonColor decine = KeyboardButtonColor.Negative;
        public static KeyboardButtonColor def = KeyboardButtonColor.Default;
        public static KeyboardButtonColor prim = KeyboardButtonColor.Primary;
        static LongPollServerResponse longPoolServerResponse;
        private static ulong ts;
        private static ulong? pts;
  
        enum PostState
        {
            imageSended = 0,
            audioSended = 0
        }
        public static void LongPollListener()
        {
            //Default menu
            KeyboardBuilder key = new KeyboardBuilder();
            KeyboardBuilder key_linked = new KeyboardBuilder();
            KeyboardBuilder key_unlinked = new KeyboardBuilder();
            KeyboardBuilder genres_menu_general = new KeyboardBuilder();

            key.AddButton("Привязать аккаунт", null, agree);
            key.AddButton("Удалить привязку", null, decine);

            genres_menu_general.AddButton("House", null, def);
            genres_menu_general.AddButton("Vocal Deep", null, def);
            genres_menu_general.AddButton("Bass", null, def);
            genres_menu_general.AddButton("Psy-Trance", null, def);
            MessageKeyboard keyboard_genres_general = genres_menu_general.Build();

            MessageKeyboard keyboard = key.Build();
            key_linked.AddButton("Удалить привязку", null, decine);
            MessageKeyboard keyboard_linked = key_linked.Build();

            key_unlinked.AddButton("Привязать аккаунт", null, agree);
            MessageKeyboard keyboard_unlinked = key_unlinked.Build();

            HashSet<string> messages = new HashSet<string>();

        Start:
            try
            {
            Restart:
                while (true) // Бесконечный цикл, получение обновлений
                {
                    var api = new VkApi();

                    try
                    {

                        api.Authorize(new ApiAuthParams()
                        {
                            AccessToken = vkapiToken,
                        });
                    }
                    catch (Exception apiAuth) { Logging.ErrorLogging(apiAuth); Logging.ReadError(); goto Restart; }
                    longPoolServerResponse = new LongPollServerResponse();
                    longPoolServerResponse = api.Groups.GetLongPollServer(groupID);               
                    BotsLongPollHistoryResponse poll = null;
                    try
                    {
                        poll = api.Groups.GetBotsLongPollHistory(
                           new BotsLongPollHistoryParams()
                           { Server = longPoolServerResponse.Server, Ts = longPoolServerResponse.Ts, Key = longPoolServerResponse.Key, Wait = 25 });
                        pts = longPoolServerResponse.Pts;
                        if (poll?.Updates == null) continue; 
                    }
                    catch (Exception ex)
                    {
                        Logging.ErrorLogging(ex);
                        Logging.ReadError();
                        goto Restart;
                    }
                    foreach (var a in poll.Updates)
                    {                  
                        PostType postType = new PostType();
                        postType = PostType.Post;

                        if (a.Type == GroupUpdateType.WallPostNew)
                            if (a.WallPost.PostType == postType)
                            {
                                Uri thumbUrl = null; //Картинка для телеграма (музыки) 200 кб 90х90px
                                string postMessage = a.WallPost.Text;

                                //Скачать песни из вложений - отправить в телеграм
                                var allAttachments = a.WallPost.Attachments;
                                Console.WriteLine(string.Format("Новый пост: {0}, вложений: {1} ", postMessage, allAttachments.Count));

                                List<VkNet.Model.Attachments.Audio> audios = new List<VkNet.Model.Attachments.Audio>();
                                List<string> trackIds = new List<string>();
  
                                List<string> photoList2Load = new List<string>();

                                if (allAttachments.Count > 0)
                                    foreach (var attach in allAttachments)
                                    {
                                        var attid = attach.Instance.Id;
                                        var attown = attach.Instance.OwnerId;

                                        string attachFullId = attown + "_" + attid;

                                        if (attach.Type == typeof(VkNet.Model.Attachments.Audio))
                                        //decode - download
                                        {
                                            trackIds.Add(attachFullId);
                                        }
                                        if (attach.Type == typeof(VkNet.Model.Attachments.Photo))
                                        {
                                            photoList2Load.Add(attachFullId);
                                        }

                                    }

                                List<VkNet.Model.Attachments.Photo> photoList = new List<VkNet.Model.Attachments.Photo>();
                                if (photoList2Load.Count > 0)
                                {
                                    try
                                    {
                                        api = new VkApi();
                                        //Get JPG Link
                                        api.Authorize(new ApiAuthParams()
                                        {
                                            Login = vkLogin,
                                            Password = vkPassword,
                                            ApplicationId = kateMobileAppID,
                                            Settings = Settings.All
                                        });
                                        Console.WriteLine("VK User auth для фото!");
                                        Thread.Sleep(1000);

                                        var photos = api.Photo.GetById(photoList2Load, null, photoSizes: true); 
                                        photoList = photos.ToList();

                                        if (photoList.Count > 0)
                                            using (WebClient webClient = new WebClient())
                                            {
                                                Classes.Photo photoClass2Process = GetLargePhoto.GetLargeAndThumbUrl(photoList);
                                                List<Uri> getLargePhoto = photoClass2Process.largePhotoUris;
                                                thumbUrl = photoClass2Process.smallPhotoUri;

                                                foreach (var largePhotoLink in getLargePhoto)
                                                {
                                                    webClient.DownloadFile(largePhotoLink.AbsoluteUri.ToString(), Path.GetFileName(largePhotoLink.ToString()));
                                                    Classes.Photo photoClass = new Classes.Photo(Path.GetFileName(largePhotoLink.ToString()), postMessage);

                                                    Thread senderTGPhotoThread = new Thread(TelegramSender.SendPhoto2TG);
                                                    senderTGPhotoThread.Start(photoClass);
                                                    Thread.Sleep(100);
                                                }
                                                getLargePhoto.Clear();
                                            }
                                    } catch (Exception sendPhotoEx) { Logging.ErrorLogging(sendPhotoEx); Logging.ReadError(); goto SendMusic; }
                                }
                                SendMusic:
                                if (trackIds.Count > 0)
                                {
                                    var services = new ServiceCollection();
                                    services.AddAudioBypass();
                                    api = new VkApi(services);

                                    try
                                    {
                                        api.Authorize(new ApiAuthParams()
                                        {
                                            Login = vkLogin,
                                            Password = vkPassword,
                                            ApplicationId = kateMobileAppID,
                                            Settings = Settings.All 
                                        }); 
                                        Console.WriteLine("VK User auth для музыки!");
                                    }
                                    catch (Exception apiEx) { Logging.ErrorLogging(apiEx); Logging.ReadError(); goto Restart;  } 
                                    audios = api.Audio.GetById(trackIds).ToList();

                                    foreach (var song in audios)
                                    {
                                        Uri audioUri = song.Url; //get URI

                                        if (audioUri != null) //Если запись не по подписке или не запрещена
                                        {

                                            Uri mp3link = AudioDownloader.DecodeAudioUrl(audioUri); 
                                            var uriString = mp3link.ToString();
                                            string mp3Direct = mp3link.AbsoluteUri; 
                                            int divider = mp3Direct.IndexOf('?');
                                            string clearLink = mp3Direct.Substring(0, divider);

                                            string mp3Title = song.Title;
                                            string mp3Artist = song.Artist;
                                            
                                            string mp3name = mp3Title + " - " + mp3Artist + ".mp3";
                                            
                                            if (song.Duration < 720) //12 минут
                                            {
                                                MP3 mP;
                                                if (thumbUrl!=null)
                                                    mP = new MP3(clearLink, mp3name, mp3Title, mp3Artist, postMessage, thumbUrl);
                                                else
                                                {
                                                    mP = new MP3(clearLink, mp3name, mp3Title, mp3Artist, postMessage);
                                                }
                                                Thread senderTGThread = new Thread(TelegramSender.SendMusic2TG);                                              
                                                senderTGThread.Start(mP);
                                                Thread.Sleep(500);
                                            }
                                            else Console.WriteLine("Слишком длинный трек >12 минут");
                                        }
                                        else continue;
                                    }                              
                                }

                                //Clear after All
                                try
                                {
                                    if (trackIds.Count > 0)
                                        trackIds.Clear();
                                    if (photoList.Count > 0)
                                        photoList.Clear();
                                    if (photoList2Load.Count > 0)
                                        photoList2Load.Clear();
                                }
                                catch (Exception ex) { Logging.ErrorLogging(ex); Logging.ReadError(); }
                            }
                      
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorLogging(ex);
                Logging.ReadError();
                goto Start;
            }
        }

      
    }
}
