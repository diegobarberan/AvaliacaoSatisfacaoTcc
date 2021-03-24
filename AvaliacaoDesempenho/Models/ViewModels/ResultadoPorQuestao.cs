using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models.ViewModels
{
    public class ResultadoPorQuestao
    {
        public int Codigo { get; set; }
        public string Questao { get; set; }
        public int TotalNunca { get; set; }
        public int TotalRaramente { get; set; }
        public int TotalEventualmente { get; set; }
        public int TotalSempre { get; set; }

    }
}
