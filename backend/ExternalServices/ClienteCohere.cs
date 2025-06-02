using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace TributoJustoBackend.ExternalServices
{
    public class ClienteCohere : IClienteIA
    {
        private readonly HttpClient _httpClient;
        private readonly string _urlApi;
        private readonly string _chaveApi;

        public ClienteCohere(HttpClient httpClient, IConfiguration configuracao)
        {
            _httpClient = httpClient;
            _urlApi = configuracao["IA:CohereUrlApi"];
            _chaveApi = configuracao["IA:CohereApiKey"];
        }

        public async Task<string> AnalisarTextoAsync(string textoEntrada)
        {
            var jsonRequisicao = ConstruirJsonRequisicao(textoEntrada);
            var requisicao = CriarHttpRequest(jsonRequisicao);

            using var cancelamento = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            var resposta = await EnviarRequisicaoAsync(requisicao, cancelamento.Token);
            var conteudo = await LerConteudoRespostaAsync(resposta);

            if (!resposta.IsSuccessStatusCode)
                return MontarMensagemErro(resposta.StatusCode, conteudo);

            return ProcessarRespostaIA(conteudo);
        }

        private string ConstruirJsonRequisicao(string textoEntrada)
        {
            var dadosRequisicao = new
            {
                model = "command",
                prompt = textoEntrada,
                max_tokens = 500,
                temperature = 0.7,
                k = 0,
                stop_sequences = new[] { "\n" },
                return_likelihoods = "NONE"
            };

            return JsonSerializer.Serialize(dadosRequisicao);
        }

        private HttpRequestMessage CriarHttpRequest(string jsonRequisicao)
        {
            var requisicao = new HttpRequestMessage(HttpMethod.Post, _urlApi)
            {
                Content = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json")
            };

            requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _chaveApi);
            requisicao.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return requisicao;
        }

        private async Task<HttpResponseMessage> EnviarRequisicaoAsync(HttpRequestMessage requisicao, CancellationToken token)
        {
            return await _httpClient.SendAsync(requisicao, token);
        }

        private async Task<string> LerConteudoRespostaAsync(HttpResponseMessage resposta)
        {
            return await resposta.Content.ReadAsStringAsync();
        }

        private string MontarMensagemErro(System.Net.HttpStatusCode statusCode, string conteudo)
        {
            return $"Erro ao chamar a IA ({statusCode}): {conteudo}";
        }

        private string ProcessarRespostaIA(string conteudo)
        {
            try
            {
                using var documento = JsonDocument.Parse(conteudo);
                var raiz = documento.RootElement;

                if (raiz.TryGetProperty("generations", out var geracoes) && geracoes.ValueKind == JsonValueKind.Array)
                {
                    var primeira = geracoes[0];
                    if (primeira.TryGetProperty("text", out var texto))
                        return texto.GetString() ?? conteudo;
                }

                return conteudo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar resposta da IA: {ex.Message}");
                return conteudo;
            }
        }
    }
}
