using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RealTimeChatWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatWebApp.Hubs
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        static List<UserChatModel> ConnectedUsersList = new List<UserChatModel>();
        static List<MessagesChatModel> SendMessagesList = new List<MessagesChatModel>();
        //static MessagesChatModel SendMessages = new MessagesChatModel();
        static List<MessagesChatModel> CacheChatMessagesList = new List<MessagesChatModel>();

        /// <summary>
        /// realiza el envio de mensajes escritos por cada usuario a la ventana del chat
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void Send(string msgFrom, string message, string userEmail, string groupName)
        {
            Clients.Group(groupName).receiveMessage(msgFrom, message, "");
            AddMessageinCache(groupName, message, userEmail, msgFrom); //almacena los datos de la conversacion en el cache global de la misma
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        public void IsTyping(string html, string groupName)
        {
            SayWhoIsTyping(html, groupName); //call the function to send the html to the other clients
        }

        /// <summary>
        /// Invokes the method to show who is typing in the chat room
        /// </summary>
        /// <param name="html"></param>
        public void SayWhoIsTyping(string html, string groupName)
        {
            Clients.OthersInGroup(groupName).sayWhoIsTyping(html);
        }

        #region "Chat Connection"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        [HubMethodName("InitConnect")]
        public void Connect(UserChatModel userInfo)
        {
            var id = Context.ConnectionId;

            StringBuilder welcomeMsg = new StringBuilder();
            string newUserMsg = null;
            string count = "NA";

            userInfo._UserId = id;

            Groups.Add(id, userInfo._GroupNameChat); //agrega el usuario a un grupo privado de chat

            if (userInfo._UserType == UserChatModel.RegisteredUserType.regularUser)
            {
                welcomeMsg.AppendLine("Bienvenido a la plataforma de servicio al cliente del BCCR");
                welcomeMsg.AppendLine("En un momento será atendido por uno de nuestros representantes ...");

                newUserMsg = "El usuario " + userInfo._UserFullName + " se ha unido al chat.";

                StringBuilder userDetails = new StringBuilder();
                userDetails.AppendLine("Información del usuario conectado al chat");
                userDetails.AppendLine(String.Format("Nombre: {0}", userInfo._UserFullName));
                userDetails.AppendLine(String.Format("Correo Electrónico: {0}", userInfo._UserEmail));
                userDetails.AppendLine(String.Format("No. Teléfono: {0}", userInfo._UserPhone));
                userDetails.AppendLine(String.Format("Pregunta: {0}", userInfo._UserQuestion));

                AddSystemMessageinCache(userInfo._GroupNameChat, userDetails.ToString()); //almacena en el cache del chat, la informacion del cliente conectado

                Clients.Caller.receiveMessage("", welcomeMsg.ToString(), "");
            }
            else
            {

                newUserMsg = "Está hablando con " + userInfo._UserFullName + ".";

                //MessagesChatModel SendMessages = SendMessagesList.FirstOrDefault(x => x._ChatGroupName == userInfo._GroupNameChat);

                StringBuilder userChatDetails = new StringBuilder();

                //foreach (var item in SendMessagesList.FirstOrDefault(x => x._ChatGroupName == userInfo._GroupNameChat)._ChatConversation)
                //{
                    userChatDetails.AppendLine(SendMessagesList.FirstOrDefault(x => x._ChatGroupName == userInfo._GroupNameChat)._ChatConversation[0]);
                //}

                Clients.Caller.receiveMessage("", userChatDetails.ToString(), "");
            }

            Clients.OthersInGroup(userInfo._GroupNameChat).receiveMessage("", newUserMsg, count);

        }

        public override System.Threading.Tasks.Task OnConnected()
        {
            return base.OnConnected();
        }

        public override async System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var id = Context.ConnectionId;

            EmailModel asyncEmails = new EmailModel();

            if (stopCalled)
            {
                Clients.AllExcept(id).userIsDisconnected();

                //sends the email to helpdesk group
                await asyncEmails.CreatesUserEndSessionEmail(CacheChatMessagesList, CacheChatMessagesList.First()._ChatGroupName);

            }
                

           // return base.OnDisconnected(stopCalled);
        }

        #endregion

        #region private Messages

        private void AddSystemMessageinCache(string chatGroupName, string message)
        {
            //verifies if the group name exists or not in the list
            if (!SendMessagesList.Exists(x => x._ChatGroupName == chatGroupName))
            { //if it does not exist, add a new item to the list
                List<string> chatMessages = new List<string>();
                chatMessages.Add("Sistema :" + message); //adds every new conversation message to the chat cache with the format user name - message

                //sets Message new information
                SendMessagesList.Add(new MessagesChatModel()
                {
                    _ChatGroupName = chatGroupName,
                    _ChatConversation = chatMessages
                });
            }
            else //if it exist only adds the message queue to the list
            {
                SendMessagesList.FirstOrDefault(x => x._ChatGroupName == chatGroupName)._ChatConversation.Add("Sistema: " + message); //sets messages dictionary to the list
            }

        }

        private void AddMessageinCache(string chatGroupName, string message, string userEmail, string name)
        {

            //verifies if the group name exists or not in the list
            if (!CacheChatMessagesList.Exists(x => x._ChatGroupName == chatGroupName))
            { //if it does not exist, add a new item to the list
                List<string> chatMessages = new List<string>();
                chatMessages.Add(name + ": " + message); //adds every new conversation message to the chat cache with the format user name - message

                Dictionary<string, bool> chatUserEmails = new Dictionary<string, bool>();
                chatUserEmails.Add(userEmail, false); //adds a new email to the list

                //sets Message new information
                CacheChatMessagesList.Add(new MessagesChatModel()
                {
                    _ChatGroupName = chatGroupName,
                    _ChatGroupEmails = chatUserEmails,
                    _ChatConversation = chatMessages
                });
            }
            else //if it exist only adds the message queue to the list
            {

                //verifies that the email is not already register at the list
                if (!CacheChatMessagesList.FirstOrDefault(x => x._ChatGroupName == chatGroupName)._ChatGroupEmails.ContainsKey(userEmail))
                {
                    CacheChatMessagesList.FirstOrDefault(x => x._ChatGroupName == chatGroupName)._ChatGroupEmails.Add(userEmail, false); //adds the new email to the list
                }

                CacheChatMessagesList.FirstOrDefault(x => x._ChatGroupName == chatGroupName)._ChatConversation.Add(name + ": " + message); //sets messages dictionary to the list
            }

        }

        #endregion

    }

}