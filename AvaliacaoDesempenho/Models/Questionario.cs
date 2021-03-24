using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models
{
    [Table("Questionarios")]
    public class Questionario
    {
        [Key, Display(Name = "Código")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório"), Display(Name = "Descrição"), MaxLength(500)]
        public string Descricao { get; set; }

        public ICollection<Questao_Avaliacao> Questao_Avaliacaos { get; set; }
    }
}
