using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models
{
    public class AppRole : IdentityRole
    {        

        public virtual ICollection<AppUserRole> UserRoles { get; set; }        

        public AppRole() { }

        public AppRole(string name)
        {
            Name = name;
        }
    }
}
