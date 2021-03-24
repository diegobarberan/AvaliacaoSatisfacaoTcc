using AvaliacaoDesempenho.Data;
using AvaliacaoDesempenho.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoDesempenho.Controllers
{
    [Authorize(Roles = "Aluno")]
    public class AlunoMatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public AlunoMatriculasController(ApplicationDbContext context, UserManager<Usuario> useManager)
        {
            _context = context;
            _userManager = useManager;
        }
        public async Task<IActionResult> Edit()
        {
            var email = User.Identity.Name;

            if (email == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matricula
                .Include(m => m.Turma)
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.EmailInst == email);
            if (matricula == null)
            {
                return View("Views/AlunoMatriculas/Erro.cshtml");
            }

            return View(matricula);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Matricula matricula)
        {
            if (matricula.RM != null)
            {
                try
                {                   
                    var _matricula = await _context.Matricula.FirstOrDefaultAsync(m => m.RM == matricula.RM);
                    _matricula.UsuarioId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                    _context.Update(_matricula);
                    await _context.SaveChangesAsync();

                    //Criar Avaliações
                    var _disciTurma = _context.Disci_Tuma
                    .Include(a => a.Turma)
                    .Where(a => a.TurmaId == _matricula.TurmaId)
                    .Include(a => a.Disciplina).ToList();

                    foreach (var item in _disciTurma)
                    {
                        if (_context.Avaliacao
                            .Where(a => a.DisciTurmaId == item.Codigo)
                            .Where(a => a.MatriculaId == _matricula.RM).Any())
                        {
                            continue;
                        }                        
                        _context.Add(new Avaliacao
                        {
                            Data = DateTime.Now,
                            MatriculaId = _matricula.RM,
                            Finalizado = false,
                            DisciTurmaId = item.Codigo
                        });
                        await _context.SaveChangesAsync();
                    }

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

                return RedirectToAction("Index", "Avaliacoes");
            }

            return View("Views/Home/Index.cshtml");
        }

        private bool MatriculaExists(string id)
        {
            return _context.Matricula.Any(e => e.RM == id);
        }



    }
}
