using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string? Name { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public Customer() { }
    public Customer(string name, string email, string phone)
    {
        Name = name;
        Email = email;
        Phone = phone;

    }
}
