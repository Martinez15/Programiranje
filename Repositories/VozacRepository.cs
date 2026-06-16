using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TaxiPrevoz.Models;

namespace TaxiPrevoz.Repositories
{
    public class VozacRepository : IRepository<Vozac>
    {
        private readonly string filePath;

        public VozacRepository(string filePath = "txt/vozaci.txt")
        {
            this.filePath = filePath;
        }

        public List<Vozac> GetAll()
        {
            var list = new List<Vozac>();
            if (!File.Exists(filePath)) return list;

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                    continue;

                var parts = line.Split(';');
                if (parts.Length < 7) continue;

                try
                {
                    var vozac = new Vozac
                    {
                        VozacID = parts[0],
                        ImePrezime = parts[1],
                        BrojLicence = parts[2],
                        VoziloID = int.Parse(parts[3]),
                        Dostupan = bool.Parse(parts[4]),
                        ProsecnaOcena = double.Parse(parts[5], CultureInfo.InvariantCulture),
                        BrojOcena = int.Parse(parts[6])
                    };
                    list.Add(vozac);
                }
                catch { }
            }
            return list;
        }

        public void SaveAll(List<Vozac> items)
        {
            var lines = new List<string>
            {
                "// Format: VozacID;ImePrezime;BrojLicence;VoziloID;Dostupan;ProsecnaOcena;BrojOcena"
            };

            foreach (var item in items)
            {
                string prosek = item.ProsecnaOcena.ToString("0.0#", CultureInfo.InvariantCulture);
                lines.Add($"{item.VozacID};{item.ImePrezime};{item.BrojLicence};{item.VoziloID};{item.Dostupan.ToString().ToLower()};{prosek};{item.BrojOcena}");
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }
    }
}
