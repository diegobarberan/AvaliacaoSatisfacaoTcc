using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models.ViewModels
{
    public class EmailData
    {
        [Required(ErrorMessage = "Campo Obrigatório!")]
        [EmailAddress(ErrorMessage = "O campo E-mail Institucional não é um endereço de e-mail válido.")]
        [Display(Name = "E-mail Institucional (exemplo@etec.sp.gov.br)")]
        public string Email { get; set; }
    }
}
