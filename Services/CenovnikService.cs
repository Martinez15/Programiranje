using System;
using System.Collections.Generic;
using System.Linq;
using TaxiPrevoz.Models;
using TaxiPrevoz.Repositories;

namespace TaxiPrevoz.Services
{
    public class CenovnikService
    {
        private readonly IRepository<Cenovnik> repository;

        public CenovnikService(IRepository<Cenovnik> repository)
        {
            this.repository = repository;
        }

        public Cenovnik GetCenovnik()
        {
            return repository.GetAll().FirstOrDefault();
        }

        public void AzurirajCenovnik(decimal startna, decimal poKm, decimal nocna)
        {
            var cenovnik = new Cenovnik
            {
                StartnaCena = startna,
                CenaPoKm = poKm,
                NocnaDoplataCena = nocna
            };
            repository.SaveAll(new List<Cenovnik> { cenovnik });
        }
    }
}
