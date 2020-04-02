using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace bot
{
    public static class Firebase
    {
        private static InlineKeyboardMarkup inlineKeyboard;

        static IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "O8LicnESXA2r6qeQ0Cv7DfX8ckNtjaYiWJeSLrm5",
            BasePath = "https://demosharp-3ae49.firebaseio.com/"
        };

        static IFirebaseClient client = new FirebaseClient(config);


        public static async Task DataAdder(int id, string name, string text)
        {
            var creationTime = DateTime.Now.Ticks;


            var data = new Data
            {
                Name = name,
                Text = text,
                Id = creationTime
            };

            var message = await client.SetTaskAsync($"{id}/{creationTime}", data);
            Console.WriteLine("Data has been added");
        }

        public static async Task DataSender(CallbackQuery callbackQuery, ITelegramBotClient botClient, bool onEdit)
        {
            var id = callbackQuery.From.Id;


            var firebaseResponse = await client.GetTaskAsync(id.ToString());

            if (firebaseResponse.Body != "null")
            {
                var newObj = JObject.Parse(firebaseResponse.Body);


                var list = newObj.Values();
                var backButtonRow = new List<InlineKeyboardButton>();


                var ikbList = new List<InlineKeyboardButton[]>();

                var builder = new StringBuilder();


                foreach (var t in list)
                {
                    var buttonsList = new List<InlineKeyboardButton>();

                    buttonsList.Add(new InlineKeyboardButton
                    {
                        CallbackData = t["Id"].ToString(),
                        Text = t["Text"].ToString()
                    });

                    ikbList.Add(buttonsList.ToArray());
                    builder.Append(t["Text"] + "\n");
                }

                backButtonRow.Add(new InlineKeyboardButton
                {
                    CallbackData = "Домой",
                    Text = "Домой"
                });
                ikbList.Add(backButtonRow.ToArray());

                switch (onEdit)
                {
                    case false:
                        inlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("☰"),
                                InlineKeyboardButton.WithCallbackData("Домой")
                            }
                        });


                        await botClient.EditMessageTextAsync(id.ToString(), callbackQuery.Message.MessageId,
                            "Вот список ваших заметок \n" + builder);

                        await botClient.EditMessageReplyMarkupAsync(callbackQuery.From.Id,
                            callbackQuery.Message.MessageId,
                            replyMarkup: inlineKeyboard);


                        break;

                    case true:

                        inlineKeyboard = new InlineKeyboardMarkup(ikbList);

                        await botClient.EditMessageTextAsync(id.ToString(), callbackQuery.Message.MessageId,
                            "Для удаления нажмите на заметку");

                        await botClient.EditMessageReplyMarkupAsync(callbackQuery.From.Id,
                            callbackQuery.Message.MessageId,
                            replyMarkup: inlineKeyboard);

                        break;
                }
            }
            else
            {
                var noNotesScreenInlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Добавить заметку")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Домой")
                    }
                });
                await botClient.EditMessageTextAsync(id.ToString(), callbackQuery.Message.MessageId,
                    "Заметок еще нет......");
                await botClient.EditMessageReplyMarkupAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    replyMarkup: noNotesScreenInlineKeyboard);
            }
        }

        public static async Task DataRemover(CallbackQuery callbackQuery)
        {
            await client.DeleteTaskAsync(callbackQuery.From.Id + "/" + callbackQuery.Data);
            Console.WriteLine("Data has been deleted");
        }
    }
}