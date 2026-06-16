using TaxiPrevoz.Models;
using TaxiPrevoz.Repositories;
using TaxiPrevoz.Services;

namespace TaxiPrevoz
{
    public static class AppState
    {
        public static KorisnikRepository KorisnikRepo { get; } = new KorisnikRepository();
        public static VozacRepository VozacRepo { get; } = new VozacRepository();
        public static VoziloRepository VoziloRepo { get; } = new VoziloRepository();
        public static PutnikRepository PutnikRepo { get; } = new PutnikRepository();
        public static VoznjaRepository VoznjaRepo { get; } = new VoznjaRepository();
        public static CenovnikRepository CenovnikRepo { get; } = new CenovnikRepository();

        public static AuthService AuthService { get; } = new AuthService(KorisnikRepo);
        public static LogService LogService { get; } = new LogService();
        public static CenovnikService CenovnikService { get; } = new CenovnikService(CenovnikRepo);
        public static VozacService VozacService { get; } = new VozacService(KorisnikRepo, VoziloRepo);
        public static VoznjaService VoznjaService { get; } = new VoznjaService(VoznjaRepo, KorisnikRepo, CenovnikRepo, LogService);
        public static OcenaService OcenaService { get; } = new OcenaService(VoznjaRepo, KorisnikRepo, LogService);
        public static IzvestajService IzvestajService { get; } = new IzvestajService(VoznjaRepo, KorisnikRepo);

        public static Korisnik TrenutniKorisnik { get; set; }
    }
}
