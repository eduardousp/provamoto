public class Locacao
{
    public int Id { get; set; }
    public int EntregadorId { get; set; }
    public int MotoId { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataPrevisaoTermino { get; set; }
    public DateTime? DataDevolucao { get; set; }
    public PlanoLocacao Plano { get; set; }
}

public class LocacaoDto
{
    public int EntregadorId { get; set; }
    public int MotoId { get; set; }
    public PlanoLocacao Plano { get; set; }
}

public enum PlanoLocacao
{
    SeteDias = 7,
    QuinzeDias = 15,
    TrintaDias = 30,
    QuarentaCincoDias = 45,
    CinquentaDias = 50
}