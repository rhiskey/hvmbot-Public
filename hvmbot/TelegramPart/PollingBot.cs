﻿using MihaZupan;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;



namespace hvmbot.TelegramPart
{
    class PollingBot
    {
        private static TelegramBotClient Bot;



        public static async Task Main()

        {

            //#if USE_PROXY

            //var Proxy = new WebProxy(Configuration.Proxy.Host, Configuration.Proxy.Port) { UseDefaultCredentials = true };

            //Bot = new TelegramBotClient(Configuration.BotToken, webProxy: Proxy);

            var SocksProxy = new HttpToSocks5Proxy(Configuration.Proxy.Host, Configuration.Proxy.Port);

            Bot = new TelegramBotClient(Configuration.BotToken, SocksProxy);

            //#else

            //            Bot = new TelegramBotClient(Configuration.BotToken);

            //#endif



            var me = await Bot.GetMeAsync();

            Console.Title = me.Username;



            var cts = new CancellationTokenSource();



            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.

            //Bot.StartReceiving(

            //    new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync),

            //    cts.Token

            //);



            Console.WriteLine($"Start listening for @{me.Username}");

            Console.ReadLine();



            // Send cancellation request to stop bot

            cts.Cancel();

        }



        public static async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)

        {

            var handler = update.Type switch

            {

                UpdateType.Message => BotOnMessageReceived(update.Message),

                UpdateType.EditedMessage => BotOnMessageReceived(update.Message),

                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery),

                UpdateType.InlineQuery => BotOnInlineQueryReceived(update.InlineQuery),

                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult),

                // UpdateType.Unknown:

                // UpdateType.ChannelPost:

                // UpdateType.EditedChannelPost:

                // UpdateType.ShippingQuery:

                // UpdateType.PreCheckoutQuery:

                // UpdateType.Poll:

                _ => UnknownUpdateHandlerAsync(update)

            };



            try

            {

                await handler;

            }

            catch (Exception exception)

            {

                await HandleErrorAsync(exception, cancellationToken);

            }

        }



        private static async Task BotOnMessageReceived(Message message)

        {

            Console.WriteLine($"Receive message type: {message.Type}");

            if (message.Type != MessageType.Text)

                return;



            var action = (message.Text.Split(' ').First()) switch

            {

                "/inline" => SendInlineKeyboard(message),

                "/keyboard" => SendReplyKeyboard(message),

                "/photo" => SendFile(message),

                "/request" => RequestContactAndLocation(message),

                _ => Usage(message)

            };

            await action;



            // Send inline keyboard

            // You can process responses in BotOnCallbackQueryReceived handler

            static async Task SendInlineKeyboard(Message message)

            {

                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);



                // Simulate longer running task

                await Task.Delay(500);



                var inlineKeyboard = new InlineKeyboardMarkup(new[]

                {

                    // first row

                    new []

                    {

                        InlineKeyboardButton.WithCallbackData("1.1", "11"),

                        InlineKeyboardButton.WithCallbackData("1.2", "12"),

                    },

                    // second row

                    new []

                    {

                        InlineKeyboardButton.WithCallbackData("2.1", "21"),

                        InlineKeyboardButton.WithCallbackData("2.2", "22"),

                    }

                });

                await Bot.SendTextMessageAsync(

                    chatId: message.Chat.Id,

                    text: "Choose",

                    replyMarkup: inlineKeyboard

                );

            }



            static async Task SendReplyKeyboard(Message message)

            {

                var replyKeyboardMarkup = new ReplyKeyboardMarkup(

                    new KeyboardButton[][]

                    {

                        new KeyboardButton[] { "1.1", "1.2" },

                        new KeyboardButton[] { "2.1", "2.2" },

                    },

                    resizeKeyboard: true

                );



                await Bot.SendTextMessageAsync(

                    chatId: message.Chat.Id,

                    text: "Choose",

                    replyMarkup: replyKeyboardMarkup



                );

            }



            static async Task SendFile(Message message)

            {

                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);



                const string filePath = @"Files/tux.png";

                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

                await Bot.SendPhotoAsync(

                    chatId: message.Chat.Id,

                    photo: new InputOnlineFile(fileStream, fileName),

                    caption: "Nice Picture"

                );

            }



            static async Task RequestContactAndLocation(Message message)

            {

                var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]

                {

                    KeyboardButton.WithRequestLocation("Location"),

                    KeyboardButton.WithRequestContact("Contact"),

                });

                await Bot.SendTextMessageAsync(

                    chatId: message.Chat.Id,

                    text: "Who or Where are you?",

                    replyMarkup: RequestReplyKeyboard

                );

            }



            static async Task Usage(Message message)

            {

                const string usage = "Usage:\n" +

                                        "/inline   - send inline keyboard\n" +

                                        "/keyboard - send custom keyboard\n" +

                                        "/photo    - send a photo\n" +

                                        "/request  - request location or contact";

                await Bot.SendTextMessageAsync(

                    chatId: message.Chat.Id,

                    text: usage,

                    replyMarkup: new ReplyKeyboardRemove()

                );

            }

        }



        // Process Inline Keyboard callback data

        private static async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)

        {

            await Bot.AnswerCallbackQueryAsync(

                callbackQuery.Id,

                $"Received {callbackQuery.Data}"

            );



            await Bot.SendTextMessageAsync(

                callbackQuery.Message.Chat.Id,

                $"Received {callbackQuery.Data}"

            );

        }



        #region Inline Mode



        private static async Task BotOnInlineQueryReceived(InlineQuery inlineQuery)

        {

            Console.WriteLine($"Received inline query from: {inlineQuery.From.Id}");



            InlineQueryResultBase[] results = {

                // displayed result

                new InlineQueryResultArticle(

                    id: "3",

                    title: "TgBots",

                    inputMessageContent: new InputTextMessageContent(

                        "hello"

                    )

                )

            };



            await Bot.AnswerInlineQueryAsync(

                inlineQuery.Id,

                results,

                isPersonal: true,

                cacheTime: 0

            );

        }



        private static async Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult)

        {

            Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");

        }



        #endregion



        private static async Task UnknownUpdateHandlerAsync(Update update)

        {

            Console.WriteLine($"Unknown update type: {update.Type}");

        }



        public static async Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)

        {

            var ErrorMessage = exception switch

            {

                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",

                _ => exception.ToString()

            };



            Console.WriteLine(ErrorMessage);

        }
    }
}
