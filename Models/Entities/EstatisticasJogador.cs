namespace BrasileiraoAPI.Models.Entities
{
    public class EstatisticasJogador
    {
        public int Id { get; set; }
        public Jogador Jogador { get; set; }
        public int Jogos { get; set; }
        public int Gols { get; set; }
        public int Assistencias { get; set; }
        public int CartoesAmarelos { get; set; }
        public int CartoesVermelhos { get; set; }
    }
}
