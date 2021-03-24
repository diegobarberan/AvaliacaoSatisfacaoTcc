using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models.ViewModels
{
    public class QuestaoIndexData
    {
        public Avaliacao Avaliacao { get; set; }
        public Questionario Questionario { get; set; }
        public Questao_Avaliacao Questao_Avaliacao { get; set; }

    }
}
