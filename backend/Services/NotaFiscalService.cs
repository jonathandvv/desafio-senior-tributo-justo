using Microsoft.EntityFrameworkCore;
using TributoJustoBackend.Data;
using TributoJustoBackend.Models.Entities;

namespace TributoJustoBackend.Services
{
    public class NotaFiscalService
    {
        private readonly AppDbContext _contexto;

        public NotaFiscalService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<NotaFiscal>> ListarPorEmpresaAsync(int empresaId)
        {
            var query = ObterQueryNotasPorEmpresa(empresaId);
            return await query.ToListAsync();
        }

        private IQueryable<NotaFiscal> ObterQueryNotasPorEmpresa(int empresaId)
        {
            return _contexto.NotasFiscais.Where(n => n.EmpresaId == empresaId);
        }
    }
}
