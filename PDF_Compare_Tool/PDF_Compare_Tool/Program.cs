using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Compare_Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            string file1 = String.Empty;
            string file2 = String.Empty;
            
            Console.WriteLine("Enter first PDF file name");
            file1 =Convert.ToString( Console.ReadLine());
            // Ask the user to type the second number.  
            Console.WriteLine("Enter second PDF file name");
            file2 =Convert.ToString( Console.ReadLine());
           //setup
            Console.WriteLine("First file: "+file2+" \nSecond file: "+file2+"");
            Console.Write("\nPress any key to close the console app...");

            Console.ReadKey();
        }
    }
}
