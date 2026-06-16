using System;
using System.Linq;
using TaxiPrevoz.Helpers;
using TaxiPrevoz.Models;
using TaxiPrevoz.Repositories;

namespace TaxiPrevoz.Services
{
    public class AuthService
    {
        private readonly IRepository<Korisnik> repository;

        public AuthService(IRepository<Korisnik> repository)
        {
            this.repository = repository;
        }

        public Korisnik Login(string username, string lozinka)
        {
            string hash = HashHelper.Hash(lozinka);
            var korisnici = repository.GetAll();
            var k = korisnici.FirstOrDefault(x => x.KorisnickoIme == username);

            if (k == null)
                throw new Exception("Korisnik ne postoji.");
            if (k.PasswordHash != hash)
                throw new Exception("Pogresna lozinka.");

            return k;
        }
    }
}
