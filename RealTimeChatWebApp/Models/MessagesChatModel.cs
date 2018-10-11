using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealTimeChatWebApp.Models
{
    public class MessagesChatModel
    {
        public string _ChatGroupName { get; set; }

        public Dictionary<string, bool> _ChatGroupEmails { get; set; }

        public List<string> _ChatConversation { get; set; }


    }
}