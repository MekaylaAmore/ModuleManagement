using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleData
{
    public class Class1
    {
        // Properties and methods for module data
    }

    public class WorkedHours
    {
        // Properties and methods for worked hours data
        public string ModuleCode { get; set; }
        public DateTime Date { get; set; }
        public double HoursWorked { get; set; }
        public int WeekNumber { get; set; }
    }
}