using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController
        : ControllerBase
    {
        private IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeesRepository)
        {
            _employeeRepository = employeesRepository;
        }

        [HttpPost]
        public ActionResult<EmployeeResponse> CreateEmployee(string firstName, string lastName, Guid roleId)
        {
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                RoleId = roleId
            };

            _employeeRepository.Create(employee);

            return CreateEmployeeResponse(employee);
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            
            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                    RoleId = x.RoleId
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();
            
            return CreateEmployeeResponse(employee);
        }
        
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeByIdAsync(Guid id,
            string firstName, string lastName)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            
            if (employee == null)
                return NotFound();
            
            employee.FirstName = firstName;
            employee.LastName = lastName;
            
            await _employeeRepository.UpdateAsync(employee);
            
            return CreateEmployeeResponse(employee);
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> DeleteEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            
            if (employee == null)
                return NotFound();
            
            _employeeRepository.Delete(employee);
            
            return CreateEmployeeResponse(employee);
        }

        private EmployeeResponse CreateEmployeeResponse(Employee employee)
        {
            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Role = new RoleItemResponse()
                {
                    Name = employee.Role.Name,
                    Description = employee.Role.Description
                },
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };
            return employeeModel;
        }
    }
}