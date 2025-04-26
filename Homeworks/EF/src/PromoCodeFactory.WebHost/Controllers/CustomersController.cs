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
        public async Task<CustomerResponse> CreateCustomer(string firstName, string lastName,
            Guid promoCodeId, Guid customerPreferenceId)
        {
            var id = Guid.NewGuid();
            var customer = new Customer()
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                PromoCodeId = promoCodeId,
            };

            _repository.Create(customer);

            await AddPreferenceByCustomerId(id, customerPreferenceId);

            return CreateCustomerResponse(customer);
        }

        private async Task AddPreferenceByCustomerId(Guid customerId, Guid customerPreferenceId)
        {
            var currentCustomer = await _repository.GetByIdAsync(customerId);

            var customerPreference = new CustomerPreference()
            {
                CustomerId = currentCustomer.Id,
                PreferenceId =  customerPreferenceId
            };

            currentCustomer.CustomerPreference.Add(customerPreference);
            await _repository.UpdateAsync(currentCustomer);
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

        /// <summary>
        /// Получить Customer по id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _repository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();
            
            return CreateCustomerResponse(customer);
        }

        /// <summary>
        /// изменить Customer по id firstName и lastName
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// удалить Customer по id
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> DeleteCustomer(Guid id)
        {
            var employee = await _repository.GetByIdAsync(id);
            
            if (employee == null)
                return NotFound();
            
            _repository.Delete(employee);
            
            return CreateCustomerResponse(employee);
        }

        #region CustomerResponseGeneration
        private CustomerResponse CreateCustomerResponse(Customer customer)
        {
            var customerResponse = new CustomerResponse()
            {
                Id = customer.Id,
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PromoCodeShortResponse = CreatePromoCodeShortResponse(customer),
                Preferences = CreatePreferenceResponses(customer)
            };

            return customerResponse;
        }

        private PromoCodeShortResponse CreatePromoCodeShortResponse(Customer customer)
        {
            return new PromoCodeShortResponse()
            {
                Id = customer.PromoCodeId,
                Code = customer.PromoCode.Code,
                ServiceInfo = customer.PromoCode.ServiceInfo,
                BeginDate = customer.PromoCode.BeginDate,
                EndDate = customer.PromoCode.EndDate,
                PartnerName = customer.PromoCode.PartnerName
            };
        }

        private List<PreferenceResponse> CreatePreferenceResponses(Customer customer)
        {
            return customer.CustomerPreference.Select(preference => new PreferenceResponse()
            {
                Id = preference.PreferenceId,
                Name = preference.Preference.Name
            }).ToList();
        }
        #endregion
    }
}