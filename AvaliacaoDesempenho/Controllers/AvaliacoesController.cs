using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AvaliacaoDesempenho.Data;
using AvaliacaoDesempenho.Models;
using Microsoft.AspNetCore.Authorization;
using AvaliacaoDesempenho.Models.ViewModels;

namespace AvaliacaoDesempenho.Controllers
{
    [Authorize(Roles = "Aluno")]
    public class AvaliacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        [ViewData]
        public bool ControleMatricula { get; set; }

        public AvaliacoesController(ApplicationDbContext context)
        {
            _context = context;
            ControleMatricula = false;
        }


        public async Task<IActionResult> Index(int? id)
        {
            var viewModel = new AvaliacaoIndexData();
            var usuarioId = _context.Users.First(m => m.Email == User.Identity.Name).Id;
            var _matriculaId = _context.Matricula.First(a => a.UsuarioId == usuarioId).RM;

            if (!_context.Avaliacao
                .Where(a => a.Finalizado == false).Any(a => a.MatriculaId == _matriculaId))
            {
                ControleMatricula = true;
                var mat = _context.Matricula.First(a => a.RM == _matriculaId);
                if (mat.ControleAvaliacao == false)
                {
                    try
                    {
                        mat.ControleAvaliacao = true;
                        _context.Update(mat);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;

                    }
                }
            }

            viewModel.Avaliacao = await _context.Avaliacao
                .Include(a => a.Disci_Turma)
                    .ThenInclude(d => d.Disciplina)
                .Include(a => a.Disci_Turma)
                    .ThenInclude(d => d.Professor)
                .Include(a => a.Matricula)
                .Include(a => a.Disci_Turma)
                    .ThenInclude(a => a.Turma)
                    .ThenInclude(a => a.Curso)
                .Include(a => a.Questao_Avaliacaos)
                .Where(a => a.MatriculaId == _matriculaId)
                .OrderBy(a => a.Disci_Turma.Disciplina.Nome).ToListAsync();

            if (id != null)
            {
                ViewData["AvaliacaoId"] = id.Value;
                viewModel.Questao_Avaliacaos = await _context.Questao_Avaliacao
                    .Include(a => a.Questionario)
                    .Include(a => a.Avaliacao)
                    .ThenInclude(m => m.Disci_Turma)
                    .ThenInclude(n => n.Disciplina)
                    .Include(a => a.Avaliacao)
                    .ThenInclude(m => m.Disci_Turma)
                    .ThenInclude(n => n.Professor)
                    .Where(a => a.AvaliacaoId == id.Value)
                    .OrderBy(a => a.Codigo).ToListAsync();
            }
            return View(viewModel);
        }

        // GET: Avaliacoes/Details/5       

        // GET: Avaliacoes/Create
        public IActionResult Create()
        {
            var usuarioId = _context.Users.First(m => m.Email == User.Identity.Name).Id;
            var _matricula = _context.Matricula.First(a => a.UsuarioId == usuarioId);

            ViewData["DisciTurma"] = new SelectList(_context.Disci_Tuma
                .Include(a => a.Turma)
                .Where(a => a.TurmaId == _matricula.TurmaId)
                .Include(a => a.Disciplina)
                , "Codigo", "Disciplina.Nome");
            ViewData["Matricula"] = new SelectList(_context.Matricula
                .Where(a => a.RM == _matricula.RM)
                , "RM", "RM");
            return View();
        }

        // POST: Avaliacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Data,DisciTurmaId,MatriculaId")] Avaliacao avaliacao)
        {
            if (ModelState.IsValid)
            {
                var usuId = _context.Users.First(m => m.Email == User.Identity.Name).Id;
                var _mat = _context.Matricula.First(a => a.UsuarioId == usuId);
                var _disciTurma = _context.Disci_Tuma
                .Include(a => a.Turma)
                .Where(a => a.TurmaId == _mat.TurmaId)
                .Include(a => a.Disciplina).ToList();
                foreach (var item in _disciTurma)
                {
                    avaliacao.DisciTurmaId = item.Codigo;
                    if (_context.Avaliacao
                        .Where(a => a.DisciTurmaId == avaliacao.DisciTurmaId)
                        .Where(a => a.MatriculaId == avaliacao.MatriculaId).Any())
                    {
                        continue;
                    }
                    _context.Add(avaliacao);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));

            }
            var usuarioId = _context.Users.First(m => m.Email == User.Identity.Name).Id;
            var _matricula = _context.Matricula.First(a => a.UsuarioId == usuarioId);
            ViewData["DisciTurma"] = new SelectList(_context.Disci_Tuma
               .Include(a => a.Turma)
                   .Where(a => a.TurmaId == _matricula.TurmaId)
                   .Include(a => a.Disciplina)
                   , "Codigo", "Disciplina.Nome");
            ViewData["Matricula"] = new SelectList(_context.Matricula
                .Where(a => a.RM == _matricula.RM)
                , "RM", "RM");
            return View(avaliacao);
        }

        public async Task<ActionResult> Dispensado(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var avaliacao = await _context.Avaliacao.FindAsync(id);
            if (avaliacao == null)
            {
                return NotFound();
            }
            try
            {
                avaliacao.Finalizado = true;
                _context.Update(avaliacao);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AvaliacaoExists(avaliacao.Codigo))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

        }

        // GET: Avaliacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacao.FindAsync(id);
            if (avaliacao == null)
            {
                return NotFound();
            }
            ViewData["DisciTurmaId"] = new SelectList(_context.Disci_Tuma, "Codigo", "Codigo", avaliacao.DisciTurmaId);
            ViewData["MatriculaId"] = new SelectList(_context.Matricula, "RM", "RM", avaliacao.MatriculaId);
            return View(avaliacao);
        }

        // POST: Avaliacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codigo,Data,DisciTurmaId,MatriculaId")] Avaliacao avaliacao)
        {
            if (id != avaliacao.Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(avaliacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvaliacaoExists(avaliacao.Codigo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DisciTurmaId"] = new SelectList(_context.Disci_Tuma, "Codigo", "Codigo", avaliacao.DisciTurmaId);
            ViewData["MatriculaId"] = new SelectList(_context.Matricula, "RM", "RM", avaliacao.MatriculaId);
            return View(avaliacao);
        }

        /*
        // GET: Avaliacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Disci_Turma)
                .Include(a => a.Matricula)
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (avaliacao == null)
            {
                return NotFound();
            }

            return View(avaliacao);
        }

        
        // POST: Avaliacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var avaliacao = await _context.Avaliacao.FindAsync(id);
            _context.Avaliacao.Remove(avaliacao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        */

        private bool AvaliacaoExists(int id)
        {
            return _context.Avaliacao.Any(e => e.Codigo == id);
        }
    }
}
