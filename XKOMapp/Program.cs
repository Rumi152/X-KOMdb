using Spectre.Console;

namespace XKOMapp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new ProductView().PrintView();

            Console.ReadKey();
        }
    }
}