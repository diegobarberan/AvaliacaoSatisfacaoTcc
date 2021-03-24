using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models
{
    [Table("Turmas")]
    public class Turma
    {
        [Key, Display(Name = "Código")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!"), Display(Name = "Nome"), StringLength(50)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!"), Display(Name = "Data")]
        public DateTime Data { get; set; }

        [Display(Name = "Curso")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public int CursoId { get; set; }

        [ForeignKey("CursoId")]
        public Curso Curso { get; set; }

        public ICollection<Matricula> Matriculas { get; set; }

        public ICollection<Disci_Turma> Disci_Turmas { get; set; }


    }
}
