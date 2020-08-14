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
        //https://vkhost.github.io

        ////Not Needed
        //public static string userToken = Configuration.vkAuth.userToken; //New acc + new app
        //public static string kateMobileToken = Configuration.vkAuth.kateMobileToken;
        //private static readonly string groupToken = Configuration.vkAuth.groupToken; //new acc + new app
        ////Not Needed
        private static readonly string vkapiToken = Configuration.vkAuth.vkapiToken; //Обновления группы 
        private static readonly ulong groupID = Configuration.vkAuth.groupID;
        private static readonly string vkLogin = Configuration.vkAuth.vkLogin;
        private static readonly string vkPassword = Configuration.vkAuth.vkPassword;
        private readonly static ulong kateMobileAppID = Configuration.vkAuth.kateMobileAppID; //Скачивание музыки и фото

        static List<string> otpList = new List<string>();
        //Add your objects for each string 
        List<string> removeMe = new List<string>();
        //Queue<string> otpQ = new Queue<string>();
        static string logfile = "log.txt";

        static KeyboardButtonColor agree = KeyboardButtonColor.Positive;
        public static KeyboardButtonColor decine = KeyboardButtonColor.Negative;
        public static KeyboardButtonColor def = KeyboardButtonColor.Default;
        public static KeyboardButtonColor prim = KeyboardButtonColor.Primary;
        static LongPollServerResponse longPoolServerResponse;
        private static ulong ts;
        private static ulong? pts;

  
        //OLD Working
        enum PostState
        {
            imageSended = 0,
            audioSended = 0
        }
        public static void LongPollListener()
        {
            //KeyboardButtonColor agree = KeyboardButtonColor.Positive;
            //KeyboardButtonColor decine = KeyboardButtonColor.Negative;
            //Default menu
            KeyboardBuilder key = new KeyboardBuilder();
            //Когда привязан уже
            KeyboardBuilder key_linked = new KeyboardBuilder();
            //Когда удалили привязку
            KeyboardBuilder key_unlinked = new KeyboardBuilder();
            //Стили 1
            KeyboardBuilder genres_menu_general = new KeyboardBuilder();

            key.AddButton("Привязать аккаунт", null, agree);
            key.AddButton("Удалить привязку", null, decine);

            genres_menu_general.AddButton("House", null, def);
            genres_menu_general.AddButton("Vocal Deep", null, def);
            genres_menu_general.AddButton("Bass", null, def);
            genres_menu_general.AddButton("Psy-Trance", null, def);
            //genres_menu_general.AddButton("Trap", null, def);
            MessageKeyboard keyboard_genres_general = genres_menu_general.Build();

            MessageKeyboard keyboard = key.Build();

            ////Когда привязан уже
            //KeyboardBuilder key_linked = new KeyboardBuilder();
            key_linked.AddButton("Удалить привязку", null, decine);
            MessageKeyboard keyboard_linked = key_linked.Build();

            ////Когда удалили привязку
            //KeyboardBuilder key_unlinked = new KeyboardBuilder();
            key_unlinked.AddButton("Привязать аккаунт", null, agree);
            MessageKeyboard keyboard_unlinked = key_unlinked.Build();

            HashSet<string> messages = new HashSet<string>();

        //Сделать уведомление если отправили старый код, чтобы новый получили
        //Thread otpCleaner = new Thread(OTPCleaner);
        //otpCleaner.Start();
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
                            //Login = "",
                            //Password = "",
                            //ApplicationId = 2685278,//kate
                            //Settings = Settings.All //берем полный доступ
                        }); //Возможно говно, авторизация от юзера (приложение)
                    }
                    catch (Exception apiAuth) { Logging.ErrorLogging(apiAuth); Logging.ReadError(); goto Restart; }
                    longPoolServerResponse = new LongPollServerResponse();
                    longPoolServerResponse = api.Groups.GetLongPollServer(groupID); //id группы
                                                                                    //var s = api.Groups.GetLongPollServer(groupID);               
                    BotsLongPollHistoryResponse poll = null;
                    try
                    {
                        poll = api.Groups.GetBotsLongPollHistory(
                           new BotsLongPollHistoryParams()
                           //{ Server = s.Server, Ts = s.Ts, Key = s.Key, Wait = 25 }); //wait=25 (default)
                           { Server = longPoolServerResponse.Server, Ts = longPoolServerResponse.Ts, Key = longPoolServerResponse.Key, Wait = 25 });
                        pts = longPoolServerResponse.Pts;
                        if (poll?.Updates == null) continue; // Проверка на новые события
                    }
                    catch (Exception ex)
                    {
                        Logging.ErrorLogging(ex);
                        Logging.ReadError();
                        //Console.WriteLine(ex);
                        goto Restart;
                    }
                    foreach (var a in poll.Updates)
                    {

                        //OLD 1 Thread VERSION

                        //Cant Execute from user

                        //if (a.Type == GroupUpdateType.MessageAllow) //Подписка на сообщения сообщества
                        //{
                        //    long? userID = a.MessageAllow.UserId;
                        //    SendMessage("&#128521; Спасибо за подписку!", userID, keyboard, null);
                        //    //break; //Так как не хотим далее исполнять код, сработал эвент - исполнили, далее другой эвент
                        //}
                        //if (a.Type == GroupUpdateType.GroupJoin) //Вступление юзера в группу
                        //{
                        //    //string userMessage = a.Message.Text.ToLower();
                        //    long? userID = a.GroupJoin.UserId;
                        //    //var markAsRead = a.Message.ReadState;
                        //    SendMessage("&#127881; Добро пожаловать в наш музыкальный паблик!\nМы разработали собственную программу для подписки на музыкальные жанры!\n Круто же!!!", userID, keyboard, null);
                        //    //break;
                        //}
                        //if (a.Type == GroupUpdateType.GroupLeave) //Выход из группы
                        //{
                        //    //string userMessage = a.Message.Text.ToLower();
                        //    long? userID = a.GroupLeave.UserId;

                        //    SendMessage("&#128532; Очень жаль(\nЗаходи к нам ещё!&#128281;", userID, keyboard, null);
                        //    //break;
                        //    //var markAsRead = a.Message.ReadState;
                        //    //Thread.Sleep(1500);
                        //}
                        PostType postType = new PostType();
                        //postType = post (не postponed и не suggested)
                        postType = PostType.Post;

                        if (a.Type == GroupUpdateType.WallPostNew)
                            if (a.WallPost.PostType == postType)
                            {
                                Uri thumbUrl = null; //Картинка для телеграма (музыки) 200 кб 90х90px
                                //long? userID = a.Message.FromId;
                                //Прогнаться по списку юзеров В БД, проверить на какие он стили подписан
                                //List < string > subGenresList= MySQLClass.GetUserSubscribtion(userID);

                                //Если сообщение в посту (хэштег соответствует стилю подписки) -> отправить пост в ЛС
                                string postMessage = a.WallPost.Text;

                                //Скачать песни из вложений - отправить в телеграм
                                var allAttachments = a.WallPost.Attachments;
                                Console.WriteLine(string.Format("Новый пост: {0}, вложений: {1} ", postMessage, allAttachments.Count));

                                List<VkNet.Model.Attachments.Audio> audios = new List<VkNet.Model.Attachments.Audio>();
                                List<string> trackIds = new List<string>();
                                //IEnumerable<string> photoCollection = null;
                                List<string> photoList2Load = new List<string>();
                                //photoList2Load = null;
                                //trackIds = null;

                                if (allAttachments.Count > 0)
                                    foreach (var attach in allAttachments)
                                    {
                                        var attid = attach.Instance.Id;
                                        var attown = attach.Instance.OwnerId;

                                        string attachFullId = attown + "_" + attid;

                                        if (attach.Type == typeof(VkNet.Model.Attachments.Audio))
                                        //decode - download
                                        {
                                            // add all audios to list
                                            trackIds.Add(attachFullId);
                                        }
                                        if (attach.Type == typeof(VkNet.Model.Attachments.Photo))
                                        {
                                            photoList2Load.Add(attachFullId);
                                        }

                                    }

                                //Отсылаем картинки
                                List<VkNet.Model.Attachments.Photo> photoList = new List<VkNet.Model.Attachments.Photo>();
                                //photoList = null;
                                if (photoList2Load.Count > 0)
                                {
                                    //services = new ServiceCollection();
                                    //services.AddAudioBypass();
                                    try
                                    {
                                        api = new VkApi();
                                        //Get JPG Link
                                        //var post = api.Wall.Get(new WallGetParams { Count = 1, OwnerId = (long?)groupID });
                                        api.Authorize(new ApiAuthParams()
                                        {
                                            //AccessToken = userToken,
                                            Login = vkLogin,
                                            Password = vkPassword,
                                            ApplicationId = kateMobileAppID,//kate
                                            Settings = Settings.All //берем полный доступ
                                        }); //Возможно говно, авторизация от юзера (приложение)
                                        Console.WriteLine("VK User auth для фото!");
                                        Thread.Sleep(1000);

                                        var photos = api.Photo.GetById(photoList2Load, null, photoSizes: true); //, photoSizes: true
                                                                                                                //Если был задан параметр photoSizes, вместо полей width и height возвращаются размеры копий фотографии в специальном формате.
                                        photoList = photos.ToList();

                                        //Photo photoClass; 
                                        //Скачивание всех фоток с поста
                                        if (photoList.Count > 0)
                                            using (WebClient webClient = new WebClient())
                                            {
                                                ////Ищем самую большую фотку,из всех картинок поста а нужно для каждой из картинок найти большую и вернуть
                                                Classes.Photo photoClass2Process = GetLargePhoto.GetLargeAndThumbUrl(photoList);
                                                List<Uri> getLargePhoto = photoClass2Process.largePhotoUris;
                                                //Также ищем превьюху
                                                thumbUrl = photoClass2Process.smallPhotoUri;

                                                //Если несколько
                                                foreach (var largePhotoLink in getLargePhoto)
                                                {
                                                    webClient.DownloadFile(largePhotoLink.AbsoluteUri.ToString(), Path.GetFileName(largePhotoLink.ToString()));
                                                    Classes.Photo photoClass = new Classes.Photo(Path.GetFileName(largePhotoLink.ToString()), postMessage);

                                                    Thread senderTGPhotoThread = new Thread(TelegramSender.SendPhoto2TG);
                                                    senderTGPhotoThread.Start(photoClass);
                                                    //var state1 = PostState.imageSended;

                                                    Thread.Sleep(100);
                                                }
                                                getLargePhoto.Clear();
                                            }
                                    } catch (Exception sendPhotoEx) { Logging.ErrorLogging(sendPhotoEx); Logging.ReadError(); goto SendMusic; }
                                }
                                SendMusic:
                                //Отсылаем музыку
                                if (trackIds.Count > 0)
                                {
                                    var services = new ServiceCollection();
                                    services.AddAudioBypass();

                                    api = new VkApi(services);
                                    //services = new ServiceCollection();
                                    //services.AddAudioBypass();

                                    //api = new VkApi(services);

                                    //var ids = new string[] { attachFullId };
                                    try
                                    {
                                        api.Authorize(new ApiAuthParams()
                                        {
                                            //AccessToken = kateMobileToken,
                                            Login = vkLogin,
                                            Password = vkPassword,
                                            ApplicationId = kateMobileAppID,//kate
                                            Settings = Settings.All //берем полный доступ
                                        }); //Возможно говно, авторизация от юзера (приложение)
                                        Console.WriteLine("VK User auth для музыки!");
                                    }
                                    catch (Exception apiEx) { Logging.ErrorLogging(apiEx); Logging.ReadError(); goto Restart;  } //mb goto restart = reauth
                                    
                                    audios = api.Audio.GetById(trackIds).ToList();
                                    //После успешного выполнения возвращает массив объектов audio. Обратите внимание, что ссылки на аудиозаписи привязаны к ip адресу.


                                    foreach (var song in audios)
                                    {
                                        //song.Url

                                        Uri audioUri = song.Url; //get URI

                                        //Pass to fileName.mp3

                                        //song.Artist;
                                        //song.FeaturedArtists;
                                        //song.Title;

                                        if (audioUri != null) //Если запись не по подписке или не запрещена
                                        {
                                            //var decoder = new AudioDownloader();

                                            Uri mp3link = AudioDownloader.DecodeAudioUrl(audioUri); //https://cs1-40v4.vkuseraudio.net/p11/1cdc782e4ea9ae.mp3?extra=CdZ7cuD7ct6LnwZL-GBcZRgaZplPIU8bmtXTedGTFsuwP-CA12bttBAe9xw9CV20IhIvKY-3-g15QuvMvyRXOiqeENaQDp5TJb_8r0jSLrzON1JoE80bb8uFLpR6OXe62Ex6Kjed8_107NxydUDYio_Tlw
                                                                                                    //
                                            var uriString = mp3link.ToString();
                                            string mp3Direct = mp3link.AbsoluteUri; //Одно и то же что и выше
                                                                                    //Должны убрать говно из адреса, все что дальше .mp3, после ?extra=
                                                                                    //https://cs1-40v4.vkuseraudio.net/p11/1cdc782e4ea9ae.mp3 -Чистый
                                            int divider = mp3Direct.IndexOf('?');
                                            string clearLink = mp3Direct.Substring(0, divider);

                                            string mp3Title = song.Title;
                                            string mp3Artist = song.Artist;
                                            // Create two different encodings.
                                            //Encoding ascii = Encoding.ASCII;
                                            //Encoding unicode = Encoding.Unicode;
                                            

                                            //if(song.FeaturedArtists!=null)
                                            //foreach (var artist in song.FeaturedArtists)
                                            //{
                                            //    mp3Artist += " " + artist.Name;
                                            //}
                                            //if(song.MainArtists!=null)
                                            //foreach (var mainartist in song.MainArtists)
                                            //{
                                            //    mp3Artist += " " + mainartist.Name;
                                            //}
                                            string mp3name = mp3Title + " - " + mp3Artist + ".mp3";
                                            // Convert the string into a byte array.
                                            //byte[] asciiBytes = ascii.GetBytes(mp3name);

                                            //Что делать если русский язык? Р’РµС‰РµСЃС‚РІР° - Dik Key.mp3

                                            //song.TrackGenre
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
                                                //Thread senderTGThread = new Thread(TelegramSender.SendMusic2TG);
                                                senderTGThread.Start(mP);
                                                //senderTGThread.Start(clearLink);
                                                //TelegramSender.SendMusic2TG(clearLink);
                                                Thread.Sleep(500);
                                            }
                                            else Console.WriteLine("Слишком длинный трек >12 минут");
                                        }
                                        else continue;
                                    }

                                    //try
                                    //{
                                    //    ///Ждем завершения потока (бесполезно, так как асихнронный метод, хоть и ожидаемый)
                                    //    //senderTGThread.Join();
                                    //    ///Отправляем опрос
                                    //    IEnumerable<string> answrsEnum = new string[] { new string("+ Качает"), new string("- Шлак") };
                                    //    TGPoll tp = new TGPoll(string.Format("Оцени подборку {0}: ", postMessage), answrsEnum);

                                    //    Thread sendPoll = new Thread(TelegramSender.SendPoll2TG);
                                    //    sendPoll.Start(tp);
                                    //}
                                    //catch (Exception pollEx) { Console.WriteLine(pollEx); }

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

                        /*                        if (a.Type == GroupUpdateType.MessageNew)
                                                {
                                                    string userMessage = a.Message.Text.ToLower();
                                                    long? userID = a.Message.FromId;
                                                    string payload = a.Message.Payload;
                                                    long? chatID = a.Message.ChatId;
                                                    long? msgID = a.Message.Id;
                                                    var markAsRead = a.Message.ReadState;
                                                    //a.Message.ReadState = VkNet.Enums.MessageReadState.Readed;
                                                    Console.WriteLine(userMessage);
                                                    // извлекает первый при сообщении, а нужно все получить, прогнаться по всем
                                                    Program P = new Program();

                                                    //Обработка  входящих сообщений
                                                    if (payload != null)//если пришло нажатие кнопки
                                                    {
                                                        //В зависимости от того какая кнопка нажата, отправляем новое меню - Если нажали првязать и успешно привязано, то удаляем кнопку"Привязать" ?! Надо ли

                                                       // bool isLinked = MySQLClass.IsVKLinked(userID);

                                                        // АНТИСПАМ: Нажали кнопку - ждем, чтобы постоянно не тыкали, сделать таймер повторной отправки для конкретного юзера

                                                        switch (userMessage)
                                                        {
                                                            case "подписка": 
                                                                //Отправляем


                                                                if (isLinked == true)
                                                                {
                                                                    SendMessage("&#8252; Вы уже привязали аккаунт!", userID, keyboard_linked, payload);
                                                                    //Таймер действия - если в течение 10 минут не произошло действие, сбросить меню
                                                                }
                                                                else { SendMessage("&#8987; Напиши (выбери) жанры для подписки :", userID, keyboard_genres_general, payload);



                                                                }
                                                                //Thread.Sleep(500);
                                                                break;
                                                            case "отписка":
                                                                //Если учетка не был привязана, проверить, если да - то проверяем

                                                                if (isLinked == true)
                                                                {
                                                                    bool isSuccess2 = MySQLClass.RemoveVKID(userID);
                                                                    if (isSuccess2 == true)
                                                                    { //Успешно удалили привязку
                                                                        SendMessage("&#10060; Учетная запись отвязана", userID, keyboard_unlinked, payload);
                                                                        //string unlink = "+" + playerName;
                                                                        //TCPOtpSender.AddToOtpAndNameQueue(successfull);
                                                                    }
                                                                    else
                                                                    {
                                                                        SendMessage("&#8252; Ошибка удаления привязки", userID, keyboard_linked, payload);
                                                                    }

                                                                }
                                                                else
                                                                { //Учетная запись не привязана - ничего не делать, отправить клавиатуру с привязкой
                                                                    SendMessage("&#9888; Учетная запись ещё не привязана.", userID, keyboard_unlinked, null);
                                                                    break;
                                                                }
                                                            case "house":
                                                                {
                                                                    //Добавить (обновить в БД)
                                                                    MySQLClass.SetUserSubscription(userID, "house");

                                                                }

                                                                //Thread.Sleep(500);
                                                                break;

                                                                //case "привет":
                                                                //SendMessage("Здарова!", userID, keyboard, payload);
                                                                //    break;

                                                                //default:

                                                                //    break;
                                                        }
                                                    }
                                                    else //Если кнопку не нажимали, написали любое сообщение, отправляем клавиатуру?!
                                                    {
                                                        //сделать таймер повторной отправки для конкретного юзера = АНТИСПАМ
                                                        // Как вариант запоминать предыдущее сообщение, сравнить с новым, если оно повторяется - АТАТА
                                                        //var lastMsgId = a.Message.OutRead;

                                                        switch (userMessage)
                                                        {
                                                            //case "привет":
                                                            //    SendMessage("Здарова!", userID, keyboard, null);
                                                            //    break;
                                                            case "начать":
                                                                SendMessage("Приступим, держи меню-клавиатуру!", userID, keyboard, null);
                                                                break;
                                                            case "Начать":
                                                                SendMessage("Приступим, держи меню-клавиатуру!", userID, keyboard, null);
                                                                break;
                                                                //case "старт":
                                                                //    SendMessage("Приступим, держи меню-клавиатуру!", userID, keyboard, null);
                                                                //    break;
                                                                //case "help":
                                                                //    SendMessage("Ты можешь привязать аккаунт в Майнкрафте к ВК или удалить привязку.\nБот умеет читать игровой чат", userID, keyboard, null);
                                                                //    break;
                                                                //case "помощь":
                                                                //    SendMessage("Ты можешь привязать аккаунт или удалить привязку", userID, keyboard, null);
                                                                //    break;
                                                        }


                                                        //SendMessage("&#128224; Ваше меню, сэр/мэм!", userID, keyboard, null); 
                                                    } //Отправляем клавиатуру


                                                }*/
                        //Thread.Sleep(1500);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorLogging(ex);
                Logging.ReadError();
                goto Start;
                //Console.WriteLine(ex);
            }
        }

      
    }
}
