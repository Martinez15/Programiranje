namespace TaxiPrevoz.Models
{
    public class Cenovnik
    {
        public decimal StartnaCena { get; set; }
        public decimal CenaPoKm { get; set; }
        public decimal NocnaDoplataCena { get; set; }

        public decimal ObracunajCenu(double km, bool nocna) =>
            StartnaCena + (decimal)km * CenaPoKm +
            (nocna ? NocnaDoplataCena : 0);
    }
}
