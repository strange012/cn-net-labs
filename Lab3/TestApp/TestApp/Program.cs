using System;
using System.Windows;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("***** Basic Console I/O *****");
            DisplayMessage();
            Console.ReadLine();

        }
        
        
        static void DisplayMessage()
        {
            // Использование string.Format() для форматирования строкового литерала,
            string userMessage = string.Format("100000 in hex is {0:x}", 100000);
            // Для успешной компиляции этой строки кода требуется
            // ссылка на библиотеку PresentationFramework.dll!
            System.Windows.MessageBox.Show(userMessage);

        }
    }
}
