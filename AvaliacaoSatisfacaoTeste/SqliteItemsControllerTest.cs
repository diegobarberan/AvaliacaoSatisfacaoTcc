using AvaliacaoDesempenho.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaliacaoSatisfacaoTeste
{
    public class SqliteItemsControllerTest : ControllerTeste
    {
        public SqliteItemsControllerTest()
            : base(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite("Filename=Test.db")
                    .Options)
        {
        }
    }
}