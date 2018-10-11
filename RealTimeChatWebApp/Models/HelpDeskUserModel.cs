using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RealTimeChatWebApp.Models
{
    public class HelpDeskUserModel
    {

        public string _ID { get; set; }

        [Display(Name = "Usuario", Description = "Ingrese su usuario", ShortName = "Usuario")]
        [Required(ErrorMessage = "Por favor ingrese su usuario")]
        public string _LoginName { get; set; }

        [Display(Name = "Contraseña", Description = "Ingrese su contraseña", ShortName = "Contraseña")]
        [Required(ErrorMessage = "Por favor ingrese su contraseña")]
        public string _Password { get; set; }

        [Display(Name = "Nombre", Description = "Ingrese su nombre", ShortName = "Nombre")]
        [Required(ErrorMessage = "Por favor ingrese su nombre")]
        public string _FullName { get; set; }

        [Display(Name = "Correo Electrónico", Description = "Ingrese su correo electrónico", ShortName = "Email")]
        [Required(ErrorMessage = "Por favor, ingrese su correo electrónico")]
        [EmailAddress(ErrorMessage = "Por favor, ingrese un correo electrónico válido")]
        public string _UserEmail { get; set; }

        [Display(Name = "ID Grupo", Description = "Ingrese el ID del grupo", ShortName = "ID Grupo")]
        //[Required(ErrorMessage = "Por favor ingrese el ID del grupo asignado")]
        public string _GroupNameChat { get; set; }

    }
}