namespace TaxiPrevoz.Models
{
    public abstract class Korisnik : IDozvola
    {
        public string KorisnickoIme { get; set; }
        public string PasswordHash { get; set; }
        public string Uloga { get; set; }

        public abstract bool MozeUpravljatiVozacima();
        public abstract bool MozeUpravljatiVozilima();
        public abstract bool MozeNarucitiVoznju();
        public abstract bool MozeOtkazatiVoznju();
        public abstract bool MozePrihvatitiVoznju();
        public abstract bool MozeMenjatiStatusVoznje();
        public abstract bool MozeVidetiSveVoznje();
        public abstract bool MozeVidetiSopstveneVoznje();
        public abstract bool MozeOcenitiVozaca();
        public abstract bool MozeVidetiOcene();
        public abstract bool MozeUpravljatiCenovnikom();
        public abstract bool MozeVidetiIzvestaje();
    }

    public class Admin : Korisnik
    {
        public override bool MozeUpravljatiVozacima() => true;
        public override bool MozeUpravljatiVozilima() => true;
        public override bool MozeNarucitiVoznju() => true;
        public override bool MozeOtkazatiVoznju() => true;
        public override bool MozePrihvatitiVoznju() => true;
        public override bool MozeMenjatiStatusVoznje() => true;
        public override bool MozeVidetiSveVoznje() => true;
        public override bool MozeVidetiSopstveneVoznje() => true;
        public override bool MozeOcenitiVozaca() => true;
        public override bool MozeVidetiOcene() => true;
        public override bool MozeUpravljatiCenovnikom() => true;
        public override bool MozeVidetiIzvestaje() => true;
    }

    public class Vozac : Korisnik
    {
        public string VozacID { get; set; }  // VOZ-0001
        public string ImePrezime { get; set; }
        public string BrojLicence { get; set; }
        public int VoziloID { get; set; }
        public bool Dostupan { get; set; }
        public double ProsecnaOcena { get; set; }
        public int BrojOcena { get; set; }

        public override bool MozeUpravljatiVozacima() => false;
        public override bool MozeUpravljatiVozilima() => false;
        public override bool MozeNarucitiVoznju() => false;
        public override bool MozeOtkazatiVoznju() => false;
        public override bool MozePrihvatitiVoznju() => true;
        public override bool MozeMenjatiStatusVoznje() => true;
        public override bool MozeVidetiSveVoznje() => false;
        public override bool MozeVidetiSopstveneVoznje() => true;
        public override bool MozeOcenitiVozaca() => false;
        public override bool MozeVidetiOcene() => true;
        public override bool MozeUpravljatiCenovnikom() => false;
        public override bool MozeVidetiIzvestaje() => false;
    }

    public class Putnik : Korisnik
    {
        public string PutnikID { get; set; }  // PUT-0001
        public string ImePrezime { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }

        public override bool MozeUpravljatiVozacima() => false;
        public override bool MozeUpravljatiVozilima() => false;
        public override bool MozeNarucitiVoznju() => true;
        public override bool MozeOtkazatiVoznju() => true;
        public override bool MozePrihvatitiVoznju() => false;
        public override bool MozeMenjatiStatusVoznje() => false;
        public override bool MozeVidetiSveVoznje() => false;
        public override bool MozeVidetiSopstveneVoznje() => true;
        public override bool MozeOcenitiVozaca() => true;
        public override bool MozeVidetiOcene() => true;
        public override bool MozeUpravljatiCenovnikom() => false;
        public override bool MozeVidetiIzvestaje() => false;
    }
}
