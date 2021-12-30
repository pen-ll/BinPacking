using System;

namespace BinPacking
{
    /// <summary>
    /// 无法适配异常
    /// 【用于跳出最外层，中止当前适配】
    /// </summary>
    public class CannotFitException : Exception
    {
        public CannotFitException()
        {
        }

        public CannotFitException(string message)
            : base(message)
        {
        }
    }
}
