using System;
using System.Collections.Generic;
using System.Linq;
using TaxiPrevoz.Models;
using TaxiPrevoz.Repositories;

namespace TaxiPrevoz.Services
{
    public class VoznjaService
    {
        private readonly VoznjaRepository voznjaRepo;
        private readonly IRepository<Korisnik> korisnikRepo;
        private readonly IRepository<Cenovnik> cenovnikRepo;
        private readonly LogService logService;

        public VoznjaService(
            VoznjaRepository voznjaRepo, 
            IRepository<Korisnik> korisnikRepo, 
            IRepository<Cenovnik> cenovnikRepo, 
            LogService logService)
        {
            this.voznjaRepo = voznjaRepo;
            this.korisnikRepo = korisnikRepo;
            this.cenovnikRepo = cenovnikRepo;
            this.logService = logService;
        }

        public List<Voznja> GetSveVoznje()
        {
            return voznjaRepo.GetAll();
        }

        public List<Voznja> GetVoznjeZaKorisnika(Korisnik korisnik)
        {
            var sve = voznjaRepo.GetAll();
            if (korisnik is Admin)
            {
                return sve;
            }
            else if (korisnik is Vozac vozac)
            {
                return sve.Where(v => v.VozacID == vozac.VozacID).ToList();
            }
            else if (korisnik is Putnik putnik)
            {
                return sve.Where(v => v.PutnikID == putnik.PutnikID).ToList();
            }
            return new List<Voznja>();
        }

        public Voznja NaruciVoznju(string putnikId, string polaziste, string odrediste, double procenjenaKm)
        {
            var korisnici = korisnikRepo.GetAll();
            var slobodanVozac = korisnici.OfType<Vozac>().FirstOrDefault(v => v.Dostupan);

            if (slobodanVozac == null)
                throw new Exception("Trenutno nema slobodnih vozaca na duznosti.");

            var cenovnik = cenovnikRepo.GetAll().FirstOrDefault() ?? new Cenovnik { StartnaCena = 200, CenaPoKm = 65, NocnaDoplataCena = 50 };
            DateTime vremeNar = DateTime.Now;
            bool nocna = vremeNar.Hour >= 22 || vremeNar.Hour < 6;
            decimal cena = cenovnik.ObracunajCenu(procenjenaKm, nocna);

            string noviId = voznjaRepo.GenerisiNoviID();
            var novaVoznja = new Voznja
            {
                VoznjaID = noviId,
                PutnikID = putnikId,
                VozacID = slobodanVozac.VozacID,
                Polaziste = polaziste,
                Odrediste = odrediste,
                VremeNarucivanja = vremeNar,
                VremePolaska = null,
                VremeDolaska = null,
                Kilometraza = procenjenaKm,
                Cena = cena,
                Status = StatusVoznje.Narucena,
                Ocena = null,
                Komentar = null
            };

            slobodanVozac.Dostupan = false;
            korisnikRepo.SaveAll(korisnici);

            var voznje = voznjaRepo.GetAll();
            voznje.Add(novaVoznja);
            voznjaRepo.SaveAll(voznje);

            logService.Log($"Voznja {noviId} narucena od putnika {putnikId}. Dodeljen vozac {slobodanVozac.VozacID}. Status: Narucena.");

            return novaVoznja;
        }

        public void PrihvatiVoznju(string voznjaId)
        {
            var voznje = voznjaRepo.GetAll();
            var voznja = voznje.FirstOrDefault(v => v.VoznjaID == voznjaId);
            if (voznja == null) throw new Exception("Voznja ne postoji.");
            if (voznja.Status != StatusVoznje.Narucena)
                throw new Exception("Voznja je vec prihvacena ili nije u statusu Narucena.");

            string stariStatus = voznja.Status.ToString();
            voznja.Status = StatusVoznje.Prihvacena;
            voznjaRepo.SaveAll(voznje);

            // Osiguravamo da je vozac oznacen kao nedostupan
            var korisnici = korisnikRepo.GetAll();
            var vozac = korisnici.OfType<Vozac>().FirstOrDefault(v => v.VozacID == voznja.VozacID);
            if (vozac != null && vozac.Dostupan)
            {
                vozac.Dostupan = false;
                korisnikRepo.SaveAll(korisnici);
            }

            logService.Log($"Voznja {voznjaId} prihvacena od strane vozaca {voznja.VozacID}. Status: {stariStatus} -> Prihvacena.");
        }

        public void OdbijVoznju(string voznjaId)
        {
            var voznje = voznjaRepo.GetAll();
            var voznja = voznje.FirstOrDefault(v => v.VoznjaID == voznjaId);
            if (voznja == null) throw new Exception("Voznja ne postoji.");
            if (voznja.Status != StatusVoznje.Narucena) throw new Exception("Voznja se ne moze odbiti u trenutnom statusu.");

            string odbijeniVozacId = voznja.VozacID;

            var korisnici = korisnikRepo.GetAll();
            var odbijeniVozac = korisnici.OfType<Vozac>().FirstOrDefault(v => v.VozacID == odbijeniVozacId);
            if (odbijeniVozac != null)
            {
                odbijeniVozac.Dostupan = true;
            }

            var noviVozac = korisnici.OfType<Vozac>().FirstOrDefault(v => v.Dostupan && v.VozacID != odbijeniVozacId);

            if (noviVozac != null)
            {
                voznja.VozacID = noviVozac.VozacID;
                noviVozac.Dostupan = false;
                logService.Log($"Vozac {odbijeniVozacId} je odbio voznju {voznjaId}. Voznja je automatski dodeljena vozacu {noviVozac.VozacID}.");
            }
            else
            {
                voznja.Status = StatusVoznje.Otkazana;
                logService.Log($"Vozac {odbijeniVozacId} je odbio voznju {voznjaId}. Nema drugih slobodnih vozaca, voznja je otkazana.");
            }

            korisnikRepo.SaveAll(korisnici);
            voznjaRepo.SaveAll(voznje);
        }

        public void KreniVoznju(string voznjaId)
        {
            var voznje = voznjaRepo.GetAll();
            var voznja = voznje.FirstOrDefault(v => v.VoznjaID == voznjaId);
            if (voznja == null) throw new Exception("Voznja ne postoji.");
            if (voznja.Status != StatusVoznje.Prihvacena) throw new Exception("Voznja mora biti prihvacena da bi zapocela.");

            voznja.Status = StatusVoznje.UToku;
            voznja.VremePolaska = DateTime.Now;

            voznjaRepo.SaveAll(voznje);
            logService.Log($"Voznja {voznjaId} je zapoceta. Status: U toku.");
        }

        public void ZavrsiVoznju(string voznjaId, double stvarnaKilometraza)
        {
            var voznje = voznjaRepo.GetAll();
            var voznja = voznje.FirstOrDefault(v => v.VoznjaID == voznjaId);
            if (voznja == null) throw new Exception("Voznja ne postoji.");
            if (voznja.Status != StatusVoznje.UToku) throw new Exception("Voznja mora biti u toku da bi se zavrsila.");

            var cenovnik = cenovnikRepo.GetAll().FirstOrDefault() ?? new Cenovnik { StartnaCena = 200, CenaPoKm = 65, NocnaDoplataCena = 50 };
            DateTime vremeDol = DateTime.Now;
            bool nocna = (voznja.VremePolaska ?? vremeDol).Hour >= 22 || (voznja.VremePolaska ?? vremeDol).Hour < 6;

            voznja.Kilometraza = stvarnaKilometraza;
            voznja.Cena = cenovnik.ObracunajCenu(stvarnaKilometraza, nocna);
            voznja.Status = StatusVoznje.Zavrsena;
            voznja.VremeDolaska = vremeDol;

            var korisnici = korisnikRepo.GetAll();
            var vozac = korisnici.OfType<Vozac>().FirstOrDefault(v => v.VozacID == voznja.VozacID);
            if (vozac != null)
            {
                vozac.Dostupan = true;
            }

            korisnikRepo.SaveAll(korisnici);
            voznjaRepo.SaveAll(voznje);

            logService.Log($"Voznja {voznjaId} je uspesno zavrsena. Km: {stvarnaKilometraza}, Cena: {voznja.Cena}. Vozac {voznja.VozacID} je ponovo dostupan.");
        }

        public void OtkaziVoznju(string voznjaId)
        {
            var voznje = voznjaRepo.GetAll();
            var voznja = voznje.FirstOrDefault(v => v.VoznjaID == voznjaId);
            if (voznja == null) throw new Exception("Voznja ne postoji.");

            if (!voznja.MozeSeOtkazati())
                throw new Exception("Voznja se ne moze otkazati jer je vec u toku ili zavrsena.");

            string stariStatus = voznja.Status.ToString();
            voznja.Status = StatusVoznje.Otkazana;

            if (!string.IsNullOrEmpty(voznja.VozacID))
            {
                var korisnici = korisnikRepo.GetAll();
                var vozac = korisnici.OfType<Vozac>().FirstOrDefault(v => v.VozacID == voznja.VozacID);
                if (vozac != null)
                {
                    vozac.Dostupan = true;
                }
                korisnikRepo.SaveAll(korisnici);
            }

            voznjaRepo.SaveAll(voznje);
            logService.Log($"Voznja {voznjaId} je otkazana. Prethodni status: {stariStatus}.");
        }

        private void PromeniStatusVoznje(string voznjaId, StatusVoznje noviStatus)
        {
            var voznje = voznjaRepo.GetAll();
            var voznja = voznje.FirstOrDefault(v => v.VoznjaID == voznjaId);
            if (voznja == null) throw new Exception("Voznja ne postoji.");

            string stariStatus = voznja.Status.ToString();
            voznja.Status = noviStatus;

            voznjaRepo.SaveAll(voznje);
            logService.Log($"Voznja {voznjaId} promenila status iz {stariStatus} u {noviStatus}.");
        }
    }
}
