using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TaxiPrevoz.Models;

namespace TaxiPrevoz.Repositories
{
    public class VoznjaRepository : IRepository<Voznja>
    {
        private readonly string filePath;
        private readonly string counterPath;
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm";

        public VoznjaRepository(string filePath = "txt/voznje.txt", string counterPath = "txt/brojaci.txt")
        {
            this.filePath = filePath;
            this.counterPath = counterPath;
        }

        public List<Voznja> GetAll()
        {
            var list = new List<Voznja>();
            if (!File.Exists(filePath)) return list;

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                    continue;

                var parts = line.Split(';');
                if (parts.Length < 13) continue;

                try
                {
                    var voznja = new Voznja
                    {
                        VoznjaID = parts[0],
                        PutnikID = parts[1],
                        VozacID = string.IsNullOrEmpty(parts[2]) ? null : parts[2],
                        Polaziste = parts[3],
                        Odrediste = parts[4],
                        VremeNarucivanja = DateTime.ParseExact(parts[5], DateTimeFormat, CultureInfo.InvariantCulture),
                        VremePolaska = string.IsNullOrEmpty(parts[6]) ? (DateTime?)null : DateTime.ParseExact(parts[6], DateTimeFormat, CultureInfo.InvariantCulture),
                        VremeDolaska = string.IsNullOrEmpty(parts[7]) ? (DateTime?)null : DateTime.ParseExact(parts[7], DateTimeFormat, CultureInfo.InvariantCulture),
                        Kilometraza = double.Parse(parts[8], CultureInfo.InvariantCulture),
                        Cena = decimal.Parse(parts[9], CultureInfo.InvariantCulture),
                        Status = (StatusVoznje)Enum.Parse(typeof(StatusVoznje), parts[10]),
                        Ocena = string.IsNullOrEmpty(parts[11]) ? (int?)null : int.Parse(parts[11]),
                        Komentar = string.IsNullOrEmpty(parts[12]) ? null : parts[12]
                    };
                    list.Add(voznja);
                }
                catch { }
            }
            return list;
        }

        public void SaveAll(List<Voznja> items)
        {
            var lines = new List<string>
            {
                "// Format: VoznjaID;PutnikID;VozacID;Polaziste;Odrediste;VremeNar;VremePol;VremeDol;Km;Cena;Status;Ocena;Komentar"
            };

            foreach (var item in items)
            {
                string vremeNar = item.VremeNarucivanja.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
                string vremePol = item.VremePolaska?.ToString(DateTimeFormat, CultureInfo.InvariantCulture) ?? "";
                string vremeDol = item.VremeDolaska?.ToString(DateTimeFormat, CultureInfo.InvariantCulture) ?? "";
                string km = item.Kilometraza.ToString(CultureInfo.InvariantCulture);
                string cena = item.Cena.ToString(CultureInfo.InvariantCulture);
                string ocena = item.Ocena?.ToString() ?? "";
                string komentar = item.Komentar ?? "";

                lines.Add($"{item.VoznjaID};{item.PutnikID};{item.VozacID ?? ""};{item.Polaziste};{item.Odrediste};{vremeNar};{vremePol};{vremeDol};{km};{cena};{item.Status};{ocena};{komentar}");
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }

        public string GenerisiNoviID()
        {
            int counter = ProcitajBrojac("Voznja");
            counter++;
            SacuvajBrojac("Voznja", counter);
            int year = DateTime.Now.Year;
            return $"VOZ-{year}-{counter:D4}";
        }

        public string GenerisiNoviPutnikID()
        {
            int counter = ProcitajBrojac("Putnik");
            counter++;
            SacuvajBrojac("Putnik", counter);
            return $"PUT-{counter:D4}";
        }

        public string GenerisiNoviVozacID()
        {
            int counter = ProcitajBrojac("Vozac");
            counter++;
            SacuvajBrojac("Vozac", counter);
            return $"VOZ-{counter:D4}";
        }

        public int GenerisiNoviVoziloID()
        {
            int counter = ProcitajBrojac("Vozilo");
            counter++;
            SacuvajBrojac("Vozilo", counter);
            return counter;
        }

        private int ProcitajBrojac(string tip)
        {
            if (!File.Exists(counterPath)) return 0;
            var lines = File.ReadAllLines(counterPath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;
                var parts = line.Split(';');
                if (parts.Length == 2 && parts[0] == tip)
                {
                    return int.Parse(parts[1]);
                }
            }
            return 0;
        }

        private void SacuvajBrojac(string tip, int novaVrednost)
        {
            var lines = new List<string>();
            bool pronadjen = false;
            if (File.Exists(counterPath))
            {
                foreach (var line in File.ReadAllLines(counterPath))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split(';');
                    if (parts.Length == 2 && parts[0] == tip)
                    {
                        lines.Add($"{tip};{novaVrednost}");
                        pronadjen = true;
                    }
                    else
                    {
                        lines.Add(line);
                    }
                }
            }
            if (!pronadjen)
            {
                lines.Add($"{tip};{novaVrednost}");
            }
            File.WriteAllLines(counterPath, lines, Encoding.UTF8);
        }
    }
}
