using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCalc
{
    class Calculator
    {
        private List<string> operators = new List<string>(new string[] { "(", ")", "+", "-", "*", "/", "^" });

        //разделение строки по элементам 
        private List<string> Separate(string inputString)
        {
            List<string> separateStringList = new List<string>();
            int position = 0;
            bool flag = false;//флаг проверки унарного минуса
            inputString = inputString.Replace(" ", String.Empty);//удаляет пробелы из входящей строки
            while (position < inputString.Length)
            {                
                string tempS = inputString[position].ToString();
                if ((inputString[position] == '-' && separateStringList.Count == 0) || (inputString[position] == '-' && inputString[position - 1] == '('))
                {//проверка на унарный минус в начале выражения или после скобки                        
                    position++;
                    tempS += inputString[position].ToString();
                    flag = true;
                }
                if (!operators.Contains(inputString[position].ToString()))
                {
                    if (Char.IsDigit(inputString[position]))
                        for (int i = position + 1; i < inputString.Length && (Char.IsDigit(inputString[i]) || inputString[i] == ',' || inputString[i] == '.'); i++)
                        {
                            if (inputString[i] == '.')
                                tempS += ',';
                            else
                                tempS += inputString[i];
                        }
                }
                separateStringList.Add(tempS);
                if (flag)
                {
                    position += tempS.Length - 1;
                    flag = false;
                }
                else
                    position += tempS.Length;
            }
            return separateStringList;
        }

        private byte Priority(string s)//приоритеты
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

        //преобразование строки в обратную польскую запись 
        private List<string> ConvertToPostfixNotation(string inputString)
        {
            List<string> postfixNotationList = new List<string>();
            Stack<string> stack = new Stack<string>();
            foreach (string s in Separate(inputString))
            {
                if (operators.Contains(s))
                {
                    if (stack.Count > 0 && !s.Equals("("))
                    {
                        if (s.Equals(")"))
                        {
                            string temp = stack.Pop();
                            while (temp != "(")
                            {
                                postfixNotationList.Add(temp);
                                temp = stack.Pop();
                            }
                        }
                        else if (Priority(s) > Priority(stack.Peek()))
                            stack.Push(s);
                        else
                        {
                            while (stack.Count > 0 && Priority(s) <= Priority(stack.Peek()))
                            {
                                if (s.Equals("^") && stack.Peek() == "^")
                                { break; }
                                else
                                    postfixNotationList.Add(stack.Pop());
                            }
                            stack.Push(s);
                        }

                    }
                    else
                        stack.Push(s);
                }
                else
                    postfixNotationList.Add(s);
            }
            if (stack.Count > 0)
                foreach (string s in stack)
                    postfixNotationList.Add(s);

            return postfixNotationList;
        }

        //вычисление выражения
        public string Calculate(string inputString)
        {
            double rezult;
            //List<string> postfixNotationList = ConvertToPostfixNotation(inputString);
            Stack<double> stack = new Stack<double>();
            foreach (string s in ConvertToPostfixNotation(inputString))
            {
                if (operators.Contains(s))
                {

                    switch (s)
                    {
                        case "+":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                double b = Convert.ToDouble(stack.Pop());
                                rezult = a + b;
                                stack.Push(rezult);
                                break;
                            }
                        case "-":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                double b = Convert.ToDouble(stack.Pop());
                                rezult = b - a;
                                stack.Push(rezult);
                                break;
                            }
                        case "*":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                double b = Convert.ToDouble(stack.Pop());
                                rezult = a * b;
                                stack.Push(rezult);
                                break;
                            }
                        case "/":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                double b = Convert.ToDouble(stack.Pop());
                                if (a == 0)//проверка деления на нуль
                                    return "Divided by zero";
                                rezult = b / a;
                                stack.Push(rezult);
                                break;
                            }
                        case "^":
                            {
                                double a = Convert.ToDouble(stack.Pop());
                                double b = Convert.ToDouble(stack.Pop());
                                rezult = Math.Pow(b, a);
                                stack.Push(rezult);
                                break;
                            }
                    }
                }
                else
                {
                    stack.Push(Convert.ToDouble(s));
                }
            }
            return Convert.ToString(Math.Round(stack.Pop(),5));
        }
    }
}
