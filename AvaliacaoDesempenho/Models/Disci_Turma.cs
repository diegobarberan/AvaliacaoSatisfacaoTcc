using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models
{
    [Table("Disci_Turmas")]
    public class Disci_Turma
    {
        [Key, Display(Name = "Código")]
        public int Codigo { get; set; }
        
        [Display(Name = "Turma")]
        [Required(ErrorMessage = "Campo Obrigatório")]        
        public int TurmaId { get; set; }
        [ForeignKey("TurmaId")]
        public Turma Turma { get; set; }


        [Display(Name = "Disciplina")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public int DisciplinaId { get; set; }
        [ForeignKey("DisciplinaId")]
        public Disciplina Disciplina { get; set; }


        [Display(Name = "Professor")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public int ProfessorId { get; set; }        
        [ForeignKey("ProfessorId")]
        public Professor Professor { get; set; }
        
        public ICollection<Avaliacao> Avaliacaos { get; set; }


    }
}
