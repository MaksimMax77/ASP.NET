using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {
        private IRepository<Customer> _repository;

        public CustomersController(IRepository<Customer> repository)
        {
            _repository = repository;
        }
        
        /// <summary>
        /// создать нового Customer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<CustomerResponse> CreateCustomer(string firstName, string lastName,
            Guid promoCodeId, Guid customerPreferenceId)
        {
            var id = Guid.NewGuid();
            
            var customer = new Customer()
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                PromoCodeId = promoCodeId,
            };/*//todo скорей всего если хочется добавить Preference, то нужно брать DbSet CustomerPreferences и добавлять туда 

            customer.CustomerPreference =
            [
                new CustomerPreference()
                {
                    CustomerId = id,
                    PreferenceId = customerPreferenceId
                }
            ];*/
 
            _repository.Create(customer);

            return CreateCustomerResponse(customer);
        }
        
        /// <summary>
        /// Получить данные всех Customers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<CustomerShortResponse>> GetCustomersAsync()
        {
            var сustomers = await _repository.GetAllAsync();

            var responses = сustomers.Select(x =>
                new CustomerShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                }).ToList();

            return responses;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _repository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();
            
            return CreateCustomerResponse(customer);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> EditCustomersAsync(Guid id, string firstName, string lastName)
        {
            var customer = await _repository.GetByIdAsync(id);
            
            if (customer == null)
                return NotFound();
            
            customer.FirstName = firstName;
            customer.LastName = lastName;
            
            await _repository.UpdateAsync(customer);
            
            return CreateCustomerResponse(customer);
            //TODO: Обновить данные клиента вместе с его предпочтениями
        }

        [HttpDelete("{id:guid}")]
        public async  Task<ActionResult<CustomerResponse>> DeleteCustomer(Guid id)
        {
            var employee = await _repository.GetByIdAsync(id);
            
            if (employee == null)
                return NotFound();
            
            _repository.Delete(employee);
            
            return CreateCustomerResponse(employee);
        }
        
        private CustomerResponse CreateCustomerResponse(Customer customer)
        {
            var customerResponse = new CustomerResponse()
            {
                Id = customer.Id,
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                customerPreferences = customer.CustomerPreference
            };
            return customerResponse;
        }
    }
}