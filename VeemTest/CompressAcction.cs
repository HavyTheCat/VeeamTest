using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VeemTest.Constant;

namespace VeemTest
{
    public sealed class CompressAction : BaseGzipAction, IArchivActionable
    {
        public CompressAction(string input, string output)
            : base(input, output) { }


        protected override void AppendAllBytes(string path, byte[] bytes)
        {
            using (var stream = new FileStream(path, FileMode.Append))
            {
                BitConverter.GetBytes(bytes.Length).CopyTo(bytes, 4);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        protected override string GetExtention()
        {
            return CommonApplicationConstants.GZIP_EXTENTION;
        }

        protected override byte[] GetGzipArray(Block block)
        {
            byte[] res = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream cs = new GZipStream(memoryStream, GetGzipReadingType()))
                    cs.Write(block.Buffer, 0, block.Buffer.Length);

                res = memoryStream.ToArray();
            }
            return res;
        }

        protected override CompressionMode GetGzipReadingType()
        {
            return CompressionMode.Compress;
        }

        protected override int GetReadedlength(long fileLengh, long pos)
        {
            int res;
            //Calculating slice size
            if (fileLengh - pos <= CommonApplicationConstants.BLOCK_SIZE)
                res = (int)(fileLengh - pos);

            else
                res = CommonApplicationConstants.BLOCK_SIZE;

            return res;
        }

        /// <summary>
        /// open input file and get byte array
        /// </summary>
        /// <param name="sourceFile">path to input file</param>
        /// <param name="pos">curent position in byte stream</param>
        /// <param name="bytesRead">size of slice array</param>
        /// <returns>byte arr of input file by giving size</returns>
        protected override byte[] GetSlice(string sourceFile, long pos, int bytesRead)
        {
            var res = new byte[bytesRead];
            using (FileStream _fileToBeCompressed = new FileStream(sourceFile, FileMode.Open))
            {
                _fileToBeCompressed.Position = pos;
                _fileToBeCompressed.Read(res, 0, bytesRead);
            }
            return res;
        }

     
    }
}
