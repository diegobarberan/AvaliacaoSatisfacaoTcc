using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaliacaoDesempenho.Data;
using AvaliacaoDesempenho.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AvaliacaoDesempenho.Controllers
{
    [Authorize(Roles = "Professor,Professor/Coord")]
    public class RelatoriosProfessorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private string emailProf;

        public RelatoriosProfessorController(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IActionResult> Index()
        {
            emailProf = User.Identity.Name;

            var questionario = await _context.Questionario.ToListAsync();
            List<ResultadoQuestao> resulQuestao = new List<ResultadoQuestao>();
            foreach (var item in questionario)
            {
                var questoes = await _context.Questao_Avaliacao
                .Include(a => a.Avaliacao)
                .ThenInclude(m => m.Disci_Turma)
                .ThenInclude(n => n.Professor)
                .Where(a => a.QuestionarioId == item.Codigo)
                .Where(a => a.Avaliacao.Disci_Turma.Professor.Email == emailProf)
                .ToListAsync();
                double total = 0;
                foreach (var q in questoes)
                {
                    total = total + q.Nota;
                }
                ResultadoQuestao resul = new ResultadoQuestao();
                resul.Codigo = item.Codigo;
                resul.Nota = total / questoes.Count;
                resulQuestao.Add(resul);

            }
            ViewData["questionario"] = questionario.OrderBy(a => a.Codigo).ToList();
            ViewData["NomeProfessor"] = _context.Professor.First(a => a.Email == emailProf).Nome;
            return View(resulQuestao);
        }

        public IActionResult Observacao()
        {
            emailProf = User.Identity.Name;
            var avaliacoes = _context.Avaliacao
                .Include(a => a.Disci_Turma)
                .ThenInclude(d => d.Disciplina)
                .Include(a => a.Disci_Turma)
                .ThenInclude(d => d.Professor)
                .Include(a => a.Disci_Turma)
                .ThenInclude(a => a.Turma)
                .Where(a => a.Observacao != null)
                .Where(a => a.VisivelProf == true)
                .Where(a => a.Disci_Turma.Professor.Email == emailProf)
                .OrderBy(a => a.Disci_Turma.Professor.Nome).ToList();
            return View(avaliacoes);            
        }
        public IActionResult PorTurma(int? turmaId, int? disciTurmaId)
        {
            emailProf = User.Identity.Name;
            var questionario = _context.Questionario.ToList();
            ViewData["Turma"] = new SelectList(_context.Disci_Tuma
                .Include(a => a.Professor)
                .Include(a => a.Turma)
                .Where(a => a.Professor.Email == emailProf)
                .OrderBy(a => a.Turma.Nome), "Turma.Codigo", "Turma.Nome");
            ViewData["questionario"] = questionario.OrderBy(a => a.Codigo).ToList();

            if (turmaId != null)
            {
                ViewData["Disciplina"] = new SelectList(_context.Disci_Tuma
                    .Include(a => a.Disciplina)
                    .Include(a => a.Professor)
                    .Where(a => a.TurmaId == turmaId)
                    .Where(a => a.Professor.Email == emailProf)
                    .OrderBy(a => a.Disciplina.Nome), "Codigo", "Disciplina.Nome");
                return View();
            }

            if (disciTurmaId != null)
            {
                List<ResultadoQuestao> resulQuestao = new List<ResultadoQuestao>();

                foreach (var item in questionario)
                {
                    var questoes = _context.Questao_Avaliacao
                    .Include(a => a.Avaliacao)
                    .ThenInclude(a => a.Disci_Turma)
                    .Where(a => a.Avaliacao.DisciTurmaId == disciTurmaId)
                    .Where(a => a.QuestionarioId == item.Codigo)
                    .ToList();
                    double total = 0;
                    foreach (var q in questoes)
                    {
                        total = total + q.Nota;
                    }
                    ResultadoQuestao resul = new ResultadoQuestao();
                    resul.Codigo = item.Codigo;
                    resul.Nota = total / questoes.Count;
                    resulQuestao.Add(resul);
                }

                var avaliacoes = _context.Avaliacao
                    .Include(a => a.Disci_Turma)
                    .Where(a => a.Finalizado == true)
                    .Where(a => a.DisciTurmaId == disciTurmaId).ToList();
                ViewData["TotalTurma"] = _context.Matricula
                    .Where(a => a.TurmaId == avaliacoes.First().Disci_Turma.TurmaId)
                    .Count();
                ViewData["TotalRespondido"] = avaliacoes.Count();

                var disTurma = _context.Disci_Tuma
                    .Include(a => a.Disciplina)
                    .Include(a => a.Turma)
                    .Include(a => a.Professor)
                    .Where(a => a.Codigo == disciTurmaId).First();
                ViewData["NomeProf"] = disTurma.Professor.Nome;
                ViewData["NomeDisc"] = disTurma.Disciplina.Nome;
                ViewData["NomeTurma"] = disTurma.Turma.Nome;


                return View(resulQuestao);

            }

            return View();
        }

        public IActionResult PorQuestaoDisciplina(int? turmaId, int? disciTurmaId)
        {
            emailProf = User.Identity.Name;
            var questionario = _context.Questionario.ToList();
            ViewData["Turma"] = new SelectList(_context.Disci_Tuma
                 .Include(a => a.Professor)
                 .Include(a => a.Turma)
                 .Where(a => a.Professor.Email == emailProf)
                 .OrderBy(a => a.Turma.Nome), "Turma.Codigo", "Turma.Nome");

            if (turmaId != null)
            {
                ViewData["Disciplina"] = new SelectList(_context.Disci_Tuma
                    .Include(a => a.Disciplina)
                    .Include(a => a.Professor)
                    .Where(a => a.TurmaId == turmaId)
                    .Where(a => a.Professor.Email == emailProf)
                    .OrderBy(a => a.Disciplina.Nome), "Codigo", "Disciplina.Nome");
                return View();
            }

            if (disciTurmaId != null)
            {
                List<ResultadoPorQuestao> resulQuestao = new List<ResultadoPorQuestao>();

                foreach (var item in questionario)
                {
                    var questoes = _context.Questao_Avaliacao
                    .Include(a => a.Avaliacao)
                    .ThenInclude(a => a.Disci_Turma)
                    .Where(a => a.Avaliacao.DisciTurmaId == disciTurmaId)
                    .Where(a => a.QuestionarioId == item.Codigo)
                    .ToList();
                    ResultadoPorQuestao _resultadoPorQuestao = new ResultadoPorQuestao();
                    _resultadoPorQuestao.Codigo = item.Codigo;
                    _resultadoPorQuestao.Questao = item.Descricao;
                    _resultadoPorQuestao.TotalNunca = questoes.Count(a => a.Nota == 0);
                    _resultadoPorQuestao.TotalRaramente = questoes.Count(a => a.Nota == 1);
                    _resultadoPorQuestao.TotalEventualmente = questoes.Count(a => a.Nota == 2);
                    _resultadoPorQuestao.TotalSempre = questoes.Count(a => a.Nota == 3);
                    resulQuestao.Add(_resultadoPorQuestao);
                }

                var avaliacoes = _context.Avaliacao
                    .Include(a => a.Disci_Turma)
                    .Where(a => a.Finalizado == true)
                    .Where(a => a.DisciTurmaId == disciTurmaId).ToList();
                ViewData["TotalTurma"] = _context.Matricula
                    .Where(a => a.TurmaId == avaliacoes.First().Disci_Turma.TurmaId)
                    .Count();
                ViewData["TotalRespondido"] = avaliacoes.Count();

                var disTurma = _context.Disci_Tuma
                    .Include(a => a.Disciplina)
                    .Include(a => a.Turma)
                    .Include(a => a.Professor)
                    .Where(a => a.Codigo == disciTurmaId).First();
                ViewData["NomeProf"] = disTurma.Professor.Nome;
                ViewData["NomeDisc"] = disTurma.Disciplina.Nome;
                ViewData["NomeTurma"] = disTurma.Turma.Nome;

                return View(resulQuestao);
            }

            return View();
        }
    }
}
