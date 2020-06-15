using MimeKit;
using PointsMall.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SecretGarden.Bos
{
    public class BoBase
    {
        public BoProvider _boProvider { get; set; }
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public void SaveChanges()
        {
            _boProvider._context.SaveChanges();
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailFrom"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailTitile"></param>
        /// <param name="mailHtmlBody"></param>
        /// <returns></returns>
        public static async Task<bool> SendEmail(MailboxAddress mailFrom,MailboxAddress mailTo,string mailTitle, string mailHtmlBody)
        {
            return await SendEmailWithAttachmentAsync(mailFrom, mailTo, mailTitle, mailHtmlBody, null, null);
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
        public static async Task<bool> SendEmailWithAttachmentAsync(MailboxAddress mailFrom, MailboxAddress mailTo, string mailTitle, string mailHtmlBody, string fileName, byte[] fileContent)
        {
            _logger.Debug($"开始发送邮件 发送人:{mailFrom.Name} {mailFrom.Address} 接收人:{mailTo.Name} {mailTo.Address} title:{mailTitle} textBody:{mailHtmlBody} fileName:{fileName}");
            try
            {
                var message = new MimeMessage();
                message.From.Add(mailFrom);
                message.To.Add(mailTo);
                message.Subject = mailTitle;

                var builder = new BodyBuilder();

                builder.HtmlBody = mailHtmlBody;

                if (!string.IsNullOrEmpty(fileName) && fileContent != null && fileContent.Length > 0)
                {
                    var attachment = builder.Attachments.Add(fileName, fileContent);
                    //builder.Attachments.Add(@"C:\Users\Administrator\source\repos\NewStaffHealthCheck\NewStaffHealthCheck\bin\Debug\netcoreapp2.1\th.jpg");
                    foreach (var param in attachment.ContentDisposition.Parameters)
                        param.EncodingMethod = ParameterEncodingMethod.Rfc2047;

                }
                // Now we just need to set the message body and we're done
                message.Body = builder.ToMessageBody();
                MailClient mailClient = new MailClient();
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.CheckCertificateRevocation = false; // don't check the Ssl Certification
                    _logger.Debug($"连接的邮件服务器:{mailClient.Host}:{mailClient.Port} UseSsl:{mailClient.UseSsl} ");
                    client.Connect(mailClient.Host, mailClient.Port, mailClient.UseSsl);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(mailClient.MailFromAccount, mailClient.MailFromPassword);

                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
                _logger.Debug("邮件发送成功");
                return true;
            }
            catch (Exception e)
            {
                _logger.Debug("邮件发送失败 0717");
                _logger.Error("发送邮件异常" + e.ToString());
                return false;
            }
        }
    }
}
