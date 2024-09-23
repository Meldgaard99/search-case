using System;

namespace ConsoleSearch
{
    class Program
    {
        static void Main(string[] args)
        {
           var app = new App();
           app.RunAsync().GetAwaiter().GetResult();
        }
    }
}
