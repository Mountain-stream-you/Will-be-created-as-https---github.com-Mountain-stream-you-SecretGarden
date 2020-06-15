using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointsMall.Common
{
    public  class EmailHelper
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    }
        /// <summary>
        /// 发送邮件, 带附件
        /// </summary>
        /// <param name="mailFrom"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailTitle"></param>
        /// <param name="mailHtmlBody"></param>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        /// <summary>
        /// 邮件发送客户端参数
        /// </summary>
        public class MailClient
        {
            /// <summary>
            /// Smtp 服务器地址
            /// </summary>
            public string Host { get; set; } = "smtp.qq.com";
            /// <summary>
            /// Smtp 服务器端口
            /// </summary>
            public int Port { get; set; } = 465;
            /// <summary>
            /// 是否使用SSL
            /// </summary>
            public bool UseSsl { get; set; } = true;
            /// <summary>
            /// 在Smtp服务器进行发邮件的账号
            /// </summary>
            public string MailFromAccount { get; set; } = "1073308628@qq.com";
            /// <summary>
            /// 在Smtp服务器进行发邮件账号的密码
            /// </summary>
            public string MailFromPassword { get; set; } = "qbttdbxqlflzbaid";
        }
    
}
