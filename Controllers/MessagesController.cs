﻿using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using VAMInsuranceBot.Dialog.Dialogs;

namespace VAMInsuranceBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>

        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
               message.SetBotConversationData("LastMessageDateTime", DateTime.UtcNow);
               return await Conversation.SendAsync(message, () => new DefaultDialog());
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
                //message.Type = "Ping";
                Message reply = message.CreateReplyMessage();
                reply.Text = "Goodbye!";

                return reply;
                
            }

            return null;
        }
    }
}