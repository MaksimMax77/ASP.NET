using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        private IRepository<PromoCode> _promoCodesRepository;
        private IRepository<Customer> _customerRepository;

        public PromocodesController(IRepository<PromoCode> promoCodesRepository,
            IRepository<Customer> customerRepository)
        {
            _promoCodesRepository = promoCodesRepository;
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromoCodesAsync()
        {
            var promoCodes = await _promoCodesRepository.GetAllAsync();

            var responses = promoCodes.Select(x =>
                new PromoCodeShortResponse()
                {
                    Id = x.Id,
                    Code = x.Code,
                    ServiceInfo = x.ServiceInfo,
                    BeginDate = x.BeginDate,
                    EndDate = x.EndDate,
                    PartnerName = x.PartnerName,
                }).ToList();

            return responses;
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<PromoCodeShortResponse>> GivePromoCodesToCustomersWithPreferenceAsync(
            GivePromoCodeRequest request)
        {
            var promoCode = new PromoCode()
            {
                Id = Guid.NewGuid(),
                Code = Guid.NewGuid().ToString(),
                ServiceInfo = request.ServiceInfo,
                PartnerName = request.PartnerName,
                PartnerManagerId = Guid.Parse(request.PartnerManagerId),
                BeginDate = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                PreferenceId = Guid.Parse(request.PreferenceId)
            };

            _promoCodesRepository.Create(promoCode);

            await AddPromoCodeToCustomersByPreferenceId(promoCode);

            return await Task.FromResult<ActionResult<PromoCodeShortResponse>>(CreatePromoCodeResponse(promoCode));
            //TODO: Создать промокод и выдать его клиентам с указанным предпочтением
        }

        private async Task AddPromoCodeToCustomersByPreferenceId(PromoCode promoCode)
        {
            var customers = await _customerRepository.GetAllAsync();

            foreach (var customer in customers)
            {
                foreach (var customerPreference in customer.CustomerPreference)
                {
                    if (customerPreference.PreferenceId != promoCode.PreferenceId)
                    {
                        continue;
                    }

                    customer.PromoCode = promoCode;
                    await _customerRepository.UpdateAsync(customer);
                }
            }
        }

        private PromoCodeShortResponse CreatePromoCodeResponse(PromoCode promoCode)
        {
            return new PromoCodeShortResponse
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                BeginDate = promoCode.BeginDate,
                EndDate = promoCode.EndDate,
                PartnerName = promoCode.PartnerName,
            };
        }
    }
}