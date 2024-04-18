using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using CircularProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text.RegularExpressions;

namespace EcoSort
{
    public partial class Form1 : Form
    {

        private Timer progressTimer;
        public Form1()
        {
            InitializeComponent();
            getAvailablePorts();
            serialPort1 = new SerialPort();
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            StartLabelBlinking();
            //UpdateProgressBarText();
            progressTimer = new Timer();
            progressTimer.Interval = 100; // Set the interval as needed (milliseconds)
            progressTimer.Tick += timer2_Tick;
            progressTimer.Start();
            serialPort1.DataReceived += SerialPort1_DataReceived;

        }
        private bool isRed = false;
        private bool isRed2 = false;

        void getAvailablePorts()
        {
            String[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if an item is selected
            if (comboBox1.SelectedItem != null)
            {
                string selectedPort = comboBox1.SelectedItem.ToString();

                try
                {
                    serialPort1.PortName = selectedPort;
                    serialPort1.BaudRate = 9600;/* Set your desired baud rate */
                    serialPort1.Open();
                    serialPort1.DataReceived += SerialPort1_DataReceived;

                    if (serialPort1.IsOpen)
                    {
                        // Assuming circularProgressBar1 is the name of your progress bar control
                        progressBar1.Value = 100;
                    }

                    // Additional operations or UI updates after opening the serial port
                    // ...
                }
                catch (UnauthorizedAccessException)
                {
                    // Handle exception (e.g., port access denied)
                    // ...
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    // ...
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            label5.Visible = true;
            if (isRed)
            {
                label5.ForeColor = Color.White; // Change label color to white
                isRed = false;
            }
            else
            {
                label5.ForeColor = Color.Red; // Change label color to red
                isRed = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            circularProgressBar1.Value = 100;
            circularProgressBar2.Value = 100;
        }

        private async void StartLabelBlinking()
        {
            while (true)
            {
                if (serialPort1.IsOpen && circularProgressBar1.Value > 85 && circularProgressBar1.Value <= 100)
                {
                    timer1.Start(); // Start label blinking
                }
                else
                {
                    timer1.Stop(); // Stop label blinking
                    label5.ForeColor = Color.Black; // Reset label color
                    isRed = false;
                    label5.Visible = false;
                }

                if (serialPort1.IsOpen && circularProgressBar2.Value > 85 && circularProgressBar2.Value <= 100)
                {
                    timer3.Start(); // Start label blinking
                }
                else
                {
                    timer3.Stop(); // Stop label blinking
                    label6.ForeColor = Color.Black; // Reset label color
                    isRed = false;
                    label6.Visible = false;
                }

                await Task.Delay(1000); // Check every 1 second
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            circularProgressBar1.Value = 50;
            circularProgressBar2.Value = 50;
        }

        private void UpdateProgressBarText()
        {
            // Set the text of the circularProgressBar1 to match its value
            circularProgressBar1.Text = circularProgressBar1.Value.ToString() + "%";
            circularProgressBar2.Text = circularProgressBar2.Value.ToString() + "%";
        }

        

        private void timer3_Tick(object sender, EventArgs e)
        {
            label6.Visible = true;
            if (isRed2)
            {
                label6.ForeColor = Color.White; // Change label color to white
                isRed2 = false;
            }
            else
            {
                label6.ForeColor = Color.Red; // Change label color to red
                isRed2 = true;
            }
        }


        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort1.ReadLine();
            Console.WriteLine(data);

            



            string[] separated = data.Split(new string[] { "and" }, StringSplitOptions.None);
            if (separated.Length >= 2)
            {
                string firstPart = separated[0].Trim();
                string secondPart = separated[1].Trim();

                // Check if the string starts with "Distance:"
                if (firstPart.StartsWith("Distance:"))
                {
                    string firstNumber = firstPart.Substring("Distance:".Length).Trim();

                    int indexSpace = secondPart.IndexOf(' ');
                    string secondNumber = indexSpace != -1 ? secondPart.Substring(0, indexSpace) : secondPart;

                    //Console.WriteLine("First number: " + firstNumber);
                    //Console.WriteLine("Second number: " + secondNumber);


                    if (int.TryParse(firstNumber, out int distance1) && int.TryParse(secondNumber, out int distance2))
                    {
                        int maxDistance1 = 24;
                        int maxDistance2 = 18;
                        int scaledDistance1 = 100 - (int)((distance1 / (double)maxDistance1) * 100);
                        int scaledDistance2 = 100 - (int)((distance2 / (double)maxDistance2) * 100);
                        //Console.WriteLine("First number: " + scaledDistance1);
                        //Console.WriteLine("Second number: " + scaledDistance2);

                        circularProgressBar1.Invoke((MethodInvoker)delegate
                        {
                            UpdateProgressBar(scaledDistance1);
                        });
                        circularProgressBar2.Invoke((MethodInvoker)delegate
                        {
                            UpdateProgressBar2(scaledDistance2);
                        });
                    }
                    else
                    {
                        //Console.WriteLine("Invalid distance values.");
                    }
                }
                else
                {
                    //Console.WriteLine("String format is not as expected.");
                }
            }
            else
            {
                //Console.WriteLine("String format is not as expected.");
            }
        }


        private void UpdateProgressBar(int scaledDistance)
        {
            // Set the circular progress bar value within a defined range
            // Ensure that it stays within the minimum and maximum values of the progress bar
            circularProgressBar1.Value = Clamp(scaledDistance, 0, 100);
        }
        private void UpdateProgressBar2(int scaledDistance)
        {
            // Set the circular progress bar value within a defined range
            // Ensure that it stays within the minimum and maximum values of the progress bar
            circularProgressBar2.Value = Clamp(scaledDistance, 0, 100);
        }

        private int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            circularProgressBar1.Text = circularProgressBar1.Value.ToString() + "%";
            circularProgressBar2.Text = circularProgressBar2.Value.ToString() + "%";
        }

        

        private void button3_Click(object sender, EventArgs e)
        {
            serialPort1.DataReceived += SerialPort1_DataReceived;

        }
        

    }
}

