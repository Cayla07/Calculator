using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace EUPINADO
{
    public partial class Form1 : Form
    {
        public static bool isError = false;
        public static bool isUndefined = false;
        bool isOperatorUsed = false;
        string CurrentExpression = "";
        public static List<double> Equation = new List<double>();
        public static List<string> Operations = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void DeciBtn_Click(object sender, EventArgs e)
        {
            if (isError)
                return;

            string[] parts = CurrentExpression.Split(new char[] { '+', '-', '*', '/' }, StringSplitOptions.RemoveEmptyEntries);
            string lastPart = parts.Length > 0 ? parts[parts.Length - 1] : "";

            if (!lastPart.Contains("."))
            {
                if (string.IsNullOrEmpty(lastPart))
                {
                    CurrentExpression += "0.";
                    Tbx_Equation.Text += "0.";
                }
                else
                {
                    CurrentExpression += ".";
                    Tbx_Equation.Text += ".";
                }
            }
        }
        private void NegPosBtn_Click(object sender, EventArgs e)
        {
            if (isError || string.IsNullOrEmpty(CurrentExpression)) return;

            int lastOperatorIndex = CurrentExpression.LastIndexOfAny(new char[] { '+', '-', '*', '/' });
            string numberToToggle = lastOperatorIndex == -1 ? CurrentExpression : CurrentExpression.Substring(lastOperatorIndex + 1);

            while (numberToToggle.Contains("---"))
            {
                numberToToggle = numberToToggle.Replace("---", "-");
            }
            if (numberToToggle.StartsWith("--"))
            {
                numberToToggle = "+" + numberToToggle.Substring(2); 
            }
            else if (numberToToggle.StartsWith("-"))
            {
                numberToToggle = numberToToggle.Substring(1); 
            }
            else
            {
                numberToToggle = "-" + numberToToggle; 
            }

            if (lastOperatorIndex == -1)
            {
                CurrentExpression = numberToToggle; 
            }
            else
            {
                CurrentExpression = CurrentExpression.Substring(0, lastOperatorIndex + 1) + numberToToggle;
            }

            Tbx_Equation.Text = CurrentExpression;
        }


        private void NumBtn_Click(object sender, EventArgs e)
        {
            if (isError)
                return;

            Button numButton = (Button)sender;
            string value = numButton.Text;

            if (Tbx_Equation.Text == "0" && value != ".")
            {
                Tbx_Equation.Text = value;
            }
            else
            {
                Tbx_Equation.Text += value;
            }
            CurrentExpression += value;
        }

        private void EqualBtn_Click(object sender, EventArgs e)
        {
            if (isError)
                return;

            try
            {
                if (string.IsNullOrEmpty(CurrentExpression) || "+-*/".Contains(CurrentExpression[CurrentExpression.Length - 1]))
                {
                    Tbx_Result.Text = "Error";
                    Tbx_Equation.Text = "Incomplete Expression";
                    isError = true;
                    return;
                }

                if (CurrentExpression.Contains("/0"))
                {
                    Tbx_Result.Text = "Undefined";
                    isError = true;
                    return;
                }

                var result = new DataTable().Compute(CurrentExpression, null);

                Tbx_Result.Text = Math.Round(Convert.ToDouble(result), 2).ToString();
            }
            catch
            {
                Tbx_Result.Text = "Error";
                isError = true;
            }
        }

        private void BckspcBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Tbx_Equation.Text) || Tbx_Equation.Text == "0")
            {
                return;
            }

            CurrentExpression = CurrentExpression.Length > 1
                ? CurrentExpression.Substring(0, CurrentExpression.Length - 1)
                : "";

            Tbx_Equation.Text = Tbx_Equation.Text.Length > 1
                ? Tbx_Equation.Text.Substring(0, Tbx_Equation.Text.Length - 1)
                : "0";
        }

        private void CancelEntryBtn_Click(object sender, EventArgs e)
        {
            ResetCalculator();
        }

        private void OperatorBtn_Click(object sender, EventArgs e)
        {
            if (isError)
                return;

            Button OperationBtn = (Button)sender;
            string operatorValue = OperationBtn.Text;

            if (!string.IsNullOrEmpty(CurrentExpression) && !"+-*/".Contains(CurrentExpression[CurrentExpression.Length - 1]))
            {
                CurrentExpression += operatorValue;
                Tbx_Equation.Text += operatorValue;
                isOperatorUsed = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(CurrentExpression) && "+-*/".Contains(CurrentExpression[CurrentExpression.Length - 1]))
                {
                    CurrentExpression = CurrentExpression.Substring(0, CurrentExpression.Length - 1) + operatorValue;
                    Tbx_Equation.Text = Tbx_Equation.Text.Substring(0, Tbx_Equation.Text.Length - 1) + operatorValue;
                }
                else
                {
                    Tbx_Result.Text = "Error";
                    Tbx_Equation.Text = "Invalid Operator Sequence";
                    isError = true;
                }
            }
        }

        private void SqrtBtn_Click(object sender, EventArgs e)
        {
            if (isError) return;

            string equation = Tbx_Equation.Text;

            char[] operators = { '+', '-', '*', '/' };
            int operatorIndex = equation.IndexOfAny(operators);

            if (operatorIndex != -1)
            {
                foreach (char op in operators)
                {
                    if (equation.Contains(op))
                    {
                        string[] parts = equation.Split(op);
                        if (parts.Length == 2 && double.TryParse(parts[0], out double num1) && double.TryParse(parts[1], out double num2))
                        {
                            num1 = Math.Sqrt(num1);  
                            double result = 0;

                            switch (op)
                            {
                                case '+': 
                                    result = num1 + num2; break;
                                case '-': 
                                    result = num1 - num2; break;
                                case '*': 
                                    result = num1 * num2; break;
                                case '/':
                                    if (num2 == 0) 
                                    { 
                                        Tbx_Result.Text = "Error"; isError = true;
                                        return; 
                                    }
                                    result = num1 / num2;
                                    break;
                            }

                            Tbx_Result.Text = Math.Round(result, 2).ToString();
                            Tbx_Equation.Text = "√" + parts[0] + " " + op + " " + parts[1];
                            return;
                        }
                    }
                }
                Tbx_Result.Text = "Invalid Input"; 
            }
            else if (double.TryParse(equation, out double num))
            {
                if (num < 0)
                {
                    Tbx_Result.Text = "Invalid Input";  
                    isError = true;
                }
                else
                {
                    Tbx_Result.Text = Math.Round(Math.Sqrt(num), 2).ToString();
                    Tbx_Equation.Text = "√" + equation;
                }
            }
            else
            {
                Tbx_Result.Text = "Invalid Input";  
                isError = true;
            }
        }
        private void ResetCalculator()
        {
            Tbx_Equation.Text = "0";
            CurrentExpression = "";
            Tbx_Result.Text = "";
            Equation.Clear();
            Operations.Clear();
            isUndefined = false;
            isError = false;
            isOperatorUsed = false;
        }
    }
}
