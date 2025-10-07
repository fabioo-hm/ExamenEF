using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs.Customers;
public record UpdateCustomerDto( string? Name, string? Email, string? Phone
);