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

            //---------------DEBUG-------------------
            Thread vkLPL = new Thread(LongPollHandler.LongPollListener);
            vkLPL.Start();

            //Thread runTG = new Thread(RunTg);
            //runTG.Start();
            //---------------DEBUG-------------------

            ////Docker can't Poll WHY?
            //Thread echoBot = new Thread(RunEchoBot);
            //echoBot.Start();
        }

        static async void RunEchoBot()
        {
            await EchoBot.Main();
        }
        static async void RunTg()
        {
            //await TelegramMusic.SendTextAsync("TestText");
            await TelegramMusic.SendAudioFromFileAsync("music.mp3");
            //await TelegramMusic.SendPhotoAsync( "https://.jpg", "");
            //await TelegramMusic.SendAudioAsync("https://github.com/TelegramBots/book/raw/master/src/docs/audio-guitar.mp3");
        }

    }
}
