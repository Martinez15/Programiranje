namespace TaxiPrevoz.Models
{
    public interface IDozvola
    {
        // -- Vozaci i vozila --------------------------------
        bool MozeUpravljatiVozacima();       // samo admin
        bool MozeUpravljatiVozilima();       // samo admin

        // -- Voznje -----------------------------------------
        bool MozeNarucitiVoznju();           // putnik + admin
        bool MozeOtkazatiVoznju();           // putnik + admin
        bool MozePrihvatitiVoznju();         // vozac + admin
        bool MozeMenjatiStatusVoznje();      // vozac + admin
        bool MozeVidetiSveVoznje();          // admin
        bool MozeVidetiSopstveneVoznje();    // vozac + putnik

        // -- Ocenjivanje ------------------------------------
        bool MozeOcenitiVozaca();            // putnik + admin
        bool MozeVidetiOcene();              // svi

        // -- Cenovnik i finansije ---------------------------
        bool MozeUpravljatiCenovnikom();     // samo admin
        bool MozeVidetiIzvestaje();          // samo admin
    }
}
