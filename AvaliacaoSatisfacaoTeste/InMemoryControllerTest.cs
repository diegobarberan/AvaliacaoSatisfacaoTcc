using AvaliacaoDesempenho.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaliacaoSatisfacaoTeste
{
    public class InMemoryControllerTest : ControllerTeste
    {
        public InMemoryControllerTest()
            : base(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase("TestDatabase")
                    .Options)
        {
        }
    }
}
