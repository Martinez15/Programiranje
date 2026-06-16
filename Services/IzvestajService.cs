using System;
using System.Collections.Generic;
using System.Linq;
using TaxiPrevoz.Models;
using TaxiPrevoz.Repositories;

namespace TaxiPrevoz.Services
{
    public class IzvestajService
    {
        private readonly VoznjaRepository voznjaRepo;
        private readonly IRepository<Korisnik> korisnikRepo;

        public IzvestajService(VoznjaRepository voznjaRepo, IRepository<Korisnik> korisnikRepo)
        {
            this.voznjaRepo = voznjaRepo;
            this.korisnikRepo = korisnikRepo;
        }

        public decimal GetUkupanPrihod(DateTime odDatuma, DateTime doDatuma)
        {
            return voznjaRepo.GetAll()
                .Where(v => v.Status == StatusVoznje.Zavrsena && 
                            v.VremeDolaska.HasValue && 
                            v.VremeDolaska.Value.Date >= odDatuma.Date && 
                            v.VremeDolaska.Value.Date <= doDatuma.Date)
                .Sum(v => v.Cena);
        }

        public List<VozacStatistika> GetStatistikaVozaca(DateTime odDatuma, DateTime doDatuma)
        {
            var voznje = voznjaRepo.GetAll()
                .Where(v => v.Status == StatusVoznje.Zavrsena && 
                            v.VremeDolaska.HasValue && 
                            v.VremeDolaska.Value.Date >= odDatuma.Date && 
                            v.VremeDolaska.Value.Date <= doDatuma.Date)
                .ToList();

            var vozaci = korisnikRepo.GetAll().OfType<Vozac>().ToList();

            var stats = new List<VozacStatistika>();
            foreach (var vozac in vozaci)
            {
                var voznjeVozaca = voznje.Where(v => v.VozacID == vozac.VozacID).ToList();
                stats.Add(new VozacStatistika
                {
                    VozacID = vozac.VozacID,
                    ImePrezime = vozac.ImePrezime,
                    BrojVoznji = voznjeVozaca.Count,
                    UkupnaZarada = voznjeVozaca.Sum(v => v.Cena),
                    ProsecnaOcena = vozac.ProsecnaOcena
                });
            }

            return stats.OrderByDescending(s => s.UkupnaZarada).ToList();
        }

        public List<PutnikStatistika> GetTopPutnici(DateTime odDatuma, DateTime doDatuma)
        {
            var voznje = voznjaRepo.GetAll()
                .Where(v => v.Status == StatusVoznje.Zavrsena && 
                            v.VremeDolaska.HasValue && 
                            v.VremeDolaska.Value.Date >= odDatuma.Date && 
                            v.VremeDolaska.Value.Date <= doDatuma.Date)
                .ToList();

            var putnici = korisnikRepo.GetAll().OfType<Putnik>().ToList();

            var stats = new List<PutnikStatistika>();
            foreach (var putnik in putnici)
            {
                var voznjePutnika = voznje.Where(v => v.PutnikID == putnik.PutnikID).ToList();
                if (voznjePutnika.Count > 0)
                {
                    stats.Add(new PutnikStatistika
                    {
                        PutnikID = putnik.PutnikID,
                        ImePrezime = putnik.ImePrezime,
                        BrojVoznji = voznjePutnika.Count,
                        UkupnoPotroseno = voznjePutnika.Sum(v => v.Cena)
                    });
                }
            }

            return stats.OrderByDescending(s => s.BrojVoznji).Take(10).ToList();
        }

        public OpstaStatistika GetOpstaStatistika()
        {
            var voznje = voznjaRepo.GetAll();
            var korisnici = korisnikRepo.GetAll();

            int ukupnoVoznji = voznje.Count;
            int zavrsenih = voznje.Count(v => v.Status == StatusVoznje.Zavrsena);
            int otkazanih = voznje.Count(v => v.Status == StatusVoznje.Otkazana);
            int uTokuAktivno = voznje.Count(v => v.Status == StatusVoznje.UToku || v.Status == StatusVoznje.Narucena || v.Status == StatusVoznje.Prihvacena);

            var vozaci = korisnici.OfType<Vozac>().ToList();
            double prosecnaOcenaSvih = vozaci.Count > 0 ? vozaci.Average(v => v.ProsecnaOcena) : 0;

            return new OpstaStatistika
            {
                UkupnoVoznji = ukupnoVoznji,
                ZavrsenoVoznji = zavrsenih,
                OtkazanoVoznji = otkazanih,
                AktivnoVoznji = uTokuAktivno,
                ProsecnaOcenaVozaca = prosecnaOcenaSvih,
                BrojRegistrovanihVozaca = vozaci.Count,
                BrojRegistrovanihPutnika = korisnici.OfType<Putnik>().Count()
            };
        }
    }

    public class VozacStatistika
    {
        public string VozacID { get; set; }
        public string ImePrezime { get; set; }
        public int BrojVoznji { get; set; }
        public decimal UkupnaZarada { get; set; }
        public double ProsecnaOcena { get; set; }
    }

    public class PutnikStatistika
    {
        public string PutnikID { get; set; }
        public string ImePrezime { get; set; }
        public int BrojVoznji { get; set; }
        public decimal UkupnoPotroseno { get; set; }
    }

    public class OpstaStatistika
    {
        public int UkupnoVoznji { get; set; }
        public int ZavrsenoVoznji { get; set; }
        public int OtkazanoVoznji { get; set; }
        public int AktivnoVoznji { get; set; }
        public double ProsecnaOcenaVozaca { get; set; }
        public int BrojRegistrovanihVozaca { get; set; }
        public int BrojRegistrovanihPutnika { get; set; }
    }
}
