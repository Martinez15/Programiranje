using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TaxiPrevoz.Models;

namespace TaxiPrevoz.Repositories
{
    public class KorisnikRepository : IRepository<Korisnik>
    {
        private readonly string filePath;
        private readonly VozacRepository vozacRepository;
        private readonly PutnikRepository putnikRepository;

        public KorisnikRepository(
            string filePath = "txt/korisnici.txt", 
            VozacRepository vozacRepo = null, 
            PutnikRepository putnikRepo = null)
        {
            this.filePath = filePath;
            this.vozacRepository = vozacRepo ?? new VozacRepository();
            this.putnikRepository = putnikRepo ?? new PutnikRepository();
        }

        public List<Korisnik> GetAll()
        {
            var list = new List<Korisnik>();
            if (!File.Exists(filePath)) return list;

            var vozaci = vozacRepository.GetAll();
            var putnici = putnikRepository.GetAll();

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                    continue;

                var parts = line.Split('|');
                if (parts.Length < 4) continue;

                string username = parts[0];
                string hash = parts[1];
                string uloga = parts[2];
                string dodatniId = parts[3];

                if (uloga == "Administrator")
                {
                    var admin = new Admin
                    {
                        KorisnickoIme = username,
                        PasswordHash = hash,
                        Uloga = uloga
                    };
                    list.Add(admin);
                }
                else if (uloga == "Vozac")
                {
                    var vData = vozaci.FirstOrDefault(x => x.VozacID == dodatniId);
                    var vozac = new Vozac
                    {
                        KorisnickoIme = username,
                        PasswordHash = hash,
                        Uloga = uloga,
                        VozacID = dodatniId,
                        ImePrezime = vData?.ImePrezime ?? username,
                        BrojLicence = vData?.BrojLicence ?? "",
                        VoziloID = vData?.VoziloID ?? 0,
                        Dostupan = vData?.Dostupan ?? true,
                        ProsecnaOcena = vData?.ProsecnaOcena ?? 0,
                        BrojOcena = vData?.BrojOcena ?? 0
                    };
                    list.Add(vozac);
                }
                else if (uloga == "Putnik")
                {
                    var pData = putnici.FirstOrDefault(x => x.PutnikID == dodatniId);
                    var putnik = new Putnik
                    {
                        KorisnickoIme = username,
                        PasswordHash = hash,
                        Uloga = uloga,
                        PutnikID = dodatniId,
                        ImePrezime = pData?.ImePrezime ?? username,
                        Telefon = pData?.Telefon ?? "",
                        Email = pData?.Email ?? ""
                    };
                    list.Add(putnik);
                }
            }

            return list;
        }

        public void SaveAll(List<Korisnik> items)
        {
            var lines = new List<string>
            {
                "// Format: Username|PasswordHash|Uloga|DodatniID"
            };

            var vozaciZaCuvanje = new List<Vozac>();
            var putniciZaCuvanje = new List<Putnik>();

            foreach (var item in items)
            {
                string dodatniId = "";
                if (item is Vozac vozac)
                {
                    dodatniId = vozac.VozacID;
                    vozaciZaCuvanje.Add(vozac);
                }
                else if (item is Putnik putnik)
                {
                    dodatniId = putnik.PutnikID;
                    putniciZaCuvanje.Add(putnik);
                }

                lines.Add($"{item.KorisnickoIme}|{item.PasswordHash}|{item.Uloga}|{dodatniId}");
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);

            if (vozaciZaCuvanje.Count > 0)
                vozacRepository.SaveAll(vozaciZaCuvanje);
            if (putniciZaCuvanje.Count > 0)
                putnikRepository.SaveAll(putniciZaCuvanje);
        }
    }
}
