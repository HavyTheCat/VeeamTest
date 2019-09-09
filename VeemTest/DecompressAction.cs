using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeemTest
{
    public class DecompressAction : BaseGzipAction, IArchivActionable
    {
        public DecompressAction(string input, string output)
            : base(input, output) { }

        protected override string GetExtention()
        {
            return "";
        }

        protected override byte[] GetGzipArray(Block block)
        {
            byte[] res = null;
            using (MemoryStream memoryStream = new MemoryStream(block.Buffer))
            {
                int _dataSize = BitConverter.ToInt32(block.Buffer, block.Buffer.Length - 4);
                res = new byte[_dataSize];
                using (GZipStream cs = new GZipStream(memoryStream, GetGzipReadingType()))
                    cs.Read(res, 0, res.Length);
            }
            return res;
        }

        protected override CompressionMode GetGzipReadingType()
        {
            return CompressionMode.Decompress;
        }

        protected override int GetReadedlength(long fileLengh, long pos)
        {
            int res;
            using (FileStream _compressedFile = new FileStream(sourceFile, FileMode.Open))
            {
                _compressedFile.Position = pos;
                byte[] lengthBuffer = new byte[8];
                _compressedFile.Read(lengthBuffer, 0, lengthBuffer.Length);
                res = BitConverter.ToInt32(lengthBuffer, 4);
            }
            return res;
        }

        protected override byte[] GetSlice(string sourceFile, long pos, int bytesRead)
        {
            var res = new byte[bytesRead];
            using (FileStream compressedFile = new FileStream(sourceFile, FileMode.Open))
            {
                compressedFile.Position = pos;
                byte[] lengthBuffer = new byte[8];
                compressedFile.Read(lengthBuffer, 0, lengthBuffer.Length);
                byte[] compressedData = new byte[bytesRead];
                lengthBuffer.CopyTo(compressedData, 0);
                compressedFile.Read(compressedData, 8, bytesRead - 8);
                res = compressedData;
            }
            return res;
        }
    }
}
