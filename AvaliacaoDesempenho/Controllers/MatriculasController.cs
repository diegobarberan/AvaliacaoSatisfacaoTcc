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
namespace AvaliacaoDesempenho.Controllers
{
    [Authorize(Roles = "Coordenador,Professor/Coord")]
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MatriculasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Matriculas
        public IActionResult Index(int? id)
        {
            ViewData["Turma"] = new SelectList(_context.Turma.OrderBy(a => a.Nome), "Codigo", "Nome");

            if (id != null)
            {
                var applicationDbContext = _context.Matricula
                .Include(m => m.Turma)
                .Include(m => m.Usuario)
                .Where(m => m.TurmaId == id)
                .OrderBy(a => a.EmailInst);
                ViewData["Turma"] = new SelectList(_context.Turma.OrderBy(a => a.Nome), "Codigo", "Nome", id);
                ViewData["TurmaId"] = id;
                return View(applicationDbContext.ToList());
            }
            return View();
        }

        public IActionResult GerarPDF(int? id)
        {

            if (id != null)
            {
                var applicationDbContext = _context.Matricula
                .Include(m => m.Turma)
                .Include(m => m.Usuario)
                .Where(m => m.TurmaId == id)
                .OrderBy(a => a.EmailInst);
                ViewData["Turma"] = new SelectList(_context.Turma.OrderBy(a => a.Nome), "Codigo", "Nome", id);
                ViewData["TurmaId"] = id;
                return View(applicationDbContext.ToList());
            }
            return View();
        }

        public IActionResult TotalRespondido()
        {
            ViewData["TotalRespondido"] = _context.Matricula.Where(a => a.ControleAvaliacao == true).Count();
            ViewData["TotalNaoRespondido"] = _context.Matricula.Where(a => a.ControleAvaliacao == false).Count();
            return View();


        }

        public IActionResult PorTurma(int? id)
        {
            ViewData["Turma"] = new SelectList(_context.Turma.OrderBy(a => a.Nome), "Codigo", "Nome");

            if (id != null)
            {
                if (_context.Matricula
                    .Where(a => a.TurmaId == id).Count() > 0)
                {
                    ViewData["TotalRespondido"] = _context.Matricula
                        .Where(a => a.TurmaId == id)
                        .Where(a => a.ControleAvaliacao == true).Count();
                    ViewData["TotalNaoRespondido"] = _context.Matricula
                        .Where(a => a.TurmaId == id)
                        .Where(a => a.ControleAvaliacao == false).Count();
                    ViewData["NomeTurma"] = _context.Turma.Where(a => a.Codigo == id).First().Nome;
                    var matricula = _context.Matricula.OrderBy(a => a.EmailInst).Where(a => a.TurmaId == id).ToList();
                    ViewData["Turma"] = new SelectList(_context.Turma.OrderBy(a => a.Nome), "Codigo", "Nome", id);
                    return View(matricula);
                }
            }
            return View();
        }

        // GET: Matriculas/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matricula
                .Include(m => m.Turma)
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.RM == id);
            if (matricula == null)
            {
                return NotFound();
            }

            return View(matricula);
        }


        // GET: Matriculas/Create
        [Authorize(Roles = "Coordenador")]
        public IActionResult Create()
        {
            ViewData["TurmaId"] = new SelectList(_context.Turma.OrderBy(a => a.Nome), "Codigo", "Nome");
            return View();
        }

        // POST: Matriculas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Coordenador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RM,EmailInst,Data,ControleAvaliacao,TurmaId")] Matricula matricula)
        {
            if (ModelState.IsValid)
            {
                _context.Add(matricula);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TurmaId"] = new SelectList(_context.Turma.OrderBy(a => a.Nome), "Codigo", "Nome", matricula.TurmaId);
            return View(matricula);
        }

        // GET: Matriculas/Edit/5
        [Authorize(Roles = "Coordenador")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matricula.FindAsync(id);
            if (matricula == null)
            {
                return NotFound();
            }
            ViewData["TurmaId"] = new SelectList(_context.Turma.OrderBy(a => a.Nome), "Codigo", "Nome", matricula.TurmaId);
            return View(matricula);
        }

        // POST: Matriculas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Coordenador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("RM,EmailInst,Data,ControleAvaliacao,TurmaId")] Matricula matricula)
        {
            if (id != matricula.RM)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(matricula);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatriculaExists(matricula.RM))
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
            ViewData["TurmaId"] = new SelectList(_context.Turma.OrderBy(a => a.Nome), "Codigo", "Nome", matricula.TurmaId);
            return View(matricula);
        }

        // GET: Matriculas/Delete/5
        [Authorize(Roles = "Coordenador")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matricula
                .Include(m => m.Turma)
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.RM == id);
            if (matricula == null)
            {
                return NotFound();
            }

            return View(matricula);
        }

        // POST: Matriculas/Delete/5
        [Authorize(Roles = "Coordenador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var matricula = await _context.Matricula.FindAsync(id);
            _context.Matricula.Remove(matricula);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatriculaExists(string id)
        {
            return _context.Matricula.Any(e => e.RM == id);
        }
    }
}
