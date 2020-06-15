using System;

namespace PointsMall.Common.Exceptions
{
    public class DataNotFoundException : Exception
    {
        public DataNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// 对应HttpStatusCode 
        /// </summary>
        public int Code { get; } = 404; //未找到资源
    }
}
