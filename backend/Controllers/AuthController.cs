using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TributoJustoBackend.Data;
using TributoJustoBackend.Models.DTOs;
using TributoJustoBackend.Models.Entities;

namespace TributoJustoBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly AppDbContext _contexto;
        private readonly IConfiguration _configuracao;
        private readonly PasswordHasher<Usuario> _hashSenha = new();

        public AutenticacaoController(AppDbContext contexto, IConfiguration configuracao)
        {
            _contexto = contexto;
            _configuracao = configuracao;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] CredenciaisUsuarioDto credenciais)
        {
            var existeUsuario = await _contexto.Usuarios.AnyAsync(u => u.NomeUsuario == credenciais.Usuario);
            if (existeUsuario)
                return BadRequest("Usuário já existe.");

            var novoUsuario = new Usuario { NomeUsuario = credenciais.Usuario };
            novoUsuario.SenhaHash = _hashSenha.HashPassword(novoUsuario, credenciais.Senha);

            _contexto.Usuarios.Add(novoUsuario);
            await _contexto.SaveChangesAsync();

            var token = GerarTokenJwt(novoUsuario.NomeUsuario);
            return Ok(new { token });
        }

        [HttpPost("entrar")]
        public async Task<IActionResult> Entrar([FromBody] CredenciaisUsuarioDto credenciais)
        {
            var usuario = await _contexto.Usuarios
                .FirstOrDefaultAsync(u => u.NomeUsuario == credenciais.Usuario);

            if (usuario == null)
                return Unauthorized("Usuário ou senha inválidos.");

            var resultadoVerificacao = _hashSenha.VerifyHashedPassword(
                usuario,
                usuario.SenhaHash,
                credenciais.Senha
            );

            if (resultadoVerificacao == PasswordVerificationResult.Failed)
                return Unauthorized("Usuário ou senha inválidos.");

            var token = GerarTokenJwt(usuario.NomeUsuario);
            return Ok(new { token });
        }

        private string GerarTokenJwt(string nomeUsuario)
        {
            var chave = Encoding.ASCII.GetBytes(_configuracao["Jwt:ChaveSecreta"]);
            var manipulador = new JwtSecurityTokenHandler();

            var descricao = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, nomeUsuario) }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(chave),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = manipulador.CreateToken(descricao);
            return manipulador.WriteToken(token);
        }
    }
}