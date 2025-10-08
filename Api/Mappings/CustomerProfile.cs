using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.Customers;
using AutoMapper;
using Domain.Entities;

namespace Api.Mappings;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerDto>();

        CreateMap<CreateCustomerDto, Customer>()
            .ConstructUsing(src => new Customer(
                src.Name,
                src.Email,
                src.Phone
            ));

        CreateMap<UpdateCustomerDto, Customer>()
            .ConstructUsing(src => new Customer(
                src.Name!,
                src.Email!,
                src.Phone!
            ));
    }
}
