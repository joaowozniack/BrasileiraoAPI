namespace BrasileiraoAPI.Models.Entities
{
    public class Partida
    {
        public Guid Id { get; set; }
        public Time TimeCasa { get; set; }
        public Time TimeVisitante { get; set; }
        public int GolsTimeCasa { get; set; }
        public int GolsTimeVisitante { get; set; }
        public DateTime Data { get; set; }
        public string Estadio { get; set; }
    }
}
