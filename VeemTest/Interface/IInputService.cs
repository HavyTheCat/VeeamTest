using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeemTest
{
    public interface IInputService
    {
        /// <summary>
        /// validate input param
        /// </summary>
        /// <param name="args"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool ValidateArgs(string[] args, out string errMsg);

        /// <summary>
        /// Get input struct from args
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        InputStruct GetInput(string[] args);

    }
}
