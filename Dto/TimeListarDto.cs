
namespace BrasileiraoAPI.Dto
{
    public class TimeListarDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sigla { get; set; }
        public string Escudo { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string AnoFundacao { get; set; }

        public int Jogos { get; set; }
        public int Pontos { get; set; }
        public int Vitorias { get; set; }
        public int Empates { get; set; }
        public int Derrotas { get; set; }
        public int GolsPro { get; set; }
        public int GolsContra { get; set; }
        public int SaldoGols => GolsPro - GolsContra;
    }
}
