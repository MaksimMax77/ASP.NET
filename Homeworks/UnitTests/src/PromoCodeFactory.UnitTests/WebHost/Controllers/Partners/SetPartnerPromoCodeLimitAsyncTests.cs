using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests //TODO: Add Unit Tests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;
        
        public SetPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound() //пункт 1 Если партнер не найден, то также нужно выдать ошибку 404;
        {
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            Partner partner = null;

            var setPartnerPromoCodeLimitRequest = new SetPartnerPromoCodeLimitRequest()
            {
                EndDate = DateTime.Now.AddDays(1000),
                Limit = 3
            };
            
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, setPartnerPromoCodeLimitRequest);

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest() //пункт 2 Если партнер заблокирован, то есть поле IsActive=false в классе Partner, то также нужно выдать ошибку 400;
        {
            // Arrange
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var partner = CreateBasePartner(partnerId);
            partner.IsActive = false;
            
            var setPartnerPromoCodeLimitRequest = new SetPartnerPromoCodeLimitRequest()
            {
                EndDate = DateTime.Now.AddDays(1000),
                Limit = 3
            };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, setPartnerPromoCodeLimitRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNumberIssuedPromoCodesIsZero() //пункт 3 Если партнеру выставляется лимит,
                                                                                              //то мы должны обнулить количество промокодов,
                                                                                              //которые партнер выдал NumberIssuedPromoCodes, если лимит закончился, то количество не обнуляется;
        {
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var partner = CreateBasePartner(partnerId);
     
            var setPartnerPromoCodeLimitRequest = new SetPartnerPromoCodeLimitRequest()
            {
                EndDate = DateTime.Now.AddDays(1000),
                Limit = 3
            };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, setPartnerPromoCodeLimitRequest);
            
            Assert.Equal(0, partner.NumberIssuedPromoCodes);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ActiveLimitCancelDateDateHasValue() //пункт 4 При установке лимита нужно отключить предыдущий лимит
        {
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var partner = CreateBasePartner(partnerId);

            var setPartnerPromoCodeLimitRequest = new SetPartnerPromoCodeLimitRequest()
            {
                EndDate = DateTime.Now.AddDays(1000),
                Limit = 3
            };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);
            
            var previousLimit = partner.PartnerLimits.ToArray()[partner.PartnerLimits.Count - 1];
            
            await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, setPartnerPromoCodeLimitRequest);
            
            Assert.NotNull(previousLimit);
            Assert.True(previousLimit.CancelDate.HasValue);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ActiveLimitIsGreaterThanZero()//пункт 5 Лимит должен быть больше 0;
        {
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var partner = CreateBasePartner(partnerId);

            var setPartnerPromoCodeLimitRequest = new SetPartnerPromoCodeLimitRequest()
            {
                EndDate = DateTime.Now.AddDays(1000),
                Limit = 3
            };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, setPartnerPromoCodeLimitRequest);
            
            var lastLimit = partner.PartnerLimits.ToArray()[partner.PartnerLimits.Count - 1];//берется последний добавленный лимит

            Assert.True(lastLimit.Limit > 0);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_RepositoryUpdateSuccess()//пункт 6 Нужно убедиться, что сохранили новый лимит в базу данных
        {
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var partner = CreateBasePartner(partnerId);
            var setPartnerPromoCodeLimitRequest = new SetPartnerPromoCodeLimitRequest()
            {
                EndDate = DateTime.Now.AddDays(1000),
                Limit = 3
            };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);
            
            await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, setPartnerPromoCodeLimitRequest);
            
            _partnersRepositoryMock.Verify(x => x.UpdateAsync(partner), Times.Once());
        }

        private Partner CreateBasePartner(Guid id)
        {
            var partner = new Partner()
            {
                Id = id,
                Name = "Суперигрушки",
                IsActive = true,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
                {
                    new()
                    {
                        Id = Guid.Parse("e00633a5-978a-420e-a7d6-3e1dab116393"),
                        CreateDate = new DateTime(2020, 07, 9),
                        EndDate = new DateTime(2020, 10, 9),
                        Limit = 100
                    }
                }
            };

            return partner;
        }
    }
}