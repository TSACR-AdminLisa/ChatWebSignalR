using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatWebApp.Models
{
    public class EmailModel
    {

        #region "Constantes"

            private const string HtmlEmailHeader = "<html><head><title></title></head><body style='font-family:arial; font-size:14px;'>";
            private const string HtmlEmailFooter = "</body></html>";

        #endregion

        #region "Properties"

            public List<string> To { get; set; }
            public List<string> CC { get; set; }
            public List<string> BCC { get; set; }
            public string From { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }

        #endregion

        #region "Constructor"

            public EmailModel()
            {
                To = new List<string>();
                CC = new List<string>();
                BCC = new List<string>();
            }

        #endregion

        #region "Functionality"

        public async Task<bool> CreatesUserStartSessionNotificationEmail(UserChatModel userDetails, string urlHelpDeskLogin)
        {
            string ToAddress = ConfigurationManager.AppSettings["SendToAddres"];
            char[] separatorArray = { ';' };
            string[] segmentedList = ToAddress.Split(separatorArray);
            string subject = "Nuevo usuario conectado al chat de servicio al cliente del BCCR";
            
            //gets To Address for the message to be send
            for (int x = 0; x <= segmentedList.Length - 2; x++)
            {
                To.Add(segmentedList[x]);
            }

            //creates the message body for this email
            StringBuilder messageBody = new StringBuilder();
            messageBody.Append("<div><p>Un nuevo usuario ha ingresado al chat de servicio al cliente del BCCR.<br/>");
            messageBody.Append("Información del usuario conectado:<br/>");
            messageBody.Append(String.Format("<ul><li>Nombre: {0}</li>", userDetails._UserFullName));
            messageBody.Append(String.Format("<li>Correo Electrónico: {0}</li>", userDetails._UserEmail));
            messageBody.Append(String.Format("<li>Teléfono: {0}</li>", userDetails._UserPhone));
            messageBody.Append(String.Format("<li>Pregunta: {0}</li></ul><br/>", userDetails._UserQuestion));
            messageBody.Append("Para ponerse en contacto con este usuario, por favor ingrese a la ");
            messageBody.Append(String.Format(" siguiente dirección: {0}</p></div>", urlHelpDeskLogin));

            foreach (string item in To)
            {
                MailAddress toAddress = new MailAddress(item);
                await SendEmailAsync(toAddress, subject, messageBody.ToString(), true);
            }

            return true;
        }

        public async Task<bool> CreatesUserEndSessionEmail(List<MessagesChatModel> paramCacheChatMessagesList, string paramGroupName)
        {
            //List<MessagesChatModel> cacheChatMessages = new List<MessagesChatModel>();
            string subject = "Resumen de la sesión chat del Servicio al Cliente del BCCR";
            List<string> emailRecipientsList = new List<string>();

            //add recipients addresses to the list
            foreach (var itemEmail in paramCacheChatMessagesList.FirstOrDefault(x => x._ChatGroupName == paramGroupName)._ChatGroupEmails)
            {
                emailRecipientsList.Add(itemEmail.Key);
            }

            To = new List<string>();
            To = emailRecipientsList;

            //creates the message body for this email
            StringBuilder messageBody = new StringBuilder();

            messageBody.Append("<div><p>A continuación se le brinda un resumen de la conversación realizada mediante<br/>");
            messageBody.Append("la plataforma de servicio al cliente del BCCR:<br/><br/><ul>");

            foreach (var item in paramCacheChatMessagesList.FirstOrDefault(x => x._ChatGroupName == paramGroupName)._ChatConversation)
            {
                messageBody.Append(String.Format("<li>{0}</li>", item));
            }

            messageBody.Append("</ul><br/><br/>Muchas gracias por utilizar nuestros servicios<br/>Servicio al Cliente BCCR</p></div>");

            //executes email sending
            foreach (string item in To)
            {
                //verifies if the email was already sent
                if (paramCacheChatMessagesList.FirstOrDefault(x => x._ChatGroupName == paramGroupName)._ChatGroupEmails[item] == false)
                {
                    MailAddress toAddress = new MailAddress(item);
                    await SendEmailAsync(toAddress, subject, messageBody.ToString(), true);

                    //sets that the emails was already sent to the recipient
                    paramCacheChatMessagesList.FirstOrDefault(x => x._ChatGroupName == paramGroupName)._ChatGroupEmails[item] = true;
                }

            }

            return true;
        }

        public async Task<bool> SendEmailAsync(MailAddress toAddress, string subject, string body, bool priority)
        {
            //MailMessage message = new MailMessage();

            //sets the configuration for the email service
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["SmptPort"]); //gets SMTP PORT
            string userName = ConfigurationManager.AppSettings["SmtpFromAddres"]; //gets SMTP user name
            string password = ConfigurationManager.AppSettings["SmtpPassword"]; //gets SMTP password
            string serverName = ConfigurationManager.AppSettings["SmtpServer"]; //gets SMTP Server Name

            MailAddress fromAddress = new MailAddress(ConfigurationManager.AppSettings["SmtpFromAddres"]); //gets SMTP From Addres

            //sets message content
            using (var message = new MailMessage(fromAddress, toAddress))
            {
                message.Subject = subject; //sets message subject
                message.Body = body; //sets message body
                message.IsBodyHtml = true; //sets HTML Text Format

                //sets message encoding
                message.HeadersEncoding = Encoding.UTF8;
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;

                //sets message priority
                if (priority)
                    message.Priority = MailPriority.High;

                //sets SMTP Client configuration
                using (var client = new SmtpClient(serverName, port))
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network; //sets delivery method
                    client.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpSsl"]); //sets SSL Encryption

                    //sets SMTP Client Credentials
                    client.UseDefaultCredentials = false;
                    NetworkCredential smtpUserInfo = new NetworkCredential(userName, password);
                    client.Credentials = smtpUserInfo;

                    //sends message
                    await client.SendMailAsync(message);
                }

            }

            return true;

        }

        #endregion

    }
}