using AvaliacaoDesempenho.Controllers;
using AvaliacaoDesempenho.Data;
using AvaliacaoDesempenho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AvaliacaoSatisfacaoTeste
{
    public abstract class ControllerTeste
    {
        protected ControllerTeste(DbContextOptions<ApplicationDbContext> contextOptions)
        {
            ContextOptions = contextOptions;

        }

        protected DbContextOptions<ApplicationDbContext> ContextOptions { get; }

        private void SeedCurso()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.AddRange(new Curso()
                {
                    Codigo = 1,
                    Nome = "Administração",
                    Nivel = "Técnico"
                }, new Curso()
                {
                    Codigo = 2,
                    Nome = "Informática",
                    Nivel = "Técnico"
                }, new Curso()
                {
                    Codigo = 3,
                    Nome = "Segurança do Trabalho",
                    Nivel = "Técnico"
                });
                context.SaveChanges();
            }
        }

        private void SeedDisciplina()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                context.AddRange(new Disciplina()
                {
                    Codigo = 1,
                    Nome = "Matemática"
                }, new Disciplina()
                {
                    Codigo = 2,
                    Nome = "Geografia"
                }, new Disciplina()
                {
                    Codigo = 3,
                    Nome = "Gestão de Sistemas Operacionais"
                });
                context.SaveChanges();
            }
        }

        private void SeedProfessor()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                context.AddRange(new Professor()
                {
                    Nome = "Diego",
                    Email = "diego@diego.com.br"
                }, new Professor()
                {
                    Nome = "Gisele",
                    Email = "gisele@gisele.com.br"
                }, new Professor()
                {
                    Nome = "Gustavo",
                    Email = "gustavo@gustavo.com.br"
                });
                context.SaveChanges();
            }
        }

        private void SeedQuestionario()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                context.AddRange(new Questionario()
                {

                    Descricao = "O Professor tem dominio sobre a conteudo ministrado?"

                }, new Questionario()
                {

                    Descricao = "O Professor aceita criticas?"
                }, new Questionario()
                {
                    Descricao = "O Professor motiva os alunos?"
                });
                context.SaveChanges();
            }
        }

        private void SeedTurma()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                context.AddRange(new Turma()
                {
                    Codigo = 10,
                    Nome = "1º Etim Informática",
                    Data = System.DateTime.Now,
                    CursoId = 2
                }, new Turma()
                {
                    Codigo = 11,
                    Nome = "1º Etim Administração",
                    Data = System.DateTime.Now,
                    CursoId = 1
                });
                context.SaveChanges();
            }
            using (var context = new ApplicationDbContext(ContextOptions))
            {

                context.AddRange(new Disci_Turma()
                {
                    TurmaId = 10,
                    DisciplinaId = 1,
                    ProfessorId = 1
                }, new Disci_Turma()
                {
                    TurmaId = 10,
                    DisciplinaId = 2,
                    ProfessorId = 2
                });
                context.SaveChanges();
            }
        }


        //Teste Curso
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfCurso()
        {
            SeedCurso();
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                var controller = new CursosController(context);

                var result = await controller.Index();

                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Curso>>(
                    viewResult.ViewData.Model);
                Assert.Equal(3, model.Count());
                Assert.Equal("Segurança do Trabalho", model.LastOrDefault().Nome);
            }
        }

        [Fact]
        public async Task Create_AddCurso_ReturnsAViewResultAsync()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new CursosController(context);
                var curso = new Curso();
                curso.Nome = "Ensino Médio";
                curso.Nivel = "Médio";

                // Act
                var result = await controller.Create(curso);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Null(redirectToActionResult.ControllerName);
                Assert.Equal("Index", redirectToActionResult.ActionName);
            }

            using (var context = new ApplicationDbContext(ContextOptions))
            {
                var controller = new CursosController(context);

                var result = await controller.Index();

                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Curso>>(
                    viewResult.ViewData.Model);
                Assert.Equal(4, model.Count());
                Assert.Equal("Ensino Médio", model.Where(a => a.Nome == "Ensino Médio").FirstOrDefault().Nome);
            }
        }

        //Teste Disciplina
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfDisciplina()
        {
            SeedDisciplina();
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                var controller = new DisciplinasController(context);

                var result = await controller.Index();

                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Disciplina>>(
                    viewResult.ViewData.Model);
                Assert.Equal(3, model.Count());
                Assert.Equal("Matemática", model.Where(a => a.Nome == "Matemática").FirstOrDefault().Nome);
            }
        }

        [Fact]
        public async Task Create_AddDisciplina_ReturnsAViewResultAsync()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new DisciplinasController(context);
                var disciplina = new Disciplina();
                disciplina.Nome = "História";


                // Act
                var result = await controller.Create(disciplina);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Null(redirectToActionResult.ControllerName);
                Assert.Equal("Index", redirectToActionResult.ActionName);
            }

            using (var context = new ApplicationDbContext(ContextOptions))
            {
                var controller = new DisciplinasController(context);

                var result = await controller.Index();

                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Disciplina>>(
                    viewResult.ViewData.Model);
                Assert.Equal("História", model.Where(a => a.Nome == "História").FirstOrDefault().Nome);
            }
        }

        //Teste Professor
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfProfessor()
        {
            SeedProfessor();
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                var controller = new ProfessorsController(context);

                var result = await controller.Index();

                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Professor>>(
                    viewResult.ViewData.Model);
                Assert.Equal(4, model.Count());
                Assert.Equal("Gustavo", model.Where(a => a.Nome == "Gustavo").FirstOrDefault().Nome);
            }
        }

        [Fact]
        public async Task Create_AddProfessor_ReturnsAViewResultAsync()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new ProfessorsController(context);
                var professor = new Professor();
                professor.Nome = "Andreia";
                professor.Email = "andreia@andreia.com.br";


                // Act
                var result = await controller.Create(professor);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Null(redirectToActionResult.ControllerName);
                Assert.Equal("Index", redirectToActionResult.ActionName);
            }

            using (var context = new ApplicationDbContext(ContextOptions))
            {
                var controller = new ProfessorsController(context);

                var result = await controller.Index();

                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Professor>>(
                    viewResult.ViewData.Model);
                Assert.Equal("Andreia", model.Where(a => a.Nome == "Andreia").FirstOrDefault().Nome);
            }
        }

        //Teste Questionário
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfQuestionario()
        {
            SeedQuestionario();
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                var controller = new QuestionariosController(context);

                var result = await controller.Index();

                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Questionario>>(
                    viewResult.ViewData.Model);
                Assert.Equal(4, model.Count());
                Assert.Equal("O Professor motiva os alunos?", model.Where(a => a.Descricao == "O Professor motiva os alunos?").FirstOrDefault().Descricao);
            }
        }

        [Fact]
        public async Task Create_AddQuestionario_ReturnsAViewResultAsync()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new QuestionariosController(context);
                var questionario = new Questionario();
                questionario.Descricao = "Teste questionario";


                // Act
                var result = await controller.Create(questionario);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Null(redirectToActionResult.ControllerName);
                Assert.Equal("Index", redirectToActionResult.ActionName);
            }

            using (var context = new ApplicationDbContext(ContextOptions))
            {
                var controller = new QuestionariosController(context);

                var result = await controller.Index();

                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Questionario>>(
                    viewResult.ViewData.Model);
                Assert.Equal("Teste questionario", model.Where(a => a.Descricao == "Teste questionario").FirstOrDefault().Descricao);
            }
        }

        //Teste Turma
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfTurma()
        {
            SeedTurma();
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                var controller = new TurmasController(context);

                var result = await controller.Index();

                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Turma>>(
                    viewResult.ViewData.Model);
                Assert.Equal(2, model.Count());
                Assert.Equal("1º Etim Informática", model.Where(a => a.Nome == "1º Etim Informática").FirstOrDefault().Nome);
            }
        }

        [Fact]
        public async Task Create_AddTurma_ReturnsAViewResultAsync()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new TurmasController(context);
                var turma = new Turma();
                turma.Nome = "Teste Turma";
                turma.Data = System.DateTime.Now;
                turma.CursoId = 3;


                // Act
                var result = await controller.Create(turma);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Null(redirectToActionResult.ControllerName);
                Assert.Equal("Index", redirectToActionResult.ActionName);
            }

            using (var context = new ApplicationDbContext(ContextOptions))
            {
                var controller = new TurmasController(context);

                var result = await controller.Index();

                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Turma>>(
                    viewResult.ViewData.Model);
                Assert.Equal("Teste Turma", model.Where(a => a.Nome == "Teste Turma").FirstOrDefault().Nome);
                Assert.Equal(1, model.Where(a => a.Nome == "Teste Turma").FirstOrDefault().Codigo);
            }
        }

        //Teste Disci_Turma
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfTurmaDisci()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                var controller = new Disci_TurmaController(context);
                int id = 1;

                var result = await controller.Index(id);

                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Disci_Turma>>(
                    viewResult.ViewData.Model);
                //Assert.Equal(0, model.Count());                
                Assert.Equal(0, model.Where(a => a.DisciplinaId == 1).FirstOrDefault().DisciplinaId);
            }
        }
    }
}
