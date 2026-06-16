using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TaxiPrevoz.Models;

namespace TaxiPrevoz.Repositories
{
    public class VoziloRepository : IRepository<Vozilo>
    {
        private readonly string filePath;

        public VoziloRepository(string filePath = "txt/vozila.txt")
        {
            this.filePath = filePath;
        }

        public List<Vozilo> GetAll()
        {
            var list = new List<Vozilo>();
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
                    var vozilo = new Vozilo
                    {
                        VoziloID = int.Parse(parts[0]),
                        Registracija = parts[1],
                        Marka = parts[2],
                        Model = parts[3],
                        Godiste = int.Parse(parts[4]),
                        Boja = parts[5],
                        Aktivno = bool.Parse(parts[6])
                    };
                    list.Add(vozilo);
                }
                catch { }
            }
            return list;
        }

        public void SaveAll(List<Vozilo> items)
        {
            var lines = new List<string>
            {
                "// Format: VoziloID;Registracija;Marka;Model;Godiste;Boja;Aktivno"
            };

            foreach (var item in items)
            {
                lines.Add($"{item.VoziloID};{item.Registracija};{item.Marka};{item.Model};{item.Godiste};{item.Boja};{item.Aktivno.ToString().ToLower()}");
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }
    }
}
