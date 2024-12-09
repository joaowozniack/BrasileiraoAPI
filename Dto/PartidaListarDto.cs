using BrasileiraoAPI.Models;

namespace BrasileiraoAPI.Dto
{
    public class PartidaListarDto
    {
        public string Status { get; set; }
        public string TimeCasa { get; set; }
        public string TimeVisitante { get; set; }
        public int GolsTimeCasa { get; set; }
        public int GolsTimeVisitante { get; set; }
        public string Placar => $"{GolsTimeCasa} x {GolsTimeVisitante}";
        public string Data { get; set; }
        public string Estadio { get; set; }
    }
}
