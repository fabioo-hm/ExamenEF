using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Vehicles;

public sealed record CreateCity(string Brand, string Model,int Year, string Vin, double Mileage, Guid CustomerId) : IRequest<Guid>;