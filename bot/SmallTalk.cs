using System;
using ApiAiSDK;
using ApiAiSDK.Model;

namespace bot
{
    public class SmallTalk
    {
        private static ApiAi apiAi;

        public string Talk(string message)
        {
            AIConfiguration config = new AIConfiguration("fdca237ae6ce426a96ea3156a992feb0", SupportedLanguage.Russian);
            var responce = apiAi.TextRequest(message);
            string answer = responce.Result.Fulfillment.Speech;
            if (answer == "")
            {
                answer = "Я тебя не понял";
            }

            return answer;
        }
    }
}