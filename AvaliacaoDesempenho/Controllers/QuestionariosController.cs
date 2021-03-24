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
    public class QuestionariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Questionarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Questionario.OrderBy(a => a.Codigo).ToListAsync());
        }

        public async Task<IActionResult> GerarPDF()
        {
            return View(await _context.Questionario.OrderBy(a => a.Codigo).ToListAsync());
        }

        // GET: Questionarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionario = await _context.Questionario
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (questionario == null)
            {
                return NotFound();
            }

            return View(questionario);
        }

        // GET: Questionarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Questionarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Descricao")] Questionario questionario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(questionario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(questionario);
        }

        // GET: Questionarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionario = await _context.Questionario.FindAsync(id);
            if (questionario == null)
            {
                return NotFound();
            }
            return View(questionario);
        }

        // POST: Questionarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codigo,Descricao")] Questionario questionario)
        {
            if (id != questionario.Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(questionario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionarioExists(questionario.Codigo))
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
            return View(questionario);
        }

        // GET: Questionarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionario = await _context.Questionario
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (questionario == null)
            {
                return NotFound();
            }

            return View(questionario);
        }

        // POST: Questionarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var questionario = await _context.Questionario.FindAsync(id);
            _context.Questionario.Remove(questionario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionarioExists(int id)
        {
            return _context.Questionario.Any(e => e.Codigo == id);
        }
    }
}
