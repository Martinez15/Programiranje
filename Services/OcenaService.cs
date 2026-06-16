using System;
using System.Collections.Generic;
using System.Linq;
using TaxiPrevoz.Models;
using TaxiPrevoz.Repositories;

namespace TaxiPrevoz.Services
{
    public class OcenaService
    {
        private readonly VoznjaRepository voznjaRepo;
        private readonly IRepository<Korisnik> korisnikRepo;
        private readonly LogService logService;

        public OcenaService(VoznjaRepository voznjaRepo, IRepository<Korisnik> korisnikRepo, LogService logService)
        {
            this.voznjaRepo = voznjaRepo;
            this.korisnikRepo = korisnikRepo;
            this.logService = logService;
        }

        public void OceniVozaca(string voznjaId, int ocena, string komentar)
        {
            if (ocena < 1 || ocena > 5)
                throw new Exception("Ocena mora biti izmedju 1 i 5.");

            var voznje = voznjaRepo.GetAll();
            var voznja = voznje.FirstOrDefault(v => v.VoznjaID == voznjaId);

            if (voznja == null)
                throw new Exception("Voznja ne postoji.");

            if (voznja.Status != StatusVoznje.Zavrsena)
                throw new Exception("Samo zavrsene voznje se mogu oceniti.");

            if (voznja.Ocena.HasValue)
                throw new Exception("Ova voznja je vec ocenjena.");

            voznja.Ocena = ocena;
            voznja.Komentar = komentar;
            voznjaRepo.SaveAll(voznje);

            var korisnici = korisnikRepo.GetAll();
            var vozac = korisnici.OfType<Vozac>().FirstOrDefault(v => v.VozacID == voznja.VozacID);
            if (vozac != null)
            {
                double ukupnoOcenaSuma = vozac.ProsecnaOcena * vozac.BrojOcena;
                vozac.BrojOcena++;
                vozac.ProsecnaOcena = (ukupnoOcenaSuma + ocena) / vozac.BrojOcena;

                korisnikRepo.SaveAll(korisnici);
            }

            logService.Log($"Voznja {voznjaId} ocenjena ocenom {ocena}. Vozac {voznja.VozacID} ima novu prosecnu ocenu: {vozac?.ProsecnaOcena:0.00}.");
        }

        public List<Vozac> GetRangListaVozaca()
        {
            var korisnici = korisnikRepo.GetAll();
            return korisnici.OfType<Vozac>()
                .OrderByDescending(v => v.ProsecnaOcena)
                .ThenByDescending(v => v.BrojOcena)
                .ToList();
        }

        public List<Voznja> GetKomentariZaVozaca(string vozacId)
        {
            return voznjaRepo.GetAll()
                .Where(v => v.VozacID == vozacId && v.Ocena.HasValue)
                .OrderByDescending(v => v.VremeDolaska)
                .ToList();
        }
    }
}
