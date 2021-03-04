using Accounts.Intuit.Dto;
using AutoMapper;
using Intuit.Ipp.Data;
using System;
using System.Collections.Generic;
using System.Text;
using IntuitData = Intuit.Ipp.Data;

namespace Accounts.Intuit
{
    public class ConvertCompanyToCustomer : ITypeConverter<IntuitCompanyDto, IntuitData.Customer>
    {
        public Customer Convert(IntuitCompanyDto source, Customer destination, ResolutionContext context)
        {
            var customer = new IntuitData.Customer
            {
                CompanyName = source.CompanyName,
                DisplayName = source.CompanyName,
                FullyQualifiedName = source.CompanyName,
                PrintOnCheckName = source.CompanyName,
                PrimaryEmailAddr = new IntuitData.EmailAddress
                {
                    Address = source.Email
                },
                GivenName = source.FirstName,
                MiddleName = source.MiddleName,
                FamilyName = source.LastName,
                PrimaryPhone = new IntuitData.TelephoneNumber
                {
                    FreeFormNumber = source.PhoneNumber
                },
                Mobile = new IntuitData.TelephoneNumber
                {
                    FreeFormNumber = source.MobileNumber
                },
                PaymentMethodRef = new IntuitData.ReferenceType
                {
                    Value = source.PaymentMethod
                },
                SalesTermRef = new IntuitData.ReferenceType
                {
                    Value = source.Terms
                },
                BillAddr = new IntuitData.PhysicalAddress
                {
                    Country = source.BillingCountry,
                    City = source.BillingCity,
                    PostalCode = source.BillingZipCode,
                    CountrySubDivisionCode = source.BillingState,
                    Line1 = source.BillingStreet
                },
                ShipAddr = new IntuitData.PhysicalAddress
                {
                    Country = source.ShippingCountry,
                    City = source.ShippingCity,
                    PostalCode = source.ShippingZipCode,
                    CountrySubDivisionCode = source.ShippingState,
                    Line1 = source.ShippingStreet
                },
                Notes = source.Notes
            };
            
            return customer;
        }
    }
}
