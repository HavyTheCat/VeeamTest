using System;

namespace VeemTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IArchivActionable action = new DecompressAction(@"C:\test\test13.mkv.gz", @"C:\test\test13.mkv");
            action.Start();
        }
    }
}
