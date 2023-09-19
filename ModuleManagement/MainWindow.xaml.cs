using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using ModuleData;


namespace ModuleManagement
{
    public partial class MainWindow : Window
    {
        List<Library> libraries = new List<Library>();
        List<WorkedHours> workedHoursList = new List<WorkedHours>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Display.Visibility = Visibility.Collapsed;
            ModuleDetails.Visibility = Visibility.Visible;
            Login.Visibility = Visibility.Collapsed;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            AddModule();
        }

        private void AddModule()
        {
            string moduleCode = txtModCode.Text;
            string moduleName = txtModuleName.Text;
            int numCredits;
            int classHours;
            int numWeeks;
            DateTime startDate = dpStartDate.SelectedDate ?? DateTime.Today;

            if (int.TryParse(txtCredits.Text, out numCredits) &&
                int.TryParse(txtClassHours.Text, out classHours) &&
                int.TryParse(txtNumWeeks.Text, out numWeeks))
            {
                // All parsing was successful, continue with your code here
                Library newLibrary = new Library(moduleCode, moduleName, numCredits, classHours, numWeeks, startDate);
                libraries.Add(newLibrary);
                MessageBox.Show("Module details have been saved.");

                // Clear input fields
                txtModCode.Clear();
                txtModuleName.Clear();
                txtCredits.Clear();
                txtClassHours.Clear();
                txtNumWeeks.Clear();
                dpStartDate.SelectedDate = null;
            }
            else
            {
                MessageBox.Show("Please enter valid numbers for credits, class hours, and number of weeks.", "Invalid Entry", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            DisplayAll();
        }


        private void DisplayAll()
        {
            display.Items.Clear();
            int moduleNo = 1;

            display.Items.Add("Modules In The System");
            foreach (var module in libraries)
            {
                display.Items.Add($"{moduleNo}. Module Code: {module.ModuleCode}");
                display.Items.Add($" Module Name: {module.ModuleName}");
                display.Items.Add($"Credits: {module.NumCredits}");
                display.Items.Add($"Class Hours Per Week: {module.ClassHours}");
                display.Items.Add($"Number of Weeks: {module.NumWeeks}");
                display.Items.Add($"Start Date: {module.StartDate.ToShortDateString()}");
                display.Items.Add($"Self-Study Hours Per Week: {module.SelfStudyHoursPerWeek()}");
                display.Items.Add("---------------------------------------");

                moduleNo++;
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // New event handler for recording hours worked
        private void btnRecordHours_Click(object sender, RoutedEventArgs e)
        {
            string moduleCode = txtModCode.Text;
            DateTime dateWorked = dpDateWorked.SelectedDate ?? DateTime.Today;

            if (double.TryParse(txtHoursWorked.Text, out double hoursWorked))
            {
                int weekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateWorked, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                WorkedHours workedHours = new WorkedHours
                {
                    ModuleCode = moduleCode,
                    Date = dateWorked,
                    HoursWorked = hoursWorked,
                    WeekNumber = weekNumber
                };
                workedHoursList.Add(workedHours);
                MessageBox.Show("Hours worked have been recorded.");
                //// Calculate and display remaining self-study hours
                //DisplayRemainingSelfStudyHours();
            }
            else
            {
                MessageBox.Show("Please enter a valid number for hours worked.", "Invalid Entry", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // New method to calculate remaining self-study hours for the current week
        private double CalculateRemainingSelfStudyHours(string moduleCode)
        {
            int currentWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

            double totalSelfStudyHours = libraries
                .Where(module => module.ModuleCode == moduleCode)
                .Select(module => module.SelfStudyHoursPerWeek())
                .FirstOrDefault();

            double recordedHoursForCurrentWeek = workedHoursList
                .Where(entry => entry.ModuleCode == moduleCode && entry.WeekNumber == currentWeek)
                .Sum(entry => entry.HoursWorked);

            double remainingHours = totalSelfStudyHours - recordedHoursForCurrentWeek;
            return remainingHours;
        }

        // New method to display remaining self-study hours for each module for the current week
        private void DisplayRemainingSelfStudyHours()
        {
            RemainingHoursListBox.Items.Clear();

            foreach (var module in libraries)
            {
                double remainingHours = CalculateRemainingSelfStudyHours(module.ModuleCode);

                RemainingHoursListBox.Items.Add($"Module Code: {module.ModuleCode}");
                RemainingHoursListBox.Items.Add($"Module Name: {module.ModuleName}");
                RemainingHoursListBox.Items.Add($"Remaining Self-Study Hours for Current Week: {remainingHours}");
                RemainingHoursListBox.Items.Add("---------------------------------------");
            }
            // Add the saved btnRecordHours_Click details
            foreach (var entry in workedHoursList)
            {
                RemainingHoursListBox.Items.Add($"Module Code: {entry.ModuleCode}");
                RemainingHoursListBox.Items.Add($"Date Worked: {entry.Date.ToShortDateString()}");
                RemainingHoursListBox.Items.Add($"Hours Worked: {entry.HoursWorked}");
                RemainingHoursListBox.Items.Add("---------------------------------------");
            }
        }

        private void btnProceedToModules_Click(object sender, RoutedEventArgs e)
        {
            // Make the Display grid visible to show the displayed modules
            Display.Visibility = Visibility.Visible;

            // Hide the Module Details grid (if needed)
            ModuleDetails.Visibility = Visibility.Collapsed;
        }

        // Back button click event handler to return to login
        private void btnBackToLogin_Click(object sender, RoutedEventArgs e)
        {
            Display.Visibility = Visibility.Collapsed;
            ModuleDetails.Visibility = Visibility.Collapsed;
            Login.Visibility = Visibility.Visible;
        }

        // Back button click event handler to return to modules
        private void btnBackToModules_Click(object sender, RoutedEventArgs e)
        {
            Display.Visibility = Visibility.Collapsed;
            ModuleDetails.Visibility = Visibility.Visible;
            Login.Visibility = Visibility.Collapsed;
            RemainingHoursGrid.Visibility = Visibility.Collapsed;
        }
        private void btnShowRemainingHours_Click(object sender, RoutedEventArgs e)
        {
            // Call the method to display remaining self-study hours
            DisplayRemainingSelfStudyHours();

            // Hide other grids and show the RemainingHoursGrid
            Display.Visibility = Visibility.Collapsed;
            ModuleDetails.Visibility = Visibility.Collapsed;
            RemainingHoursGrid.Visibility = Visibility.Visible;

           
        }
        private void btnBackToPrevious_Click(object sender, RoutedEventArgs e)
        {
            Display.Visibility = Visibility.Visible;
            ModuleDetails.Visibility = Visibility.Collapsed;
            Login.Visibility = Visibility.Collapsed;
            RemainingHoursGrid.Visibility = Visibility.Collapsed;
        
        }
    }
}

