using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models.ViewModels
{
    public class AvaliacaoIndexData
    {
        public IEnumerable<Avaliacao> Avaliacao { get; set; }
        public IEnumerable<Questao_Avaliacao> Questao_Avaliacaos { get; set; }
       
    }
}
