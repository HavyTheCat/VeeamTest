using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeemTest.Constant
{
    public static class CommonApplicationConstants
    {
        /// <summary>
        /// Const for for compressed file extantion
        /// </summary>
        public const string GZIP_EXTENTION = ".gz";

        /// <summary>
        /// Const for compress actions
        /// </summary>
        public const string COMPRESS = "compress";

        /// <summary>
        /// Const for decompress actions
        /// </summary>
        public const string DECOMPRESS = "decompress";

        /// <summary>
        /// Const for blocksize
        /// </summary>
        public const int BLOCK_SIZE = 10000000;

    }

    public enum ActionType
    {
        Compress,
        Decompress
    }
}
