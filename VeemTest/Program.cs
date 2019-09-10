using Microsoft.Extensions.DependencyInjection;
using System;
using VeemTest.Constant;

namespace VeemTest
{
    class Program
    {
        static IArchiveActionable action;

        static void Main(string[] args)
        {
#if DEBUG
            args = new string[3];
            args[0] = CommonApplicationConstants.COMPRESS;
            args[1] = @"C:\test\test21.mkv";
            args[2] = @"C:\test\test21.mkv";
#endif

            string errMsg;

            var collection = new ServiceCollection();
            collection.AddScoped<IInputService, InputService>();
            var serviceProvider = collection.BuildServiceProvider();

            var inputServ = serviceProvider.GetService<IInputService>();

            if (inputServ.ValidateArgs(args, out errMsg))
            {
                InputStruct inputStruct = inputServ.GetInput(args);

                switch (inputStruct.Action)
                {
                    case Constant.ActionType.Compress:
                        action = new CompressAction(inputStruct.InputFileInfo.FullName, inputStruct.OutPutFileInfo.FullName);
                        break;
                    case Constant.ActionType.Decompress:
                        action = new DecompressAction(inputStruct.InputFileInfo.FullName, inputStruct.OutPutFileInfo.FullName);
                        break;
                }
                action.Start();

            }
            else
                Console.WriteLine(errMsg);
        }

        static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs _args)
        {
            if (_args.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                _args.Cancel = true;
                action.Stop();

            }
        }

    }
}