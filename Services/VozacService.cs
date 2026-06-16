using System;
using System.Collections.Generic;
using System.Linq;
using TaxiPrevoz.Models;
using TaxiPrevoz.Repositories;

namespace TaxiPrevoz.Services
{
    public class VozacService
    {
        private readonly IRepository<Korisnik> korisnikRepo;
        private readonly IRepository<Vozilo> voziloRepo;

        public VozacService(IRepository<Korisnik> korisnikRepo, IRepository<Vozilo> voziloRepo)
        {
            this.korisnikRepo = korisnikRepo;
            this.voziloRepo = voziloRepo;
        }

        public List<Vozac> GetSviVozaci()
        {
            return korisnikRepo.GetAll().OfType<Vozac>().ToList();
        }

        public Vozac GetVozacPoID(string vozacId)
        {
            return GetSviVozaci().FirstOrDefault(v => v.VozacID == vozacId);
        }

        public void DodajVozaca(string username, string password, string imePrezime, string brojLicence, int voziloId)
        {
            var svi = korisnikRepo.GetAll();
            if (svi.Any(x => x.KorisnickoIme.Equals(username, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("Korisnicko ime vec postoji.");

            var vozila = voziloRepo.GetAll();
            if (!vozila.Any(v => v.VoziloID == voziloId && v.Aktivno))
                throw new Exception("Izabrano vozilo ne postoji ili nije aktivno.");

            var voznjaRepo = new VoznjaRepository();
            string noviVozacId = voznjaRepo.GenerisiNoviVozacID();

            var noviVozac = new Vozac
            {
                KorisnickoIme = username,
                PasswordHash = Helpers.HashHelper.Hash(password),
                Uloga = "Vozac",
                VozacID = noviVozacId,
                ImePrezime = imePrezime,
                BrojLicence = brojLicence,
                VoziloID = voziloId,
                Dostupan = true,
                ProsecnaOcena = 0.0,
                BrojOcena = 0
            };

            svi.Add(noviVozac);
            korisnikRepo.SaveAll(svi);
        }

        public void IzmeniVozaca(string vozacId, string imePrezime, string brojLicence, int voziloId, bool dostupan)
        {
            var svi = korisnikRepo.GetAll();
            var vozac = svi.OfType<Vozac>().FirstOrDefault(v => v.VozacID == vozacId);
            if (vozac == null)
                throw new Exception("Vozac nije pronadjen.");

            var vozila = voziloRepo.GetAll();
            if (!vozila.Any(v => v.VoziloID == voziloId && v.Aktivno))
                throw new Exception("Izabrano vozilo ne postoji ili nije aktivno.");

            vozac.ImePrezime = imePrezime;
            vozac.BrojLicence = brojLicence;
            vozac.VoziloID = voziloId;
            vozac.Dostupan = dostupan;

            korisnikRepo.SaveAll(svi);
        }

        public void DeaktivirajVozaca(string vozacId)
        {
            var svi = korisnikRepo.GetAll();
            var korisnikZaBrisanje = svi.FirstOrDefault(k => k is Vozac v && v.VozacID == vozacId);
            if (korisnikZaBrisanje != null)
            {
                svi.Remove(korisnikZaBrisanje);
                korisnikRepo.SaveAll(svi);
            }
        }
    }
}
