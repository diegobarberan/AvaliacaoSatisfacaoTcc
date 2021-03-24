using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models
{
    [Table("Avaliacoes")]
    public class Avaliacao
    {
        [Key, Display(Name = "Código")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!"), Display(Name = "Data"), Column(TypeName = "date")]
        public DateTime Data { get; set; }
        
        [Display(Name = "Data")]
        public bool Finalizado { get; set; }

        [Display(Name = "Disciplina Turma")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public int DisciTurmaId { get; set; }

        [Display(Name = "Matricula")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public string MatriculaId { get; set; }
        
        [Display(Name = "Observações e Elogios"), StringLength(200, ErrorMessage = "Quantidade máxima de caracteres são 200")]
        public string Observacao { get; set; }

        [Display(Name = "Visivel para Professor")]
        public bool VisivelProf { get; set; }


        [ForeignKey("DisciTurmaId")]
        public Disci_Turma Disci_Turma { get; set; }

        [ForeignKey("MatriculaId")]
        public Matricula Matricula { get; set; }

        public ICollection<Questao_Avaliacao> Questao_Avaliacaos { get; set; }
    }
}
