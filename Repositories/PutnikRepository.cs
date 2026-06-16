using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TaxiPrevoz.Models;

namespace TaxiPrevoz.Repositories
{
    public class PutnikRepository : IRepository<Putnik>
    {
        private readonly string filePath;

        public PutnikRepository(string filePath = "txt/putnici.txt")
        {
            this.filePath = filePath;
        }

        public List<Putnik> GetAll()
        {
            var list = new List<Putnik>();
            if (!File.Exists(filePath)) return list;

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                    continue;

                var parts = line.Split(';');
                if (parts.Length < 4) continue;

                try
                {
                    var putnik = new Putnik
                    {
                        PutnikID = parts[0],
                        ImePrezime = parts[1],
                        Telefon = parts[2],
                        Email = parts[3]
                    };
                    list.Add(putnik);
                }
                catch { }
            }
            return list;
        }

        public void SaveAll(List<Putnik> items)
        {
            var lines = new List<string>
            {
                "// Format: PutnikID;ImePrezime;Telefon;Email"
            };

            foreach (var item in items)
            {
                lines.Add($"{item.PutnikID};{item.ImePrezime};{item.Telefon};{item.Email}");
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }
    }
}
