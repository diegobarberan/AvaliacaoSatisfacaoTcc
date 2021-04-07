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

        private void Seed()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                //Add Curso
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

                //Add Disciplina
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

                //Add Professor
                context.AddRange(new Professor()
                {
                    Codigo = 1,
                    Nome = "Diego",
                    Email = "diego@diego.com.br"
                }, new Professor()
                {
                    Codigo = 2,
                    Nome = "Gisele",
                    Email = "gisele@gisele.com.br"
                }, new Professor()
                {
                    Codigo = 3,
                    Nome = "Gustavo",
                    Email = "gustavo@gustavo.com.br"
                });
                context.SaveChanges();

                //Add Questionario
                context.AddRange(new Questionario()
                {
                    Codigo = 1,
                    Descricao = "O Professor tem dominio sobre a conteudo ministrado?"

                }, new Questionario()
                {
                    Codigo = 2,
                    Descricao = "O Professor aceita criticas?"
                }, new Questionario()
                {
                    Codigo = 3,
                    Descricao = "O Professor motiva os alunos?"
                });
                context.SaveChanges();

                //add Turma
                context.AddRange(new Turma()
                {
                    Codigo = 1,
                    Nome = "1º Etim Informática",
                    Data = System.DateTime.Now,
                    CursoId = 2
                }, new Turma()
                {
                    Codigo = 2,
                    Nome = "1º Etim Administração",
                    Data = System.DateTime.Now,
                    CursoId = 1
                });
                context.SaveChanges();

                //Add Turma Disciplina
                context.AddRange(new Disci_Turma()
                {
                    TurmaId = 1,
                    DisciplinaId = 1,
                    ProfessorId = 1
                }, new Disci_Turma()
                {
                    TurmaId = 1,
                    DisciplinaId = 2,
                    ProfessorId = 2
                });
                context.SaveChanges();

                //Add Turma Matricula
                context.AddRange(new Matricula()
                {
                    RM = "1",
                    Data = System.DateTime.Now,
                    EmailInst = "teste@teste.com.br",
                    TurmaId = 1
                }, new Matricula()
                {
                    RM = "2",
                    Data = System.DateTime.Now,
                    EmailInst = "teste2@teste.com.br",
                    TurmaId = 1
                });
                context.SaveChanges();
            }
        }

        //Teste Curso
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfCurso()
        {
            Seed();
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new CursosController(context);

                // Act
                var result = await controller.Index();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Curso>>(
                    viewResult.ViewData.Model);
                Assert.Equal(3, model.Count());
                Assert.Equal("Segurança do Trabalho", model.LastOrDefault().Nome);
            }
        }

        [Fact]
        public async Task Details_ReturnsANotFoundResult_ErrorDetailsOfCurso()
        {            
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new CursosController(context);

                // Act
                var result = await controller.Details(8);

                // Assert
                Assert.IsType<NotFoundResult>(result);
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
                // Arrange
                var controller = new CursosController(context);

                // Act
                var result = await controller.Index();

                // Assert
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
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new DisciplinasController(context);

                // Act
                var result = await controller.Index();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Disciplina>>(
                    viewResult.ViewData.Model);
                Assert.Equal(3, model.Count());
                Assert.Equal("Matemática", model.Where(a => a.Nome == "Matemática").FirstOrDefault().Nome);
            }
        }

        [Fact]
        public async Task Details_ReturnsANotFoundResult_ErrorDetailsOfDisciplina()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new DisciplinasController(context);

                // Act
                var result = await controller.Details(8);

                // Assert
                Assert.IsType<NotFoundResult>(result);
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
                // Arrange
                var controller = new DisciplinasController(context);

                // Act
                var result = await controller.Index();

                // Assert
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
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new ProfessorsController(context);

                // Act
                var result = await controller.Index();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Professor>>(
                    viewResult.ViewData.Model);
                Assert.Equal(4, model.Count());
                Assert.Equal("Gustavo", model.Where(a => a.Nome == "Gustavo").FirstOrDefault().Nome);
            }
        }

        [Fact]
        public async Task Details_ReturnsANotFoundResult_ErrorDetailsOfProfessor()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new ProfessorsController(context);

                // Act
                var result = await controller.Details(8);

                // Assert
                Assert.IsType<NotFoundResult>(result);
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
                // Arrange
                var controller = new ProfessorsController(context);

                // Act
                var result = await controller.Index();

                // Assert
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
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new QuestionariosController(context);

                // Act
                var result = await controller.Index();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Questionario>>(
                    viewResult.ViewData.Model);
                Assert.Equal(4, model.Count());
                Assert.Equal("O Professor motiva os alunos?", model.Where(a => a.Descricao == "O Professor motiva os alunos?").FirstOrDefault().Descricao);
            }
        }

        [Fact]
        public async Task Details_ReturnsANotFoundResult_ErrorDetailsOfQuestionario()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new QuestionariosController(context);

                // Act
                var result = await controller.Details(8);

                // Assert
                Assert.IsType<NotFoundResult>(result);
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
                // Arrange
                var controller = new QuestionariosController(context);

                // Act
                var result = await controller.Index();

                // Assert
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
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new TurmasController(context);

                // Act
                var result = await controller.Index();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Turma>>(
                    viewResult.ViewData.Model);
                Assert.Equal(3, model.Count());
                Assert.Equal("1º Etim Informática", model.Where(a => a.Nome == "1º Etim Informática").FirstOrDefault().Nome);
            }
        }

        [Fact]
        public async Task Details_ReturnsANotFoundResult_ErrorDetailsOfTurma()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new TurmasController(context);

                // Act
                var result = await controller.Details(8);

                // Assert
                Assert.IsType<NotFoundResult>(result);
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
                turma.Nome = "Turma Teste";
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
                // Arrange
                var controller = new TurmasController(context);

                // Act
                var result = await controller.Index();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Turma>>(
                    viewResult.ViewData.Model);
                Assert.Equal("Turma Teste", model.Where(a => a.Nome == "Turma Teste").FirstOrDefault().Nome);
                Assert.Equal(3, model.Where(a => a.Nome == "Turma Teste").FirstOrDefault().Codigo);
            }
        }

        [Fact]
        public async Task Details_ReturnsANotFoundResult_ErrorDetailsOfTurmaDisci()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new Disci_TurmaController(context);

                // Act
                var result = await controller.Details(8);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        //Teste Disci_Turma
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfTurmaDisci()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new Disci_TurmaController(context);                

                // Act
                var result = await controller.Index(1);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Disci_Turma>>(
                    viewResult.ViewData.Model);
                Assert.Equal(2, model.Count());
                Assert.Equal(1, model.Where(a => a.DisciplinaId == 1).FirstOrDefault().DisciplinaId);
            }
        }

        [Fact]
        public async Task Create_AddTurmaDisci_ReturnsAViewResultAsync()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new Disci_TurmaController(context);
                var disciTurma = new Disci_Turma();
                disciTurma.Codigo = 3;
                disciTurma.DisciplinaId = 3;
                disciTurma.ProfessorId = 3;
                disciTurma.TurmaId = 2;

                // Act
                var result = await controller.Create(disciTurma);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Null(redirectToActionResult.ControllerName);
                Assert.Equal("Index", redirectToActionResult.ActionName);
            }

            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new Disci_TurmaController(context);

                // Act
                var result = await controller.Index(2);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Disci_Turma>>(
                    viewResult.ViewData.Model);
                Assert.Equal(3, model.Where(a => a.Codigo == 3).FirstOrDefault().Codigo);                
            }
        }

        //Teste Matricula
        [Fact]
        public void Index_ReturnsAViewResult_WithAListOfMatricula()
        {            
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new MatriculasController(context);


                // Act
                var result = controller.Index(1);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Matricula>>(
                    viewResult.ViewData.Model);
                Assert.Equal(2, model.Count());
                Assert.Equal("1", model.LastOrDefault().RM);
            }
        }

        [Fact]
        public async Task Details_ReturnsANotFoundResult_ErrorDetailsOfMatricula()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new MatriculasController(context);

                // Act
                var result = await controller.Details("8");

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task Create_AddMatricula_ReturnsAViewResultAsync()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new MatriculasController(context);
                var matricula = new Matricula();
                matricula.RM = "3";
                matricula.Data = System.DateTime.Now;
                matricula.EmailInst = "teste3@teste.com.br";
                matricula.TurmaId = 2;

                // Act
                var result = await controller.Create(matricula);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Null(redirectToActionResult.ControllerName);
                Assert.Equal("Index", redirectToActionResult.ActionName);
            }

            using (var context = new ApplicationDbContext(ContextOptions))
            {
                // Arrange
                var controller = new MatriculasController(context);

                // Act
                var result = controller.Index(2);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Matricula>>(
                    viewResult.ViewData.Model);
                Assert.Single(model);
                Assert.Equal("teste3@teste.com.br", model.Where(a => a.EmailInst == "teste3@teste.com.br").FirstOrDefault().EmailInst);
            }
        }
    }
}
