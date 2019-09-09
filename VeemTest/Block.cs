using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeemTest
{
    public class Block
    {
        /// <summary>
        /// Id
        /// </summary>
        public int ID { get;  set; }

        /// <summary>
        /// Original buffer
        /// </summary>
        public byte[] Buffer { get;  set; }


        public override string ToString() => $"ID = {ID}; length = {Buffer.Length}";


    }
}
