using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TaxiPrevoz.Models;

namespace TaxiPrevoz.Repositories
{
    public class CenovnikRepository : IRepository<Cenovnik>
    {
        private readonly string filePath;

        public CenovnikRepository(string filePath = "txt/cenovnik.txt")
        {
            this.filePath = filePath;
        }

        public List<Cenovnik> GetAll()
        {
            var list = new List<Cenovnik>();
            if (!File.Exists(filePath)) return list;

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                    continue;

                var parts = line.Split(';');
                if (parts.Length < 3) continue;

                try
                {
                    var cenovnik = new Cenovnik
                    {
                        StartnaCena = decimal.Parse(parts[0], CultureInfo.InvariantCulture),
                        CenaPoKm = decimal.Parse(parts[1], CultureInfo.InvariantCulture),
                        NocnaDoplataCena = decimal.Parse(parts[2], CultureInfo.InvariantCulture)
                    };
                    list.Add(cenovnik);
                    break;
                }
                catch { }
            }

            if (list.Count == 0)
            {
                list.Add(new Cenovnik { StartnaCena = 200, CenaPoKm = 65, NocnaDoplataCena = 50 });
            }

            return list;
        }

        public void SaveAll(List<Cenovnik> items)
        {
            var lines = new List<string>
            {
                "// Format: StartnaCena;CenaPoKm;NocnaDoplata"
            };

            if (items.Count > 0)
            {
                var item = items[0];
                string sc = item.StartnaCena.ToString(CultureInfo.InvariantCulture);
                string cp = item.CenaPoKm.ToString(CultureInfo.InvariantCulture);
                string nd = item.NocnaDoplataCena.ToString(CultureInfo.InvariantCulture);
                lines.Add($"{sc};{cp};{nd}");
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }
    }
}
