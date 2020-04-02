using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using ApiAiSDK;


namespace bot
{
    class Program
    {
        private static ITelegramBotClient botClient;
        private static ApiAi apiAi;

        private static bool
            isAddingMode =
                false; // Если включен то добавялем сообщение в заметку , если выключен не реагируем нп событие получение сообщения 

        private static InlineKeyboardMarkup startInlineKeyboard;
        private static InlineKeyboardMarkup backlInlineKeyboard;
        private static InlineKeyboardMarkup pomadoroInlineKeyboard;
        private static string WelcomeText;


        static void Main(string[] args)
        {
            //AIConfiguration config = new AIConfiguration("fdca237ae6ce426a96ea3156a992feb0", SupportedLanguage.Russian);
            //apiAi = new ApiAi(config);

            startInlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Показать  список заметок")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Добавить заметку"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Помидор"),
                        InlineKeyboardButton.WithCallbackData("Что нового?")
                    }
                }
            );
            backlInlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Отменить")
                }
            });

            pomadoroInlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Домой"),
                InlineKeyboardButton.WithCallbackData("Сбросить помидор"),
                InlineKeyboardButton.WithCallbackData("Состояние помидора"),
                InlineKeyboardButton.WithCallbackData("Установить помидор"),
            });

            WelcomeText = "Привет! Что ты хочешь сделать?     ";


            botClient = new TelegramBotClient("1058524291:AAGBQT7FFQj1QqRV7AKpRqc5aexe4yyDSzw")
                {Timeout = TimeSpan.FromSeconds(5)};

            botClient.OnMessage += Bot_OnMessage;
            botClient.OnCallbackQuery += OnCallBackQueryReceived;

            var me = botClient.GetMeAsync().Result;

            Console.WriteLine($"Бот {me.Username} запустился.");

            botClient.StartReceiving();
            Console.ReadKey();
            botClient.StopReceiving();
        }


        private static async void OnCallBackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            var buttonText = e.CallbackQuery.Data;
            var name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            var id = e.CallbackQuery.From.Id;
            try
            {
                // await botClient.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"{name} нажал {buttonText}");
            }
            catch (Telegram.Bot.Exceptions.InvalidParameterException exception)
            {
                Console.WriteLine(exception);
                throw;
            }

            var onEdit = false; //Следит за режимом инлайн клавиатуры для редактирования заметки 

            switch (buttonText)
            {
                case "Добавить заметку":
                    isAddingMode = true; //Говорит о том что следущее сообщение довавить в бд

                    await botClient.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId,
                        "Ок, сейчас отправь заметку, которую хочешь сохранить      ");
                    await botClient.EditMessageReplyMarkupAsync(e.CallbackQuery.From.Id,
                        e.CallbackQuery.Message.MessageId, replyMarkup: backlInlineKeyboard);
                    break;

                case "Отменить":
                    await botClient.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId,
                        WelcomeText);
                    await botClient.EditMessageReplyMarkupAsync(e.CallbackQuery.From.Id,
                        e.CallbackQuery.Message.MessageId, replyMarkup: startInlineKeyboard);
                    break;

                case "Домой":
                    await botClient.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId,
                        WelcomeText);
                    await botClient.EditMessageReplyMarkupAsync(e.CallbackQuery.From.Id,
                        e.CallbackQuery.Message.MessageId, replyMarkup: startInlineKeyboard);
                    break;

                case "Что нового?":

                    var res = $"{await Weather.WeatherSender()}\n{await News.Currency()}";

                    await botClient.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId,
                        res);
                    await botClient.EditMessageReplyMarkupAsync(e.CallbackQuery.From.Id,
                        e.CallbackQuery.Message.MessageId, replyMarkup: backlInlineKeyboard);
                    break;
                case "Показать  список заметок":
                    await Firebase.DataSender(e.CallbackQuery, botClient, onEdit);
                    break;
                case "☰":
                    onEdit = true;
                    await Firebase.DataSender(e.CallbackQuery, botClient, onEdit);
                    break;
                case "Установить помидор":


                    await botClient.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId,
                        "Установлен помидор на 2 часа ");
                    await botClient.EditMessageReplyMarkupAsync(e.CallbackQuery.From.Id,
                        e.CallbackQuery.Message.MessageId, replyMarkup: pomadoroInlineKeyboard);


                    Pomodoro.IsWorking = true;
                    Pomodoro.Timer(e.CallbackQuery, botClient);
                    break;

                case "Сбросить помидор":
                    Pomodoro.IsWorking = false;
                    Pomodoro.Timer(e.CallbackQuery, botClient);

                    await botClient.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Помидор сброшен");
                    await botClient.SendTextMessageAsync(e.CallbackQuery.From.Id,
                        WelcomeText, replyMarkup: startInlineKeyboard);
                    break;
                case "Состояние помидора":

                    await botClient.SendTextMessageAsync(e.CallbackQuery.From.Id,
                        $"Прошло {Pomodoro.ReturnTimerStatus()} минут");
                    break;
                case "Помидор":


                    await botClient.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId,
                        "Выберите опцию");
                    await botClient.EditMessageReplyMarkupAsync(e.CallbackQuery.From.Id,
                        e.CallbackQuery.Message.MessageId, replyMarkup: pomadoroInlineKeyboard);

                    break;
                default:
                    await Firebase.DataRemover(e.CallbackQuery);
                    onEdit = true;
                    await Firebase.DataSender(e.CallbackQuery, botClient, onEdit);
                    break;
            }
        }


        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message == null && message.Type != MessageType.Text)
            {
                return;
            }

            var name = $"{message.From.FirstName} {message.From.LastName}"; //Users name
            var id = message.From.Id;

            switch (message.Text)
            {
                case "/start":
                    await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText,
                        replyMarkup: startInlineKeyboard);

                    break;

                default:

                    var noteAddedInlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Домой")
                        }
                    });

                    if (isAddingMode && message.Text != null)
                    {
                        await Firebase.DataAdder(id, name, message.Text);

                        await botClient.SendTextMessageAsync(message.Chat.Id, "Ваша заметка добавлена!",
                            replyMarkup: noteAddedInlineKeyboard);
                        isAddingMode = false; //Говорит о что том что заметкa добавлена и мы не слушаем сообщения  
                    }

                    break;
            }
        }
    }
}