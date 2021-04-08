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
    [Authorize(Roles = "Coordenador")]
    public class Disci_TurmaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Disci_TurmaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [TempData]
        public int? CodTurma { get; set; }

        [ViewData]
        public string NomeTurma { get; set; }



        // GET: Disci_Turma
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Para rodar os teste é necessario comentar o os atributos CodTurma e NomeTurma
            CodTurma = id;            
            NomeTurma = _context.Turma.Find(id).Nome;
            TempData.Keep("CodTurma");
            var applicationDbContext = _context.Disci_Tuma
                .Include(d => d.Disciplina)
                .Include(d => d.Professor)
                .Include(d => d.Turma)
                .Where(d => d.TurmaId == id)
                .OrderBy(a => a.Disciplina.Nome);

            ViewData["TurmaCod"] = id;
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> GerarPDF(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
           
            NomeTurma = _context.Turma.Find(id).Nome;
            var applicationDbContext = _context.Disci_Tuma
                .Include(d => d.Disciplina)
                .Include(d => d.Professor)
                .Include(d => d.Turma)
                .Where(d => d.TurmaId == id)
                .OrderBy(a => a.Disciplina.Nome);            
            return View(await applicationDbContext.ToListAsync());
        }



        // GET: Disci_Turma/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var disci_Turma = await _context.Disci_Tuma
                .Include(d => d.Disciplina)
                .Include(d => d.Professor)
                .Include(d => d.Turma)
                .FirstOrDefaultAsync(m => m.Codigo == id);
            //Para rodar os teste é necessario comentar o os atributos CodTurma e NomeTurma
            CodTurma = disci_Turma.TurmaId;
            TempData.Keep("CodTurma");
            if (disci_Turma == null)
            {
                return NotFound();
            }

            return View(disci_Turma);
        }

        // GET: Disci_Turma/Create
        public IActionResult Create()
        {

            ViewData["DisciplinaId"] = new SelectList(_context.Disciplina.OrderBy(a => a.Nome), "Codigo", "Nome");
            ViewData["ProfessorId"] = new SelectList(_context.Professor.OrderBy(a => a.Nome), "Codigo", "Nome");
            ViewData["TurmaId"] = new SelectList(_context.Turma.Where(d => d.Codigo == CodTurma), "Codigo", "Nome");
            TempData.Keep("CodTurma");            
            return View();

        }

        // POST: Disci_Turma/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,TurmaId,DisciplinaId,ProfessorId")] Disci_Turma disci_Turma)
        {
            //Para rodar os teste é necessario comentar o os atributos CodTurma
            CodTurma = disci_Turma.TurmaId;
            TempData.Keep("CodTurma");
            if (ModelState.IsValid)
            {
                var _disci_Turma = _context.Disci_Tuma
                .Where(d => d.TurmaId == disci_Turma.TurmaId)
                .Where(d => d.DisciplinaId == disci_Turma.DisciplinaId)
                .ToListAsync();
                if (_disci_Turma.Result.Count == 0)
                {
                    _context.Add(disci_Turma);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { id = disci_Turma.TurmaId });
                }
                ViewData["Falha"] = true;
            }

            ViewData["DisciplinaId"] = new SelectList(_context.Disciplina.OrderBy(a => a.Nome), "Codigo", "Nome", disci_Turma.DisciplinaId);
            ViewData["ProfessorId"] = new SelectList(_context.Professor.OrderBy(a => a.Nome), "Codigo", "Nome", disci_Turma.ProfessorId);
            ViewData["TurmaId"] = new SelectList(_context.Turma.Where(d => d.Codigo == disci_Turma.TurmaId), "Codigo", "Nome");
            return View(disci_Turma);
        }

        // GET: Disci_Turma/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }


            var disci_Turma = await _context.Disci_Tuma.FindAsync(id);
            CodTurma = disci_Turma.TurmaId;
            TempData.Keep("CodTurma");
            if (disci_Turma == null)
            {
                return NotFound();
            }
            ViewData["DisciplinaId"] = new SelectList(_context.Disciplina.OrderBy(a => a.Nome), "Codigo", "Nome", disci_Turma.DisciplinaId);
            ViewData["ProfessorId"] = new SelectList(_context.Professor.OrderBy(a => a.Nome), "Codigo", "Nome", disci_Turma.ProfessorId);
            ViewData["TurmaId"] = new SelectList(_context.Turma.Where(d => d.Codigo == disci_Turma.TurmaId), "Codigo", "Nome");
            return View(disci_Turma);
        }

        // POST: Disci_Turma/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codigo,TurmaId,DisciplinaId,ProfessorId")] Disci_Turma disci_Turma)
        {
            CodTurma = disci_Turma.TurmaId;
            if (id != disci_Turma.Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(disci_Turma);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Disci_TurmaExists(disci_Turma.Codigo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }                
                return RedirectToAction(nameof(Index), new { id = disci_Turma.TurmaId });

            }
            ViewData["DisciplinaId"] = new SelectList(_context.Disciplina.OrderBy(a => a.Nome), "Codigo", "Nome", disci_Turma.DisciplinaId);
            ViewData["ProfessorId"] = new SelectList(_context.Professor.OrderBy(a => a.Nome), "Codigo", "Nome", disci_Turma.ProfessorId);
            ViewData["TurmaId"] = new SelectList(_context.Turma.Where(d => d.Codigo == disci_Turma.TurmaId), "Codigo", "Nome");
            return View(disci_Turma);
        }

        // GET: Disci_Turma/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disci_Turma = await _context.Disci_Tuma
                .Include(d => d.Disciplina)
                .Include(d => d.Professor)
                .Include(d => d.Turma)
                .FirstOrDefaultAsync(m => m.Codigo == id);
            CodTurma = disci_Turma.TurmaId;
            if (disci_Turma == null)
            {
                return NotFound();
            }


            return View(disci_Turma);
        }

        // POST: Disci_Turma/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var disci_Turma = await _context.Disci_Tuma.FindAsync(id);
            CodTurma = disci_Turma.TurmaId;
            _context.Disci_Tuma.Remove(disci_Turma);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = disci_Turma.TurmaId });
        }

        private bool Disci_TurmaExists(int id)
        {
            return _context.Disci_Tuma.Any(e => e.Codigo == id);
        }
    }
}
