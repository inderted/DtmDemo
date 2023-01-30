using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DtmDemo.WebApi.Models;

namespace DtmDemo.WebApi.Data
{
    public class DtmDemoWebApiContext : DbContext
    {
        public DtmDemoWebApiContext (DbContextOptions<DtmDemoWebApiContext> options)
            : base(options)
        {
        }

        public DbSet<DtmDemo.WebApi.Models.BankAccount> BankAccount { get; set; } = default!;
    }
}
