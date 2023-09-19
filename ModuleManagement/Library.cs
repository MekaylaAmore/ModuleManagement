using System;

namespace ModuleManagement
{
    internal class Library
    {
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public int NumCredits { get; set; }
        public int ClassHours { get; set; }
        public int NumWeeks { get; set; }
        public DateTime StartDate { get; set; }

        public Library(string moduleCode, string moduleName, int numCredits, int classHours, int numWeeks, DateTime startDate)
        {
            ModuleCode = moduleCode;
            ModuleName = moduleName;
            NumCredits = numCredits;
            ClassHours = classHours;
            NumWeeks = numWeeks;
            StartDate = startDate;
        }

        public double SelfStudyHoursPerWeek()
        {
            if (NumWeeks > ClassHours)
            {
                return (NumCredits * 10.0) / (NumWeeks - ClassHours);
            }
            return 0;
        }
    }
}

