using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace hvmbot
{

    public static class Configuration
    {
		//Telegram
        public readonly static string AccessToken = "";
        public static readonly ChatId tg_chat_id = ;
        public readonly static string BotToken = "";

//#if USE_PROXY

        public static class Proxy

        {

            public readonly static string Host = "127.0.0.1";

            public readonly static int Port = 9050;

        }

//#endif
        public static class vkAuth
        {
            public readonly static string vkLogin  = "";
            public readonly static string vkPassword = "";
            public readonly static ulong kateMobileAppID = 2685278;
            //Получается https://oauth.vk.com/authorize?client_id=ID&scope=notify,wall,groups,messages,notifications,offline&redirect_uri=http://api.vk.com/blank.html&display=page&response_type=token                                                    
           //NOT NEEDED
            public readonly static string groupToken = ""; //new acc + new app (для работы автоответчика - чат бота группы)
            public readonly static string userToken = ""; //New acc + new app
            public readonly static string kateMobileToken = "";
            //NOT NEEDED

            //Получуется на vknet.github.io
            public readonly static string vkapiToken = ""; //Для работы обновлений стены
            public readonly static ulong groupID = ;
        }

        public static class dbAuth
        {
            public readonly static string host = "";
            public readonly static int port = 3306;
            public readonly static string database = "";
            public readonly static string username = "";
            public readonly static string password = "";
        }

    }
}
