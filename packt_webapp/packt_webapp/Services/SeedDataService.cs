using packt_webapp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace packt_webapp.Services
{
    public class SeedDataService: ISeedDataService
    {
        private PacktDbContext _context;
        public SeedDataService(PacktDbContext context)
        {
            _context = context;
        }
        public async Task EnsureSeedData()
        {
            _context.Database.EnsureCreated();

            _context.Customers.RemoveRange(_context.Customers);
            _context.SaveChanges();

            Customer customer = new Customer();
            customer.FirstName = "Tharana";
            customer.LastName = "Mayuranga";
            customer.Age = 26;
            customer.Id = Guid.NewGuid();

            _context.Add(customer);

            Customer customer2 = new Customer();
            customer2.FirstName = "Amila";
            customer2.LastName = "Suranga";
            customer2.Age = 30;
            customer2.Id = Guid.NewGuid();


            _context.Add(customer2);

            Customer customer3 = new Customer();
            customer3.FirstName = "Amila";
            customer3.LastName = "Suranga";
            customer3.Age = 30;
            customer3.Id = Guid.NewGuid();


            _context.Add(customer3);

            Customer customer4 = new Customer();
            customer4.FirstName = "Amila";
            customer4.LastName = "Suranga";
            customer4.Age = 30;
            customer4.Id = Guid.NewGuid();


            _context.Add(customer4);

            Customer customer5 = new Customer();
            customer5.FirstName = "Amila";
            customer5.LastName = "Suranga";
            customer5.Age = 30;
            customer5.Id = Guid.NewGuid();


            _context.Add(customer5);
            Customer customer6 = new Customer();
            customer6.FirstName = "Amila";
            customer6.LastName = "Suranga";
            customer6.Age = 30;
            customer6.Id = Guid.NewGuid();


            _context.Add(customer6);

            await _context.SaveChangesAsync();



        }

    }
}
