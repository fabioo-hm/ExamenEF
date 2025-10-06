using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities;
public class Vehicle
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string? Brand { get; private set; }
    public string? Model { get; private set; }
    public int Year { get; private set; }
    public string? Vin { get; private set; }
    public double Mileage { get; private set; }

    public Guid CustomerId { get; private set; }
    public Customer? Customer { get; private set; }

    public ICollection<ServiceOrder> ServiceOrders { get; private set; } = new List<ServiceOrder>();

    public Vehicle() { }

    public Vehicle(string brand, string model, int year, string vin, double mileage, Guid customerId)
    {
        Brand = brand;
        Model = model;
        Year = year;
        Vin = vin;
        Mileage = mileage;
        CustomerId = customerId;
    }
}