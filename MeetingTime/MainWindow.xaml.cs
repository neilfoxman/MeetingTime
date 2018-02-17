using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace MeetingTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        Boolean meetingOngoing = false; // State of meeting
        Stopwatch sw = new Stopwatch(); // Stopwatch object for starting and stopping time keeping
        TimeSpan timeElapsed = new TimeSpan(); // timespan object to store meeting time

        const int nPayGrades = 5;
        int[] numAttendees = new int[nPayGrades];
        long tphr = TimeSpan.TicksPerHour; // store ticks per hour for conversions

        public MainWindow()
        {
            InitializeComponent();

            // Load Defaults
            loadDefs();
            
            // Setup timer thread
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick); // Set handler
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50); // Set interval
            dispatcherTimer.Start(); // Start thread

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            if (meetingOngoing) // If meeting has started
            {
                // Calculate the time elapsed
                timeElapsed = sw.Elapsed;
                long ticksElapsed = timeElapsed.Ticks;

                // Update time elapsed
                lblTimeElapsed.Content = String.Format("{0:00}:{1:00}:{2:00}", timeElapsed.Hours, timeElapsed.Minutes, timeElapsed.Seconds);

                // Get the pay rates and store them
                try
                {
                    double[] rates = new double[nPayGrades];
                    rates[0] = tbToDouble(tbRate0);
                    rates[1] = tbToDouble(tbRate1);
                    rates[2] = tbToDouble(tbRate2);
                    rates[3] = tbToDouble(tbRate3);
                    rates[4] = tbToDouble(tbRate4);

                    // Using pay rate, convert time elapsed to burned cash
                    double[] burned = new double[nPayGrades];
                    double totalBurn = 0;
                    for (int i = 0; i < nPayGrades; i++)
                    {
                        burned[i] = ticksElapsed / (double)tphr * rates[i] * (double)numAttendees[i]; // burned cash = tickselapsed * (1hr/36*10^9 ticks) * ($ hourly rate/1 hr) * numAttendees
                        totalBurn += burned[i];
                    }

                    // Update cash burned
                    lblBurn0.Content = burned[0].ToString("C"); // Display as currency
                    lblBurn1.Content = burned[1].ToString("C");
                    lblBurn2.Content = burned[2].ToString("C");
                    lblBurn3.Content = burned[3].ToString("C");
                    lblBurn4.Content = burned[4].ToString("C");

                    // Total Cash Burned
                    lblTotalBurn.Content = totalBurn.ToString("C");
                } catch (FormatException ex)
                {
                    MessageBox.Show("One of the Rates entered is not a number.\nPlease correct the Rates and restart/resume the meeting.");

                    meetingOngoing = false;
                }

            }


            // Update Number of Attendees
            lblAttendees0.Content = numAttendees[0];
            lblAttendees1.Content = numAttendees[1];
            lblAttendees2.Content = numAttendees[2];
            lblAttendees3.Content = numAttendees[3];
            lblAttendees4.Content = numAttendees[4];

        }

        private void StartMeeting_Click(object sender, RoutedEventArgs e)
        {
            //meetingStartTime = DateTime.Now;
            sw.Reset();
            sw.Start();
            meetingOngoing = true;
        }

        private void StopMeeting_Click(object sender, RoutedEventArgs e)
        {
            sw.Stop();
            meetingOngoing = false;
        }

        private void ResumeMeeting_Click(object sender, RoutedEventArgs e)
        {
            sw.Start();
            meetingOngoing = true;
        }

        private void Add_Click_0(object sender, RoutedEventArgs e)
        {
            numAttendees[0] += 1;
        }

        private void Add_Click_1(object sender, RoutedEventArgs e)
        {
            numAttendees[1] += 1;
        }

        private void Add_Click_2(object sender, RoutedEventArgs e)
        {
            numAttendees[2] += 1;
        }

        private void Add_Click_3(object sender, RoutedEventArgs e)
        {
            numAttendees[3] += 1;
        }

        private void Add_Click_4(object sender, RoutedEventArgs e)
        {
            numAttendees[4] += 1;
        }



        private void Rm_Click_0(object sender, RoutedEventArgs e)
        {
            numAttendees[0] -= 1;
        }

        private void Rm_Click_1(object sender, RoutedEventArgs e)
        {
            numAttendees[1] -= 1;
        }

        private void Rm_Click_2(object sender, RoutedEventArgs e)
        {
            numAttendees[2] -= 1;
        }

        private void Rm_Click_3(object sender, RoutedEventArgs e)
        {
            numAttendees[3] -= 1;
        }

        private void Rm_Click_4(object sender, RoutedEventArgs e)
        {
            numAttendees[4] -= 1;
        }

        private void saveDefs()
        {
            try
            {
                Properties.Settings.Default.defRate0 = tbRate0.Text;
                Properties.Settings.Default.defRate1 = tbRate1.Text;
                Properties.Settings.Default.defRate2 = tbRate2.Text;
                Properties.Settings.Default.defRate3 = tbRate3.Text;
                Properties.Settings.Default.defRate4 = tbRate4.Text;
                Properties.Settings.Default.Save();
            } catch (NullReferenceException ex)
            {
                MessageBox.Show("NullReferenceException at " + ex.Source);

            }

        }

        private void loadDefs()
        {
            tbRate0.Text = Properties.Settings.Default.defRate0;
            tbRate1.Text = Properties.Settings.Default.defRate1;
            tbRate2.Text = Properties.Settings.Default.defRate2;
            tbRate3.Text = Properties.Settings.Default.defRate3;
            tbRate4.Text = Properties.Settings.Default.defRate4;
        }


        private void tbCommonLoseFocus(object sender, RoutedEventArgs e)
        {
            saveDefs();
        }

        private double tbToDouble(TextBox tb)
        {
            if (tb.Text == "")
            {
                return (0);
            }
            else
            {
                double dub = Convert.ToDouble(tb.Text);
                return (dub);
            }
        }
    }
}
