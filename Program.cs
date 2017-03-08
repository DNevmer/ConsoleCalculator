using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCalc
{
    class Program
    {
        static void Main(string[] args)
        {     
            CalculateFile("input_1.txt", "output_1.txt");
            CalculateFile("input_2.txt", "output_2.txt");
        }

        //считывание и запись в файл
        public static void CalculateFile(string fileInputName, string fileOutputName)
        {
            Calculator calc = new Calculator();
            string line;            
            System.IO.StreamReader file = new System.IO.StreamReader(fileInputName);//считывание из файла
            System.IO.File.Delete(fileOutputName);
            while ((line = file.ReadLine()) != null)
            {
                System.IO.File.AppendAllText(fileOutputName, line + " = " + calc.Calculate(line) + Environment.NewLine);//вычисление и запись в файл
            }
        }
    }
}
