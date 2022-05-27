using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using TestBetaApi.API.Extensions;
using TestBetaApi.API.ViewModels;
using TestBetaApi.Business.Intefaces;

namespace TestBetaApi.API.Controllers
{
    [Route("api")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;

        public AuthController(SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              IOptions<AppSettings> appSettings,
                              INotificador notificador) : base(notificador)
        {
            _signManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }


        [HttpPost("cadastrar")]
        public async Task<ActionResult> Registrar(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if(result.Succeeded)
            {
                await _signManager.SignInAsync(user, false);
                //return CustomResponse(registerUser);
                return CustomResponse(GerarJwt());
            }

            foreach(var error in result.Errors)
            {
                NotificarErro(error.Description);
            }

            return CustomResponse(registerUser);
        }

        [HttpPost("autenticar")]
        public async Task<ActionResult> Login(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _signManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if(result.Succeeded)
            {
                //return CustomResponse(loginUser);
                return CustomResponse(GerarJwt());
            }

            if(result.IsLockedOut)
            {
                NotificarErro("Usuário temporariamente bloqueado por tentativas invalidas");
                return CustomResponse(loginUser);
            }

            NotificarErro("Usuario ou senha incorretos");
            return CustomResponse(loginUser);
        }

        private string GerarJwt()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodeToken = tokenHandler.WriteToken(token);

            return encodeToken;
        }
    }
}
