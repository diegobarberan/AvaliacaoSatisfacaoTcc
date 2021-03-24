using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AvaliacaoDesempenho.Data;
using AvaliacaoDesempenho.Models;
using AvaliacaoDesempenho.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace AvaliacaoDesempenho.Controllers
{
    [Authorize(Roles = "Aluno")]
    public class Questao_AvaliacaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Questao_AvaliacaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Questao_Avaliacao
        public async Task<IActionResult> Questao(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var viewModel = new QuestaoIndexData();
            viewModel.Avaliacao = await _context.Avaliacao
                .Include(a => a.Disci_Turma)
                .ThenInclude(m => m.Professor)
                .Include(a => a.Disci_Turma)
                .ThenInclude(m => m.Disciplina)
                .Where(a => a.Codigo == id).FirstAsync();

            var questionario = await _context.Questionario.ToListAsync();
            TempData["TotalQuestao"] = questionario.Count();
            foreach (var item in questionario)
            {
                var questao = _context.Questao_Avaliacao
                    .Where(a => a.AvaliacaoId == id)
                    .Where(a => a.QuestionarioId == item.Codigo);
                if (questao.Any())
                {
                    continue;
                }
                viewModel.Questionario = item;
                return View(viewModel);

            }
            var avaliacao = viewModel.Avaliacao;
            avaliacao.Finalizado = true;
            if (avaliacao.Codigo > 0)
            {
                try
                {
                    _context.Update(avaliacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Avaliacao.Any(a => a.Codigo == avaliacao.Codigo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(Finalizado), new { id = viewModel.Avaliacao.Codigo });
        }

        public async Task<IActionResult> Finalizado(int? id)
        {
            return View(await _context.Avaliacao
                .Include(a => a.Disci_Turma)
                .ThenInclude(m => m.Professor)
                .Include(a => a.Disci_Turma)
                .ThenInclude(m => m.Disciplina)
                .Where(a => a.Codigo == id).ToListAsync());
        }



        // POST: Questao_Avaliacao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Nota,AvaliacaoId,QuestionarioId,Observacao")] Questao_Avaliacao questao_Avaliacao)
        {
            if (ModelState.IsValid)
            {
                if (questao_Avaliacao.Observacao != null)
                {
                    var avaliacao = _context.Avaliacao.Where(a => a.Codigo == questao_Avaliacao.AvaliacaoId).First();
                    avaliacao.Observacao = questao_Avaliacao.Observacao;
                    if (avaliacao.Codigo > 0)
                    {
                        try
                        {
                            _context.Update(avaliacao);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!_context.Avaliacao.Any(a => a.Codigo == avaliacao.Codigo))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
                _context.Add(questao_Avaliacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Questao), new { id = questao_Avaliacao.AvaliacaoId });
            }
            return View(questao_Avaliacao);
        }


        // GET: Questao_Avaliacao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var viewModel = new QuestaoIndexData();
            viewModel.Questao_Avaliacao = await _context.Questao_Avaliacao
                .Include(a => a.Avaliacao)
                    .ThenInclude(a => a.Disci_Turma)
                        .ThenInclude(a => a.Disciplina)
                .Include(a => a.Avaliacao)
                    .ThenInclude(a => a.Disci_Turma)
                        .ThenInclude(a => a.Professor)
                .Include(a => a.Questionario)
                .Where(a => a.Codigo == id).FirstOrDefaultAsync();

            var questionario = await _context.Questionario.ToListAsync();
            TempData["TotalQuestao"] = questionario.Count();

            if (viewModel.Questao_Avaliacao == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        // POST: Questao_Avaliacao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codigo,Nota,AvaliacaoId,QuestionarioId,Observacao")] Questao_Avaliacao questao_Avaliacao)
        {           
            
            if (id != questao_Avaliacao.Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {               
                try
                {
                    _context.Update(questao_Avaliacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Questao_AvaliacaoExists(questao_Avaliacao.Codigo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Avaliacoes");
            }
            
            return View(questao_Avaliacao);
        }

        /*
        // GET: Questao_Avaliacao/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questao_Avaliacao = await _context.Questao_Avaliacao
                .Include(q => q.Avaliacao)
                .Include(q => q.Questionario)
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (questao_Avaliacao == null)
            {
                return NotFound();
            }

            return View(questao_Avaliacao);
        }

        // POST: Questao_Avaliacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var questao_Avaliacao = await _context.Questao_Avaliacao.FindAsync(id);
            _context.Questao_Avaliacao.Remove(questao_Avaliacao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        */


        private bool Questao_AvaliacaoExists(int id)
        {
            return _context.Questao_Avaliacao.Any(e => e.Codigo == id);
        }
    }
}
