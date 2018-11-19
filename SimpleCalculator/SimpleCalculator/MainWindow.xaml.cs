using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        /// <summary>
        ///  Keeps track if the value should be presented €??,?? instead of ??,??
        /// </summary>
        bool EuroToggle = false;
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
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
            if (EuroToggle) res += "€ ";
            res += StringifyList(ValueBuffer) + " ";
            res += StringifyList(OperatorBuffer) + " ";
            this.Current.Text = res;
        }
        /// <summary>
        /// This function gets called whenever the user enters '0-9'
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
        /// This function gets called whenever the user enters ','
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
        /// This function gets called whenever the user enters '+,-,×,/,%'
        /// </summary>
        private void Operator(object sender, RoutedEventArgs e)
        {
            //try to clear valuebuffer
            BuffValue();
            Button Target = sender as Button;
            string NewData = Target.Content.ToString();
            double arg = 0;
            if (Equation.Count <= 0 || double.TryParse(Equation[Equation.Count - 1], out arg))
            {

                // Add to array if equation is empty or last entered value was a double
                Equation.Add(NewData);
            }
            else
            {
                // >0 and lat value was not a double
                if (NewData == "-")
                {
                    switch (Equation[Equation.Count - 1])
                    {
                        case "-":
                            Equation[Equation.Count() - 1] = "+";
                            break;
                        case "+":
                            Equation[Equation.Count - 1] = "-";
                            break;
                        default:
                            Equation.Add(NewData);
                            break;
                    }
                }
                else
                {
                    while (Equation.Count > 0 && !double.TryParse(Equation[Equation.Count - 1], out arg))
                    {
                        Equation.RemoveAt(Equation.Count - 1);
                    }
                    Equation.Add(NewData);

                }
            }

            UpdateCurrent();
        }

        private void BuffValue()
        {
            if (ValueBuffer.Count > 0 && EuroToggle)
            {
                Equation.Add("€");
                EuroToggle = false;
            }
            PushBuffer(ValueBuffer);
        }

        /// <summary>
        /// This function gets called whenever the user enters '='
        /// </summary>
        private void Solve(object sender, RoutedEventArgs e)
        {
            bool isValuta = false;
            BuffValue();
            List<string> ErasableEquation = new List<string>(Equation);
            double arg;
            // Check for divide By 0 errors
            for (int i = 1; i < Equation.Count; i++)
            {
                if (double.TryParse(ErasableEquation[i], out arg) && arg == 0)
                {
                    if (ErasableEquation[i - 1] == "/" || ErasableEquation[i - 1] == "€" & ErasableEquation[i - 2] == "/")
                    {
                        Clear(sender, e);
                        this.Current.Text = "DivideByZeroError";
                        return;
                    }
                }
            }
            if (ErasableEquation.Contains("€"))
            {
                ErasableEquation.RemoveAll(x => x == "€");
                isValuta = true;
            }
            while (!double.TryParse(ErasableEquation.Last(), out arg))
            {
                ErasableEquation.RemoveAt(ErasableEquation.Count - 1);
            }
            // First pass
            for (int i = 0; i < ErasableEquation.Count; i++)
            {
                if (ErasableEquation[i] == "-")
                {
                    double outp = 0;
                    if (double.TryParse(ErasableEquation[i + 1], out outp))
                    {
                        ErasableEquation[i + 1] = (outp * -1).ToString();
                        ErasableEquation.RemoveAt(i);
                    }
                }
            }
            double left;
            double right;
            for (int i = 1; i < ErasableEquation.Count - 1; i++)
            {
                if ("×/%".Contains(ErasableEquation[i]))
                {
                    left = double.Parse(ErasableEquation[i - 1]);
                    right = double.Parse(ErasableEquation[i + 1]);
                    switch (ErasableEquation[i])
                    {
                        case "×":
                            ErasableEquation[i] = (left * right).ToString();
                            break;
                        case "/":
                            ErasableEquation[i] = (left / right).ToString();
                            break;
                        case "%":
                            ErasableEquation[i] = (left * right * 0.01).ToString();
                            break;
                        default:
                            break;
                    }
                    ErasableEquation.RemoveAt(i + 1);
                    ErasableEquation.RemoveAt(i - 1);
                    i--;
                }
            }
            for (int i = 1; i < ErasableEquation.Count - 1; i++)
            {
                if ("+".Contains(ErasableEquation[i]))
                {
                    left = double.Parse(ErasableEquation[i - 1]);
                    right = double.Parse(ErasableEquation[i + 1]);
                    switch (ErasableEquation[i])
                    {
                        case "+":
                            ErasableEquation[i] = (left + right).ToString();
                            break;
                        case "/":
                            ErasableEquation[i] = (left / right).ToString();
                            break;
                        case "%":
                            ErasableEquation[i] = (left * right * 0.01).ToString();
                            break;
                        default:
                            break;
                    }
                    ErasableEquation.RemoveAt(i + 1);
                    ErasableEquation.RemoveAt(i - 1);
                    i--;
                }
            }
            double res = 0;
            ErasableEquation.RemoveAll(x => "€+/×%-".Contains(x));
            foreach (var item in ErasableEquation)
            {
                res += double.Parse(item);
            }
            History.Text += $"{res}\n";
        }
        /// <summary>
        /// This function gets called whenever the user enters 'C'
        /// </summary>
        private void Clear(object sender, RoutedEventArgs e)
        {
            //Clear all current values
            ValueBuffer.Clear();
            OperatorBuffer.Clear();
            Equation.Clear();
            EuroToggle = false;
            UpdateCurrent();
        }
        /// <summary>
        /// This function gets called whenever the user enters '€'
        /// </summary>
        private void EU_Click(object sender, RoutedEventArgs e)
        {
            EuroToggle = !EuroToggle;
            UpdateCurrent();
        }
    }
}
