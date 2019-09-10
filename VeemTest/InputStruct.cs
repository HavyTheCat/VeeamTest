using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeemTest.Constant;

namespace VeemTest
{
    public class InputStruct
    {
        /// <summary>
        /// Compress/Decompress actions
        /// </summary>
        public ActionType Action { set; get; }

        /// <summary>
        /// Input file
        /// </summary>
        public FileInfo InputFileInfo { set; get; }

        /// <summary>
        /// Output file
        /// </summary>
        public FileInfo OutPutFileInfo { set; get; }
    }
}
