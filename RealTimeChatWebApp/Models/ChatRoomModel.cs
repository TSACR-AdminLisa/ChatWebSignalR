using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RealTimeChatWebApp.Models
{
    public class ChatRoomModel
    {

        [Display(Description = "Almacena el nombre del grupo o sala de chat donde se asignarán los usuarios", ShortName = "Grupo Chat")]
        public string _RoomName { get; set; }  //nombre del grupo de chat

        [Display(Description = "Almacena el Id de la conexión donde se asignarán los usuarios", ShortName = "ID Conexión")]
        public string _ConnectionId { get; set; }

        [Display(Description = "Almacena la lista de los usuarios que serán asignados al chat", ShortName = "Lista Usuarios")]
        public List<UserChatModel> _UserInfoList { get; set; }

        [Display(Description = "Almacena la lista de los ultimos mensajes enviados al chat", ShortName = "Lista Mensajes")]
        public List<MessagesChatModel> _SendMessagesList { get; set; }

    }
}