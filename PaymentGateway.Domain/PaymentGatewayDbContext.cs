using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain
{
    public class PaymentGatewayDbContext : DbContext
    {
        public PaymentGatewayDbContext (DbContextOptions<PaymentGatewayDbContext> options)
            : base(options)
        {
        }

        public DbSet<PaymentTransaction> PaymentTransaction { get; set; } = default!;
    }
}
