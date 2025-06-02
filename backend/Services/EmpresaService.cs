using Microsoft.EntityFrameworkCore;
using TributoJustoBackend.Data;
using TributoJustoBackend.Models.Entities;

namespace TributoJustoBackend.Services
{
    public class EmpresaService
    {
        private readonly AppDbContext _contexto;

        public EmpresaService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Empresa>> ListarEmpresasAsync()
        {
            return await _contexto.Empresas.ToListAsync();
        }

        public async Task<Empresa?> ObterEmpresaPorCnpjAsync(string cnpj)
        {
            var cnpjLimpo = LimparCnpj(cnpj);
            return await BuscarEmpresaPorCnpjLimpoAsync(cnpjLimpo);
        }

        private string LimparCnpj(string cnpj)
        {
            return new string(cnpj.Where(char.IsDigit).ToArray());
        }

        private async Task<Empresa?> BuscarEmpresaPorCnpjLimpoAsync(string cnpjLimpo)
        {
            return await _contexto.Empresas
                .FirstOrDefaultAsync(e => e.Cnpj == cnpjLimpo);
        }
    }
}
