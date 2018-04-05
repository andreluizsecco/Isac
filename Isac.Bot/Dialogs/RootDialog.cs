using Isac.Bot.Common;
using Isac.Bot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Isac.Bot.Dialogs
{
    [LuisModel("{ModelID}", "{SubscriptionKey}")]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        public RootDialog() { }

        [LuisIntent("intent.conversation.greet")]
        public async Task Greet(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Olá, tudo bem?");

            var cardActions = new List<CardAction>();

            cardActions.Add(new CardAction()
            {
                Title = $"Ligar a Lâmpada",
                Type = ActionTypes.ImBack,
                Value = $"Ligar a Lâmpada"
            });

            cardActions.Add(new CardAction()
            {
                Title = $"Desligar a Lâmpada",
                Type = ActionTypes.ImBack,
                Value = $"Desligar a Lâmpada"
            });

            cardActions.Add(new CardAction()
            {
                Title = $"Exibir Temperatura",
                Type = ActionTypes.ImBack,
                Value = $"Temperatura"
            });

            cardActions.Add(new CardAction()
            {
                Title = $"Exibir Umidade",
                Type = ActionTypes.ImBack,
                Value = $"Umidade"
            });

            var card = new HeroCard()
            {
                Title = "Algumas ações disponíveis para você!",
                Buttons = cardActions
            };

            var activity = context.MakeMessage();
            activity.Id = new Random().Next().ToString();
            activity.Attachments.Add(card.ToAttachment());

            await context.PostAsync(activity);
            context.Wait(MessageReceived);
        }

        [LuisIntent("intent.conversation.feeling")]
        public async Task Feeling(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Estou muito bem, obrigado por perguntar!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("intent.conversation.self_happy")]
        public async Task SelfHappy(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Sim claro, sou feliz no que eu faço.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("intent.conversation.self_likeMe")]
        public async Task SelfLikeMe(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"É...digamos que eu gosto de conversar com você.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("intent.conversation.self_age")]
        public async Task SelfAge(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Não tenho idade como os humanos, mas se quiser saber, estou na versão BETA");
            context.Wait(MessageReceived);
        }

        [LuisIntent("intent.conversation.self_createdBy")]
        public async Task SelfCreatedBy(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Fui criado pelo André Secco, para facilitar a vida das pessoas! Quer saber mais sobre ele? Veja esse link: http://andresecco.com.br");
            context.Wait(MessageReceived);
        }

        [LuisIntent("intent.conversation.self_son")]
        public async Task SelfSon(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Não tenho! Sou um software, que por sinal não tem uma alma gêmea :(");
            context.Wait(MessageReceived);
        }

        [LuisIntent("intent.conversation.self_aboutMe")]
        public async Task SelfAboutMe(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Meu nome é Isac (Intelligent System for All Concerns) e fui criado para ser um assistente virtual inteligente para todas as suas necessidades!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("intent.communication.publish_message")]
        public async Task PublishMessage(IDialogContext context, LuisResult result)
        {
            var message = string.Empty;
            result.Entities.Where(x => x.Type.Equals("communication.message")).ToList().ForEach(m => message += m.Entity + " ");
            message = message.Replace("\"", "");
            if (!string.IsNullOrEmpty(message))
            {
                new Twitter().PublishTweet(message);
                await context.PostAsync($"Ok! Já publiquei o tweet que pediu.");
            }
            else
                await context.PostAsync($"Não entendi o conteúdo para fazer o tweet, tente me falar novamente.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("intent.iot.get_temperature")]
        public async Task GetTemperature(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"A temperatura atual é de " + DHT11.GetInfByName("temperature"));
            context.Wait(MessageReceived);
        }

        [LuisIntent("intent.iot.get_humidity")]
        public async Task GetHumidity(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"A umidade atual é de " + DHT11.GetInfByName("humidity"));
            context.Wait(MessageReceived);
        }

        [LuisIntent("intent.iot.device_on")]
        public async Task DeviceOn(IDialogContext context, LuisResult result)
        {
            var iot_device_name = result.Entities.FirstOrDefault(x => x.Type.Equals("iot.device_name"))?.Entity;
            if (!string.IsNullOrEmpty(iot_device_name))
            {
                if (Functions.RemoveDiacritics(iot_device_name.ToLower()).Contains("lamp") || iot_device_name.ToLower().Contains("luz"))
                {
                    new IoTHub().DeviceControl(true, iot_device_name);
                    await context.PostAsync($"Ok! Liguei a lâmpada!");
                }
                else
                    await context.PostAsync($"Não existe nenhum dispositivo '{iot_device_name}' conectado a sua central de dispositivos.");
            }
            else
                await context.PostAsync("Não entendi qual é o dispositivo que deseja ligar, tente me falar novamente.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("intent.iot.device_off")]
        public async Task DeviceOff(IDialogContext context, LuisResult result)
        {
            var iot_device_name = result.Entities.FirstOrDefault(x => x.Type.Equals("iot.device_name"))?.Entity;
            if (!string.IsNullOrEmpty(iot_device_name))
            {
                if (Functions.RemoveDiacritics(iot_device_name.ToLower()).Contains("lamp") || iot_device_name.ToLower().Contains("luz"))
                {
                    new IoTHub().DeviceControl(false, iot_device_name);
                    await context.PostAsync($"Ok! Desliguei a lâmpada!");
                }
                else
                    await context.PostAsync($"Não existe nenhum dispositivo '{iot_device_name}' conectado a sua central de dispositivos.");
            }
            else
                await context.PostAsync("Não entendi qual é o dispositivo que deseja desligar, tente me falar novamente.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Desculpe! Não entendi o que deseja, pode explicar melhor?");
            context.Wait(MessageReceived);
        }
    }
}