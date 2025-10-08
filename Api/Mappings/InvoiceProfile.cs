using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.Invoices;
using AutoMapper;
using Domain.Entities;

namespace Api.Mappings;
public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<Invoice, InvoiceDto>();

        CreateMap<CreateInvoiceDto, Invoice>()
            .ConstructUsing(src => new Invoice(
                src.ServiceOrderId,
                src.LaborCost,
                src.PartsTotal,
                src.PaymentMethod!
            ));
        
        CreateMap<UpdateInvoiceDto, Invoice>()
            .ConstructUsing(src => new Invoice(
                src.ServiceOrderId,
                src.LaborCost,
                src.PartsTotal,
                src.PaymentMethod!
            ));
    }
}
