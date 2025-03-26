using System;
using System.Collections.Generic;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    public static class FakeDataFactory
    {
        private static List<Employee> _employees;
        private static List<Role> _roles;
        private static List<Preference> _preferences;
        private static List<Customer> _customers;
        private static List<PromoCode> _promoCodes;

        public static List<Employee> Employees => _employees;
        public static List<Role> Roles => _roles;
        public static List<Preference> Preferences => _preferences;
        public static List<Customer> Customers => _customers;
        public static List<PromoCode>  PromoCodes => _promoCodes;
        
        public static void Generate()
        {
            GenerateRoles();
            GenerateEmployees();
            GeneratePreferences();
            GeneratePromoCodes();
            GenerateCustomers();
        }
        
        private static void GenerateRoles()
        {
            _roles =
            [
                new Role()
                {
                    Id = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                    Name = "Admin",
                    Description = "Администратор",
                },

                new Role()
                {
                    Id = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                    Name = "PartnerManager",
                    Description = "Партнерский менеджер"
                }
            ];
        }

        private static void GenerateEmployees()
        {
            _employees =
            [
                new Employee()
                {
                    Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                    Email = "owner@somemail.ru",
                    FirstName = "Иван",
                    LastName = "Сергеев",
                    RoleId = _roles[0].Id,
                    AppliedPromocodesCount = 5
                },

                new Employee()
                {
                    Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                    Email = "andreev@somemail.ru",
                    FirstName = "Петр",
                    LastName = "Андреев",
                    RoleId = _roles[1].Id,
                    AppliedPromocodesCount = 10
                }
            ];
        }

        private static void GeneratePreferences()
        {
            _preferences =
            [
                new Preference()
                {
                    Id = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c"),
                    Name = "Театр",
                },

                new Preference()
                {
                    Id = Guid.Parse("c4bda62e-fc74-4256-a956-4760b3858cbd"),
                    Name = "Семья",
                },

                new Preference()
                {
                    Id = Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84"),
                    Name = "Дети",
                }
            ];
        }

        private static void GenerateCustomers()
        {
            _customers =
            [
                new Customer()
                {
                    Id = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"),
                    Email = "ivan_sergeev@mail.ru",
                    FirstName = "Иван",
                    LastName = "Петров",
                    PromoCodeId = _promoCodes[0].Id,
                    CustomerPreferenceId = _preferences[0].Id,
                }
            ];
        }

        private static void GeneratePromoCodes()
        {
            _promoCodes =
            [
                new PromoCode
                {
                    Id = Guid.Parse("4C473795-40AF-435C-8F14-E2143D37F591"),
                    ServiceInfo = "Театр промокод",
                    PartnerManagerId = _employees[1].Id,
                    PreferenceId = _preferences[0].Id,
                }
            ];
        }
    }
}