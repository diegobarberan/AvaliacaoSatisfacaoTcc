using System;
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
    [Authorize(Roles = "Coordenador,Professor/Coord")]
    public class RelatoriosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RelatoriosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var questionario = await _context.Questionario.ToListAsync();
            List<ResultadoQuestao> resulQuestao = new List<ResultadoQuestao>();
            foreach (var item in questionario)
            {
                var questoes = await _context.Questao_Avaliacao
                .Include(a => a.Avaliacao)
                .Where(a => a.QuestionarioId == item.Codigo)
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
            return View(resulQuestao);
        }


        public IActionResult Observacao(int? turmaId)
        {
            ViewData["Turma"] = new SelectList(_context.Turma.OrderBy(a => a.Nome), "Codigo", "Nome");

            if (turmaId != null)
            {
                var avaliacoes = _context.Avaliacao
                    .Include(a => a.Disci_Turma)
                    .ThenInclude(d => d.Disciplina)
                    .Include(a => a.Disci_Turma)
                    .ThenInclude(d => d.Professor)
                    .Where(a => a.Disci_Turma.TurmaId == turmaId)
                    .Where(a => a.Observacao != null)
                    .OrderBy(a => a.Disci_Turma.Professor.Nome)
                    .ToList();
                ViewData["Turma"] = new SelectList(_context.Turma.OrderBy(a => a.Nome), "Codigo", "Nome", turmaId);
                return View(avaliacoes);
            }
            return View();
        }

        public IActionResult Liberar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var avaliacao = _context.Avaliacao
                .Include(a => a.Disci_Turma)
                .Where(a => a.Codigo == id)
                .First();
            if (avaliacao != null)
            {
                avaliacao.VisivelProf = true;
                _context.Update(avaliacao);
                _context.SaveChanges();
                return RedirectToAction(nameof(Observacao), new { turmaId = avaliacao.Disci_Turma.TurmaId });
            }
            return View();
        }

        public IActionResult Bloquear(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var avaliacao = _context.Avaliacao
                .Include(a => a.Disci_Turma)
                .Where(a => a.Codigo == id)
                .First();
            if (avaliacao != null)
            {
                avaliacao.VisivelProf = false;
                _context.Update(avaliacao);
                _context.SaveChanges();
                return RedirectToAction(nameof(Observacao), new { turmaId = avaliacao.Disci_Turma.TurmaId });
            }
            return View();
        }


        public IActionResult PorTurma(int? turmaId, int? disciTurmaId)
        {
            var questionario = _context.Questionario.ToList();
            ViewData["Turma"] = new SelectList(_context.Turma.OrderBy(a => a.Nome), "Codigo", "Nome");
            ViewData["questionario"] = questionario.OrderBy(a => a.Codigo).ToList();

            if (turmaId != null)
            {
                ViewData["Disciplina"] = new SelectList(_context.Disci_Tuma
                    .Include(a => a.Disciplina)
                    .Include(a => a.Professor)
                    .Where(a => a.TurmaId == turmaId)
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

                if (avaliacoes.Count > 0)
                {

                    ViewData["TotalTurma"] = _context.Matricula
                        .Where(a => a.TurmaId == avaliacoes.First().Disci_Turma.TurmaId)
                        .Count();
                    ViewData["TotalRespondido"] = avaliacoes.Count();
                }
                ViewData["TotalTurma"] = 0;
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
            var questionario = _context.Questionario.ToList();
            ViewData["Turma"] = new SelectList(_context.Turma.OrderBy(a => a.Nome), "Codigo", "Nome");

            if (turmaId != null)
            {
                ViewData["Disciplina"] = new SelectList(_context.Disci_Tuma
                    .Include(a => a.Disciplina)
                    .Include(a => a.Professor)
                    .Where(a => a.TurmaId == turmaId).OrderBy(a => a.Disciplina.Nome), "Codigo", "Disciplina.Nome");
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

                if (avaliacoes.Count > 0)

                {
                    var totalTurma = _context.Matricula
                        .Where(a => a.TurmaId == avaliacoes.First().Disci_Turma.TurmaId)
                        .Count();

                    ViewData["TotalTurma"] = totalTurma;
                    ViewData["TotalRespondido"] = avaliacoes.Count();
                }
                ViewData["TotalTurma"] = 0;
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
