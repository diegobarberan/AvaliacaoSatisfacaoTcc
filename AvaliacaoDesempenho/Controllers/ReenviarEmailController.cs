using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AvaliacaoDesempenho.Data;
using AvaliacaoDesempenho.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace AvaliacaoDesempenho.Controllers
{
    public class ReenviarEmailController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public ReenviarEmailController(
            UserManager<Usuario> userManager,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _context = context;
        }
        
            public IActionResult Index()
        {
            return View();
        }

        // POST: Professors/Delete/5
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailConfirmed(string email, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (email == null)
            {
                ViewData["Mensagem"] = "Você não digitou um email";
                return View();
            }
            
            if(!UsuarioExists(email))
            {
                ViewData["Mensagem"] = "Nenhuma conta registrada com este e-mail " + email;
                return View();
            }
            
            var user = await _context.Users.FirstAsync(a => a.Email == email);

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                ViewData["Mensagem"] = "Você já confirmou seu email.";
                return View();
            }
            
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);


            await _emailSender.SendEmailAsync(user.Email, "Confirme seu email",
                $"Por favor, confirme sua conta no site de avaliação de satisfação 2020 " +
                $"da Etec Taubaté <a href = '{HtmlEncoder.Default.Encode(callbackUrl)}'> clicando aqui </ a >.");
            
            return RedirectToAction(nameof(ReenvioConfirmado));
        }

        public IActionResult ReenvioConfirmado()
        {
            return View();
        }


        private bool UsuarioExists(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }
    }
}
