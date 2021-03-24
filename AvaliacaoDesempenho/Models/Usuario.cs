using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models
{
    public class Usuario : IdentityUser
    {
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
        public virtual ICollection<AppUserRole> UserRoles { get; set; }

        [PersonalData, Required(ErrorMessage = "Campo Obrigatório!"), StringLength(50)]
        public string Nome { get; set; }           

        [PersonalData]
        public ICollection<Matricula> Matriculas { get; set; }
    }
}
