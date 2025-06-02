namespace TributoJustoBackend.ExternalServices
{
    public interface IClienteIA
    {
        Task<string> AnalisarTextoAsync(string prompt);
    }
}
