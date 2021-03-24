using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using AvaliacaoDesempenho.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using AvaliacaoDesempenho.Data;

namespace AvaliacaoDesempenho.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;


        public RegisterModel(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Campo Obrigatório!")]
            [EmailAddress]
            [Display(Name = "E-mail Institucional (exemplo@hotmail.com)")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Campo Obrigatório!")]
            [DataType(DataType.Text)]
            [Display(Name = "Nome")]
            public string Nome { get; set; }

            [Required(ErrorMessage = "Campo Obrigatório!")]
            [StringLength(100, ErrorMessage = "O {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [RegularExpression(@"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "A Senha deve ter pelo menos 1 letra Maiúscula, 1 minúscula e 1 número.")]
            [Display(Name = "Senha - Alfanumérica com uma letra maiúscula - 6 caractere mínimo")]
            public string Senha { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar senha")]
            [Compare("Senha", ErrorMessage = "A SENHA e CONFIRMAR SENHA não correspondem.")]
            public string ConfirmSenha { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new Usuario { UserName = Input.Email, Email = Input.Email, Nome = Input.Nome };
                var result = await _userManager.CreateAsync(user, Input.Senha);
                if (result.Succeeded)
                {
                    _logger.LogInformation("O usuário criou uma nova conta com senha.");

                    //Lista de coordenadores
                    List<string> coord = new List<string>();                   
                    coord.Add("diego.barberan@etec.sp.gov.br");
                    coord.Add("coord-puc-tcc@hotmail.com");                    

                    //Lista de professores coordenador
                    List<string> profCoord = new List<string>();
                    profCoord.Add("profcoord-puc-tcc@hotmail.com");                    

                    if (coord.Any(a => a == user.Email))
                    {
                        await _userManager.AddToRoleAsync(user, "Coordenador");
                    }
                    else if (profCoord.Any(a => a == user.Email))
                    {
                        await _userManager.AddToRoleAsync(user, "Professor/Coord");
                    }
                    else if (_context.Professor.Any(a => a.Email == user.Email))
                    {
                        await _userManager.AddToRoleAsync(user, "Professor");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Aluno");
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirme seu email",
                        $"Por favor, confirme sua conta no site de avaliação de satisfação 2020 " +
                        $"da Etec Taubaté <a href = '{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");


                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
