using packt_webapp.Entities;
using packt_webapp.QueryParameters;
using System;
using System.Linq;

namespace packt_webapp.Repositories
{
    public interface ICustomerRepository
    {
        void Add(Customer item);
        void Delete(Guid id);
        IQueryable<Customer> GetAll(CustomerQueryParameters customerQueryParameters);
        Customer GetSingle(Guid id);
        bool Save();
        void Update(Customer item);
        int Count();
    }
}