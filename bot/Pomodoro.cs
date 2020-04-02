using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace bot
{
    public static class Pomodoro
    {
        private static float workTime;
        private static float shortBreak;
        private static float longBreak;
        public static bool IsWorking;
        private static int i;


        public static int ReturnTimerStatus()
        {
            return i;
        }


        public static void Timer(CallbackQuery callbackQuery, ITelegramBotClient botClient)
        {
            var fullTomato = 120;
            var twentyFive = 25;
            var five = 5;
            var endBreakPoint = 0;

            Console.WriteLine(IsWorking);


            for (i = 1; i <= fullTomato; i++)
            {
                Thread.Sleep(1000);


                if (i % twentyFive == 0 && IsWorking)
                {
                    botClient.SendTextMessageAsync(callbackQuery.From.Id, $"Прошло {i} минут, теперь перерыв 5 минут");
                    endBreakPoint = i + 5;
                }

                if (i == endBreakPoint && IsWorking)
                {
                    botClient.SendTextMessageAsync(callbackQuery.From.Id, "Перерыв закончен");
                }

                if (IsWorking == false)
                {
                    Console.WriteLine("Помидор остановлен");
                    break;
                }

                Console.WriteLine("tik" + i);
            }
        }
    }
}