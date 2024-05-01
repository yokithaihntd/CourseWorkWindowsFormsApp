using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWorkWindowsFormsApp
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string Position { get; set; }

        public virtual double CalculateTotalSalary()
        {
            return 0;
        }
    }

    public class HourlyEmployee : Employee
    {
        public int WorkedHours { get; set; }
        public double HourlyRate { get; set; }
        public double Salary { get; set; }

        public override double CalculateTotalSalary()
        {
            return WorkedHours * HourlyRate;
        }
    }

    public class SalariedEmployee : Employee
    {
        public double Salary { get; set; }

        public override double CalculateTotalSalary()
        {
            return Salary;
        }
    }
}
