using System;

namespace TaxiPrevoz.Models
{
    public class Voznja
    {
        public string VoznjaID { get; set; }  // VOZ-2026-0001
        public string PutnikID { get; set; }
        public string VozacID { get; set; }
        public string Polaziste { get; set; }
        public string Odrediste { get; set; }
        public DateTime VremeNarucivanja { get; set; }
        public DateTime? VremePolaska { get; set; }
        public DateTime? VremeDolaska { get; set; }
        public double Kilometraza { get; set; }
        public decimal Cena { get; set; }
        public StatusVoznje Status { get; set; }
        public int? Ocena { get; set; }
        public string Komentar { get; set; }

        public bool MozeSeOtkazati() =>
            Status == StatusVoznje.Narucena ||
            Status == StatusVoznje.Prihvacena;
    }
}
