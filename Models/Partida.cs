namespace BrasileiraoAPI.Models
{
    public class Partida
    {
        public int Id { get; set; }
        public int Rodada { get; set; }
        public StatusPartida Status { get; set; }
        public Time TimeCasa { get; set; }
        public Time TimeVisitante { get; set; }
        public int GolsTimeCasa { get; set; }
        public int GolsTimeVisitante { get; set; }
        public string Data { get; set; }
        public string Estadio { get; set; }
    }
}
