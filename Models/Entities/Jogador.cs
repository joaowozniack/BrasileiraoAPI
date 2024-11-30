namespace BrasileiraoAPI.Models.Entities
{
    public class Jogador
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string SobreNome { get; set; }
        public string Posicao { get; set; }
        public int Idade { get; set; }
        public Time Time { get; set; }
    }
}
