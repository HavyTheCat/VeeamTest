using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeemTest.Constant;

namespace VeemTest
{
    public class InputService : IInputService
    {
        public InputStruct GetInput(string[] args)
        {
            InputStruct res = new InputStruct();
            res.Action = GetAction(args);
            res.InputFileInfo = new FileInfo(args[1]);
            res.OutPutFileInfo = new FileInfo(args[2]);

            return res;
        }
       
        public bool ValidateArgs(string[] args, out string errMsg)
        {
            errMsg = string.Empty;
            FileInfo inputFileInfo;
            FileInfo outPutFileInfo;

            try
            {
                if (args.Length != 3)
                    errMsg = "There should be three parameters. compress/decompress [Source file] [Destination file]";

                if (string.IsNullOrEmpty(errMsg) &&
                    args[0].ToLower() != CommonApplicationConstants.COMPRESS &&
                    args[0].ToLower() != CommonApplicationConstants.DECOMPRESS)
                    errMsg = $"First argument shall be { CommonApplicationConstants.COMPRESS} or { CommonApplicationConstants.DECOMPRESS }.";

                inputFileInfo = new FileInfo(args[1]);
                if (args[0].ToLower() == CommonApplicationConstants.COMPRESS)
                    outPutFileInfo = new FileInfo(args[2] + CommonApplicationConstants.GZIP_EXTENTION);
                else
                    outPutFileInfo = new FileInfo(args[2]);


                if (string.IsNullOrEmpty(errMsg) && !inputFileInfo.Exists)
                    errMsg = $"No such file or directory {inputFileInfo.FullName}";

                if (string.IsNullOrEmpty(errMsg) && inputFileInfo.FullName == outPutFileInfo.FullName)
                    errMsg = "The input and output file must have different paths.";

                if (string.IsNullOrEmpty(errMsg) && args[1].Length == 0)
                    errMsg = "No input file specified.";

                if (string.IsNullOrEmpty(errMsg) && args[2].Length == 0)
                    errMsg = "No output file specified.";

                if (string.IsNullOrEmpty(errMsg) && !inputFileInfo.Exists)
                    errMsg = $"Input file {inputFileInfo.FullName} not exist";

                if (string.IsNullOrEmpty(errMsg) &&
                    inputFileInfo.Extension == CommonApplicationConstants.GZIP_EXTENTION &&
                    args[0] == CommonApplicationConstants.COMPRESS)
                    errMsg = $"Input file {inputFileInfo.FullName} already compressed";

                if (string.IsNullOrEmpty(errMsg) && 
                    File.Exists(outPutFileInfo.FullName + CommonApplicationConstants.GZIP_EXTENTION) &&
                    args[0] == CommonApplicationConstants.COMPRESS)
                    errMsg = $"Output file {outPutFileInfo.FullName} already exist.";

                if (string.IsNullOrEmpty(errMsg) &&
                    outPutFileInfo.Exists)
                    errMsg = $"Output file {outPutFileInfo.FullName} already exist.";

                if(string.IsNullOrEmpty(errMsg) && 
                    args[0] == CommonApplicationConstants.DECOMPRESS &&
                    inputFileInfo.Extension != CommonApplicationConstants.GZIP_EXTENTION)
                    errMsg = $"Input file {inputFileInfo.FullName} shuld have \"{CommonApplicationConstants.GZIP_EXTENTION}\" extention.";

                return string.IsNullOrEmpty(errMsg);
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        private ActionType GetAction(string[] args)
        {
            if (args[0].ToLower() == CommonApplicationConstants.COMPRESS)
                return ActionType.Compress;
            else
                return ActionType.Decompress;
        }
    }
}
