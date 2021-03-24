using AvaliacaoDesempenho.Data.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models
{
    [Table("Questao_Avaliacoes")]
    public class Questao_Avaliacao
    {
        [Key, Display(Name = "Código")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório"), Display(Name = "Nota")]
        public int Nota { get; set; }

        [Display(Name = "Avaliação")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public int AvaliacaoId { get; set; }

        [Display(Name = "Questionario")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public int QuestionarioId { get; set; }

        [ForeignKey("AvaliacaoId")]
        public Avaliacao Avaliacao { get; set; }

        [ForeignKey("QuestionarioId")]
        public Questionario Questionario { get; set; }

        [NotMapped]
        [Display(Name = "Observação - máximo 200 caracteres"), StringLength(200, ErrorMessage = "Quantidade máxima de caracteres são 200")]
        public string Observacao { get; set; }
    }
}
