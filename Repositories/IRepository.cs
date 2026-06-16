using System.Collections.Generic;

namespace TaxiPrevoz.Repositories
{
    public interface IRepository<T>
    {
        List<T> GetAll();
        void SaveAll(List<T> items);
    }
}
