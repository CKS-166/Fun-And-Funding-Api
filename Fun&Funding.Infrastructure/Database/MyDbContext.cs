using Fun_Funding.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Infrastructure.Database
{
    public class MyDbContext: DbContext
    {
        public MyDbContext()
        {
            
        }
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            
        }
        public DbSet<TestUser> Users { get; set; }
    }
}
