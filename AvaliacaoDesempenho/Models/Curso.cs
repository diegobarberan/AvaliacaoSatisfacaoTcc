using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models
{
    [Table("Cursos")]
    public class Curso
    {
        [Key, Display(Name = "Código")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!"), Display(Name = "Nome"), StringLength(50)]
        public string Nome { get; set; }

        [Display(Name = "Nível"), StringLength(10)]
        public string Nivel { get; set; }
        
    }
}
