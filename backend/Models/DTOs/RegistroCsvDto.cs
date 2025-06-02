using CsvHelper.Configuration.Attributes;

public class RegistroCsvDto
{
    [Name("cnpj")]
    public string Cnpj { get; set; }

    [Name("razao_social")]
    public string RazaoSocial { get; set; }

    [Name("numero_nota")]
    public string NumeroNota { get; set; }

    [Name("data_emissao")]
    public DateTime DataEmissao { get; set; }

    [Name("codigo_item")]
    public int CodigoItem { get; set; }

    [Name("descricao_item")]
    public string DescricaoItem { get; set; }

    [Name("quantidade")]
    public int Quantidade { get; set; }

    [Name("valor_unitario")]
    public decimal ValorUnitario { get; set; }

    [Name("imposto_item")]
    public decimal ImpostoItem { get; set; }
}