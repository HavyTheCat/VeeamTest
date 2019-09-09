using System;

namespace VeemTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IArchivActionable action = new CompressAcction(@"C:\Users\Igor\Downloads\Dave.Chappelle.Sticks.and.Stones.2019.1080p.NF.WEB-DL.DDP5.1.Atmos.x264-NTG[TGx]\Dave.Chappelle.Sticks.and.Stones.2019.1080p.NF.WEB-DL.DDP5.1.x264-NTG.mkv", @"C:\test\test12.mkv");
            action.Start();
        }
    }
}
