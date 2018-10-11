using System.ComponentModel.DataAnnotations;

namespace RealTimeChatWebApp.Models
{
    public class UserChatModel
    {
        #region "Enumerator"

            public enum RegisteredUserType
            {
                regularUser = 0,
                helpDeskUser = 1
            }

        #endregion

        #region "Properties"

            [Display(Name = "Nombre", Description = "Almacena el nombre completo del usuario que ingresa al chat", ShortName = "Nombre")]
            [Required(ErrorMessage = "Por favor, ingrese su nombre")]
            public string _UserFullName { get; set; }

            [Display(Name = "Correo Electrónico", Description = "Ingrese su correo electrónico", ShortName = "Email")]
            [Required(ErrorMessage = "Por favor, ingrese su correo electrónico")]
            [EmailAddress(ErrorMessage = "Por favor, ingrese un correo electrónico válido")]
            public string _UserEmail { get; set; }

            [Display(Name = "Pregunta", Description = "Ingrese la pregunta a ser realizada", ShortName = "Pregunta")]
            [Required(ErrorMessage = "Por favor, ingrese la pregunta a ser realizada")]
            public string _UserQuestion { get; set; }

            [Display(Name = "Teléfono", Description = "Ingrese un número de teléfono", ShortName = "Teléfono", Prompt = "####-####")]
            [MaxLength(9, ErrorMessage = "El número de teléfono ingresado no puede ser superior a 9 dígitos")]
            [Required(ErrorMessage = "Por favor, ingrese un número de teléfono")]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^\(?([0-9]{4})[-. ]?([0-9]{4})$", ErrorMessage = "Por favor, ingrese un número de teléfono válido")]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:####-####}")]
            public string _UserPhone { get; set; }

            [Display(Description = "Almacena el Id del usuario asignado al chat", ShortName = "ID Usuario")]
            public string _UserId { get; set; }

            public string _GroupNameChat { get; set; }

            public RegisteredUserType _UserType { get; set; }

        #endregion



    }
}