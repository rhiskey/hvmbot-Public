using hvmbot.TelegramPart;
using hvmbot.VKPart;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using VkNet;

namespace hvmbot
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread vkLPL = new Thread(LongPollHandler.LongPollListener);
            vkLPL.Start();
        }

        static async void RunEchoBot()
        {
            await EchoBot.Main();
        }
        static async void RunTg()
        {
            await TelegramMusic.SendAudioFromFileAsync("music.mp3");

        }

    }
}
