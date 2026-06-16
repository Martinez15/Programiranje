namespace TaxiPrevoz.Models
{
    public class Vozilo
    {
        public int VoziloID { get; set; }
        public string Registracija { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public int Godiste { get; set; }
        public string Boja { get; set; }
        public bool Aktivno { get; set; }
    }
}
