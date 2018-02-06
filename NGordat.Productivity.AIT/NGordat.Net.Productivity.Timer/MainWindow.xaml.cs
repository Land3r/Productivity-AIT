using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NGordat.Net.Productivity.Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The path of the file used to save the timer value.
        /// </summary>
        private static string FileName = @"timer.save";

        /// <summary>
        /// Gets or sets whether or not the timer is running.
        /// </summary>
        private bool IsRunning { get; set; } = true;

        /// <summary>
        /// Gets or sets the number of worked days in the month.
        /// </summary>
        private int NumberOfWorkedDays { get; set; } = 0;

        /// <summary>
        ///  Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            // Designer.
            InitializeComponent();

            // Event binding.
            Closing += SetValueToFile;
            Deactivated += Window_Deactivated;
            WorkedDaysUpDown.ValueChanged += WorkedDaysUpDown_ValueChanged;

            NumberOfWorkedDays = GetWorkingDays(DateTime.Now);
            WorkedDaysUpDown.Value = NumberOfWorkedDays;
        }

        /// <summary>
        /// Event receiver for updating the number of worked days.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void WorkedDaysUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            NumberOfWorkedDays = (int)e.NewValue;
            UpdateDailyObjective();
        }

        private void UpdateDailyObjective()
        {
            int dailyMinutes = (int)Properties.Settings.Default.MonthlyMinutesTime / NumberOfWorkedDays;
            this.DailyObjective.Content = "Daily Objective: " + ToReadableTime(dailyMinutes * 60);
        }

        /// <summary>
        /// Gets the number of working days on a month.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to look for opened days.</param>
        /// <returns>The number of opened days in this month.</returns>
        private int GetWorkingDays(DateTime date)
        {
            int dayCouter = 0;
            DateTime start, end;
            start = new DateTime(date.Year, date.Month, 1);
            end = new DateTime(date.Year, date.Month + 1, 1).AddDays(-1);

            while (end >= start)
            {
                if (end.DayOfWeek != DayOfWeek.Saturday && end.DayOfWeek != DayOfWeek.Sunday)
                {
                    dayCouter++;
                }

                end = end.AddDays(-1);
            }

            return dayCouter;
        }

        /// <summary>
        /// Event receiver for when the window is loaded.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int monthlyMinutesObjective = (int)Properties.Settings.Default.MonthlyMinutesTime;
            MonthlyObjective.Content = "Monthly Objective : " + ToReadableTime(monthlyMinutesObjective * 60);

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();

            Timer = RetrieveValueFromFile();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window) sender;
            window.Topmost = true;
        }

        /// <summary>
        /// Reads the value of the timer from a file.
        /// </summary>
        /// <returns>The value of the timer.</returns>
        private int RetrieveValueFromFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader(FileName))
                {
                    string line = sr.ReadToEnd();
                    int retrievedValue = int.Parse(line);
                    return retrievedValue;
                }
            }
            catch (FileNotFoundException)
            {
                File.Create(FileName);
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Saves the current value of the timer to a file, in order to be retrieved for the next execution.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event.</param>
        private void SetValueToFile(object sender, CancelEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(FileName))
            {
                sw.Write(Timer);
            }
        }

        /// <summary>
        /// Gets or sets the value of the internal timer used.
        /// </summary>
        private int Timer { get; set; } = 0;

        /// <summary>
        /// Event receiver for the timer update.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                Timer++;
            }
            TimerLabel.Content = this.ToReadableTime(Timer);

            if (Timer > Properties.Settings.Default.MonthlyMinutesTime)
            {
                ObjectiveCompleteLabel.Visibility = Visibility.Visible;
            }
            else
            {
                ObjectiveCompleteLabel.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Parses the number of seconds in order to display a properly formatted datetime.
        /// </summary>
        /// <param name="secondTimer">The number of seconds.</param>
        /// <returns>The time as a readable string.</returns>
        protected string ToReadableTime(int secondTimer)
        {
            int seconds, minutes, hours = 0;
            minutes = secondTimer / 60;
            hours = minutes / 60;
            minutes = minutes % 60;
            seconds = secondTimer % 60;
            
            return string.Format("{0}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            this.IsRunning = true;
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            this.IsRunning = false;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to reset the timer?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.IsRunning = false;
                this.Timer = 0;
            }
        }
    }
}
