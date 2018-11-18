using System;
using System.Collections.Generic;
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

namespace SimpleCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Holds the latest number input before it is added to the Equation
        /// </summary>
        List<char> ValueBuffer = new List<char>();
        /// <summary>
        /// Holds entered operators before they are added to the equation
        /// </summary>
        List<char> OperatorBuffer = new List<char>(2);
        /// <summary>
        /// A list of values and operators in string format
        /// </summary>
        List<string> Equation = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This function gets called when the user clicks on the buttons 0-9,
        /// </summary>
        private void Number(object sender, RoutedEventArgs e)
        {
            PushBuffer(OperatorBuffer);
            Button Target = sender as Button;
            char newval = Target.Content.ToString()[0];
            ValueBuffer.Add(newval);
            //Remove Leading 0's
            while (ValueBuffer.Count > 1 && ValueBuffer[0] == '0' && ValueBuffer[1] == '0')
            {
                ValueBuffer.RemoveAt(0);
            }
            UpdateCurrent();
        }
        /// <summary>
        /// Pushes the data from the buffer to the Equation
        /// </summary>
        private void PushBuffer(List<char> buffer)
        {
            if (buffer.Count > 0)
            {
                string a = StringifyList(buffer);
                buffer.Clear();
                Equation.Add(a);
            }
        }
        /// <summary>
        /// Converts each item to a string
        /// </summary>
        private string StringifyList(List<char> buffer)
        {
            string result = "";
            foreach (var item in buffer)
            {
                result += item;
            }
            return result;
        }
        /// <summary>
        /// Update the input output fields to reflect the new data
        /// </summary>
        private void UpdateCurrent()
        {
            StringBuilder b = new StringBuilder();
            string res = "";
            foreach (var item in Equation)
            {
                res += item + " ";
            }
            res += StringifyList(ValueBuffer)+ " ";
            res += StringifyList(OperatorBuffer)+" ";
            this.Current.Text = res;
        }
        /// <summary>
        /// This event get's called whenever the ',' Button is pressed
        /// </summary>
        private void Comma(object sender, RoutedEventArgs e)
        {
            Button Target = sender as Button;
            char comma = Target.Content.ToString()[0];
            if (!ValueBuffer.Contains(comma))
            {
                ValueBuffer.Add(comma);
            }
            UpdateCurrent();
        }
        /// <summary>
        /// This function gets called whenever +,-,×,/
        /// </summary>
        private void Operator(object sender, RoutedEventArgs e)
        {
            PushBuffer(ValueBuffer);
            Button Target = sender as Button;
            char newOp = Target.Content.ToString()[0];
            while (OperatorBuffer.Count >= 2)
            {
                OperatorBuffer.RemoveAt(0);
            }
            if (OperatorBuffer.Count <= 0)
            {
                OperatorBuffer.Add(newOp);
            }
            else
            {
                if (newOp == '-')
                {
                    if (OperatorBuffer[0] == '+')
                    {
                        OperatorBuffer[0] = '-';
                    }
                    else
                    {
                        if (OperatorBuffer[0] == '-')
                        {
                            OperatorBuffer[0] = '+';
                        }
                        else
                        {
                            OperatorBuffer.Add('-');
                        }
                    }
                }
                else
                {
                    OperatorBuffer[0] = newOp;
                }
            }
            UpdateCurrent();
        }
        private void Solve(object sender, RoutedEventArgs e)
        {

        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            ValueBuffer.Clear();
            OperatorBuffer.Clear();
            Equation.Clear();
            UpdateCurrent();
        }
    }
}
