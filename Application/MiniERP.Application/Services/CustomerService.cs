using MiniERP.ApplicationLayer.Interfaces;
using MiniERP.Domain;

namespace MiniERP.ApplicationLayer.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<Customer?> GetCustomerByCodeAsync(string code)
        {
            return await _customerRepository.GetByCodeAsync(code);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await GetAllCustomersAsync();
            }
            return await _customerRepository.SearchAsync(keyword);
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Code))
            {
                throw new ArgumentException("客户编码不能为空", nameof(customer));
            }

            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                throw new ArgumentException("客户名称不能为空", nameof(customer));
            }

            var existing = await _customerRepository.GetByCodeAsync(customer.Code);
            if (existing != null)
            {
                throw new InvalidOperationException($"客户编码 {customer.Code} 已存在");
            }

            customer.CreatedAt = DateTime.Now;
            return await _customerRepository.AddAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var existing = await _customerRepository.GetByIdAsync(customer.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"客户 ID {customer.Id} 不存在");
            }

            if (existing.Code != customer.Code)
            {
                var codeExists = await _customerRepository.GetByCodeAsync(customer.Code);
                if (codeExists != null)
                {
                    throw new InvalidOperationException($"客户编码 {customer.Code} 已存在");
                }
            }

            customer.UpdatedAt = DateTime.Now;
            await _customerRepository.UpdateAsync(customer);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var existing = await _customerRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"客户 ID {id} 不存在");
            }

            await _customerRepository.DeleteAsync(id);
        }
    }
}

