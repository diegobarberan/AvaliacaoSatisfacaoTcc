using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models
{
    [Table("Professores")]
    public class Professor
    {
        [Key, Display(Name = "Código")]
        public int Codigo { get; set; }

        [Display(Name = "Nome"), Required(ErrorMessage = "Campo Obrigatório!"), StringLength(50)]
        public string Nome { get; set; }

        [Display(Name = "E-Mail"), Required(ErrorMessage = "Campo Obrigatório!"), EmailAddress]
        public string Email { get; set; }

        public ICollection<Disci_Turma> Disci_Turmas { get; set; }
    }
}
