using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWorkWindowsFormsApp
{
    internal class DataContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<HourlyEmployee> HourlyEmployees { get; set; }
        public DbSet<SalariedEmployee> SalariedEmployees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=courseWork;Username=postgres;Password=1111;");
        }
    }
}
