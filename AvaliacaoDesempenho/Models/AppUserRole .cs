using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Models
{
    public class AppUserRole : IdentityUserRole<string>
    {
        public virtual Usuario User { get; set; }
        public virtual AppRole Role { get; set; }
    }
}
