using Microsoft.EntityFrameworkCore;
using MiniERP.ApplicationLayer.Interfaces;
using MiniERP.Domain;
using MiniERP.Infrastructure.Data;

namespace MiniERP.Infrastructure.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<IEnumerable<Customer>> SearchAsync(string keyword)
        {
            return await _dbSet
                .Where(c => c.Name.Contains(keyword) || 
                           c.Code.Contains(keyword) ||
                           (c.ContactPerson != null && c.ContactPerson.Contains(keyword)))
                .ToListAsync();
        }
    }
}

