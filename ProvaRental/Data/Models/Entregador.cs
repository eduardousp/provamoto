public class Entregador
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string CNPJ { get; set; }
    public string CNHNumero { get; set; }
    public string CNHTipo { get; set; } // A, B ou A+B
    public DateTime DataNascimento { get; set; }
    public string ImagemCNH { get; set; } // URL ou caminho de arquivo
}
