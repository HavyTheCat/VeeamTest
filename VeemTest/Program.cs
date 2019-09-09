using System;

namespace VeemTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IArchivActionable action = new DecompressAction(@"C:\test\test12.mkv.gz", @"C:\test\test12.mkv");
            action.Start();
        }
    }
}
