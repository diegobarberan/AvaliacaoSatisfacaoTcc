using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models
{
    [Table("Matriculas")]
    public class Matricula
    {
        [Key, Display(Name = "RM"), Column(TypeName = "varchar(10)")]
        public string RM { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!"), Display(Name = "E-mail Institucional"), EmailAddress, StringLength(50)]
        public string EmailInst { get; set; }        

        [Required(ErrorMessage = "Campo Obrigatório!"), Display(Name = "Data Matricula")]
        public DateTime Data { get; set; }

        [Display(Name = "Controle Avaliação")]
        public bool ControleAvaliacao { get; set; }

        [Display(Name = "Usuário")]        
        public string UsuarioId { get; set; }

        [Display(Name = "Turma")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public int TurmaId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [ForeignKey("TurmaId")]
        public Turma Turma { get; set; }

        public ICollection<Avaliacao> Avaliacaos { get; set; }        
    }
}
