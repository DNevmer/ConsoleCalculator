using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCalc
{
    public sealed class Operators
    {
        public static readonly Operators leftBracket = new Operators("(");
        public static readonly Operators rightBracket = new Operators(")");
        public static readonly Operators minus = new Operators("-");
        public static readonly Operators plus = new Operators("+");
        public static readonly Operators multiply = new Operators("*");
        public static readonly Operators division = new Operators("/");
        public static readonly Operators ex = new Operators("^");
        public static readonly Operators cos = new Operators("cos");
        public static readonly Operators sin = new Operators("sin");
        public static readonly Operators tan = new Operators("tan");
        public static readonly Operators sqrt = new Operators("sqrt");
        public static readonly Operators comma = new Operators(",");

        private Operators(string value)
        {
            Value = value;
        }
        public string Value { get; private set; }
    }

    class Calculator
    {
        private List<string> operators = new List<string>(new string[] { Operators.leftBracket.Value, Operators.rightBracket.Value, Operators.plus.Value, Operators.minus.Value, Operators.multiply.Value, Operators.division.Value, Operators.ex.Value });
        private List<string> operatorsLetters = new List<string>(new string[] { Operators.cos.Value, Operators.sin.Value, Operators.tan.Value, Operators.sqrt.Value });

        private List<string> Separate(string inputString)
        {
            List<string> separateStringList = new List<string>();
            int position = 0;
            bool flagUnaryMinus = false;

            inputString = inputString.Replace(" ", String.Empty);
            inputString = inputString.Replace(".", ",");
            while (position < inputString.Length)
            {
                string tempString = inputString[position].ToString();
                if ((String.Equals(inputString[position].ToString(), Operators.minus.Value) && separateStringList.Count == 0) || (String.Equals(inputString[position].ToString(), Operators.minus.Value) && String.Equals(inputString[position - 1].ToString(), Operators.leftBracket.Value)))
                {//check unary minus                      
                    position++;
                    tempString += inputString[position].ToString();
                    flagUnaryMinus = true;
                }
                if (!operators.Contains(inputString[position].ToString()))
                {
                    if (Char.IsDigit(inputString[position]))
                        for (int i = position + 1; i < inputString.Length && (Char.IsDigit(inputString[i]) || String.Equals(inputString[i].ToString(), Operators.comma.Value)); i++) 
                                tempString += inputString[i];
                    else if (char.IsLetter(inputString[position]))
                        for (int i = position + 1; i < inputString.Length && Char.IsLetter(inputString[i]); i++)
                            tempString += inputString[i];
                }
                separateStringList.Add(tempString);
                if (flagUnaryMinus)
                {
                    position += tempString.Length - 1;
                    flagUnaryMinus = false;
                }
                else
                    position += tempString.Length;
            }
            return separateStringList;
        }

        private byte Priority(string s)
        {
            switch (s)
            {
                case "(":
                    return 0;
                case ")":
                    return 0;
                case "+":
                    return 1;
                case "-":
                    return 1;
                case "*":
                    return 2;
                case "/":
                    return 2;
                case "^":
                    return 3;
                default:
                    return 4;
            }
        }

        private List<string> ConvertToPostfixNotation(string inputString)
        {
            List<string> postfixNotationList = new List<string>();
            Stack<string> stack = new Stack<string>();
            List<string> separateInputStringList = new List<string>(Separate(inputString));

            for (int i = 0; i < separateInputStringList.Count; i++)
            {
                if (operators.Contains(separateInputStringList[i]))
                {
                    if (stack.Count > 0 && !separateInputStringList[i].Equals(Operators.leftBracket.Value))
                    {
                        if (separateInputStringList[i].Equals(Operators.rightBracket.Value))
                        {
                            string temp = stack.Pop();
                            while (temp != Operators.leftBracket.Value)
                            {
                                postfixNotationList.Add(temp);
                                temp = stack.Pop();
                            }
                        }
                        else if (Priority(separateInputStringList[i]) > Priority(stack.Peek()))
                        {
                            stack.Push(separateInputStringList[i]);
                        }
                        else
                        {
                            while (stack.Count > 0 && Priority(separateInputStringList[i]) <= Priority(stack.Peek()))
                            {
                                if (separateInputStringList[i].Equals(Operators.ex.Value) && stack.Peek() == Operators.ex.Value)
                                    break;
                                else
                                    postfixNotationList.Add(stack.Pop());
                            }
                            stack.Push(separateInputStringList[i]);
                        }
                    }
                    else
                        stack.Push(separateInputStringList[i]);
                }
                else if (operatorsLetters.Contains(separateInputStringList[i]))
                {                    
                    Stack<string> bracketStack = new Stack<string>();
                    string tempStringInBrackets = String.Empty;
                    string tempOperatorLetters = separateInputStringList[i].ToString();
                    i++;                    
                    while (i < separateInputStringList.Count)//Generates a string in brackets
                    {                   
                        if (separateInputStringList[i].Equals(Operators.leftBracket.Value))
                        {
                            bracketStack.Push(separateInputStringList[i]);
                            tempStringInBrackets += separateInputStringList[i].ToString();
                            i++;
                        }
                        else if (separateInputStringList[i].Equals(Operators.rightBracket.Value))
                        {
                            bracketStack.Pop();
                            tempStringInBrackets += separateInputStringList[i].ToString();                            
                            if (bracketStack.Count == 0)
                                break;
                            i++;
                        }
                        else
                        {
                            tempStringInBrackets += separateInputStringList[i].ToString();
                            i++;
                        }
                    }
                    //recursive convert to postfix notation
                    foreach (string tS in ConvertToPostfixNotation(tempStringInBrackets))
                        postfixNotationList.Add(tS);
                    postfixNotationList.Add(tempOperatorLetters);
                }
                else
                    postfixNotationList.Add(separateInputStringList[i]);
            }
            if (stack.Count > 0)
                foreach (string s in stack)
                    postfixNotationList.Add(s);
            return postfixNotationList;
        }

        public string Calculate(string inputString)
        {
            double result;
            Stack<double> stack = new Stack<double>();
            foreach (string s in ConvertToPostfixNotation(inputString))
            {
                if (operators.Contains(s)  || operatorsLetters.Contains(s))
                {
                    switch (s)
                    {
                        case "+":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                double b = Convert.ToDouble(stack.Pop());
                                result = a + b;
                                stack.Push(result);
                                break;
                            }
                        case "-":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                double b = Convert.ToDouble(stack.Pop());
                                result = b - a;
                                stack.Push(result);
                                break;
                            }
                        case "*":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                double b = Convert.ToDouble(stack.Pop());
                                result = a * b;
                                stack.Push(result);
                                break;
                            }
                        case "/":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                double b = Convert.ToDouble(stack.Pop());
                                if (a == 0)
                                    return "Divided by zero";
                                result = b / a;
                                stack.Push(result);
                                break;
                            }
                        case "^":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                double b = Convert.ToDouble(stack.Pop());
                                result = Math.Pow(b, a);
                                stack.Push(result);
                                break;
                            }
                        case "cos":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                result = Math.Cos(a);
                                stack.Push(result);
                                break;
                            }
                        case "sin":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                result = Math.Sin(a);
                                stack.Push(result);
                                break;
                            }
                        case "tan":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                result = Math.Tan(a);
                                stack.Push(result);
                                break;
                            }
                        case "sqrt":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                result = Math.Sqrt(a);
                                stack.Push(result);
                                break;
                            }
                    }
                }
                else
                    stack.Push(Convert.ToDouble(s));
            }
            return Convert.ToString(Math.Round(stack.Pop(),5));
        }
    }
}
