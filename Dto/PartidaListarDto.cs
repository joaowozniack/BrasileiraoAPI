using BrasileiraoAPI.Models;

namespace BrasileiraoAPI.Dto
{
    public class PartidaListarDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int Rodada { get; set; }
        public string TimeCasa { get; set; }
        public string TimeVisitante { get; set; }
        public int GolsTimeCasa { get; set; }
        public int GolsTimeVisitante { get; set; }
        public string Data { get; set; }
        public string Estadio { get; set; }
        public string EscudoTimeCasa { get; set; }
        public string EscudoTimeVisitante { get; set; }
    }
}
