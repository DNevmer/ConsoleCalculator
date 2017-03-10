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
            CalculateFile("input_3.txt", "output_3.txt");
        }

        //read file, calculate and write result in file
        public static void CalculateFile(string fileInputName, string fileOutputName)
        {
            Calculator calc = new Calculator();
                       
            System.IO.StreamReader file = new System.IO.StreamReader(fileInputName);
            System.IO.File.Delete(fileOutputName);
            string line; 
            while ((line = file.ReadLine()) != null)
                System.IO.File.AppendAllText(fileOutputName, line + " = " + calc.Calculate(line) + Environment.NewLine);
        }
    }
}
