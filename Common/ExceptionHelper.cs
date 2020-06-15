using PointsMall.Common.Exceptions;
using System;
using System.IO;

namespace PointsMall.Common
{
    /// <summary>
    /// 项目中常用的Exception列表
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// 数据库,配置,Redis,接口返回等找不到想要的数据
        ///  DataNotFoundException HttpStatusCode = 404(未找到)
        /// </summary>
        /// <param name="message"></param>
        public static DataNotFoundException DataNotFoundException(string message)
        {
            return new DataNotFoundException(message);
        }

        /// <summary>
        /// 数据库中的数据不合法,脏数据
        /// InvalidDataException  409;(数据冲突)
        /// </summary>
        /// <param name="message"></param>
        public static InvalidDataException InvalidDataException(string message)
        {
            return new InvalidDataException(message);
        }

        /// <summary>
        /// InvalidArgumentException  400//错误请求
        /// 输入的参数不合法或为空
        /// </summary>
        /// <param name="message"></param>
        public static ArgumentException InvalidArgumentException(string message)
        {
            return new ArgumentException(message);
        }


        /// <summary>
        /// 没有权限下的操作,资源过期如合同过期后继续添加体检订单等, 
        /// 订单未支付情况,要预约体检等..  405; (资源禁用)
        /// </summary>
        /// <param name="message"></param>
        public static InvalidOperationException InvalidOperationException(string message)
        {
            return new InvalidOperationException(message);
        }

        /// <summary>
        /// 不支持此操作
        /// </summary>
        /// <param name="message"></param>
        public static Exception NotSupportedException(string message)
        {
            return new NotSupportedException(message);
        }

        /// <summary>
        /// 其他情况,直接用Exception 500;// (服务器内部错误)
        /// </summary>
        /// <param name="message"></param>
        public static Exception UnknownException(string message)
        {
            return new Exception(message);
        }
    }
}
