using MiniERP.Domain;

namespace MiniERP.ApplicationLayer.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByCodeAsync(string code);
        Task<IEnumerable<Customer>> SearchAsync(string keyword);
    }
}

