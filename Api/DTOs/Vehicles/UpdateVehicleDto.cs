using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs.Vehicles;

public record UpdateVehicleDto( string? Brand, string? Model, int? Year, string? Vin, double? Mileage);