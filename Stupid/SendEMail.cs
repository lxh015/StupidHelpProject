using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Stupid
{
    /// <summary>
    /// 邮件帮助类
    /// </summary>
   public class SendEMail
    {
        /// <summary>
        /// 发送人
        /// </summary>
        public string From;

        /// <summary>
        /// 发送人密码
        /// </summary>
        public string PassWord;

        /// <summary>
        /// 收件人
        /// </summary>
        public string[] To;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title;

        /// <summary>
        /// 内容
        /// </summary>
        public string Body;

        /// <summary>
        /// SMTP事物主机
        /// </summary>
        public string MailHost;

        /// <summary>
        /// 主机端口
        /// </summary>
        public string Port;

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        public bool Send(bool isHtml)
        {
            MailAddress Efrom = new MailAddress(From);

            MailMessage mail = new MailMessage();

            //防屏蔽功能
            #region

            mail.Headers.Add("X-Mailer", "Tom");
            mail.Headers.Add("X-Priority", "3");
            mail.Headers.Add("X-MSMail-Priority", "Normal");
            mail.Headers.Add("X-MimeOLE", "Produced By Microsoft MimeOLE V6.00.2900.2869");
            mail.Headers.Add("ReturnReceipt", "1");

            #endregion

            mail.Subject = Title;
            mail.From = Efrom;

            foreach (var item in To)
            {
                mail.To.Add(new MailAddress(item));
            }

            mail.Body = Body;

            //设置邮件编码
            mail.BodyEncoding = System.Text.Encoding.UTF8;

            //是否用HTML格式显示
            mail.IsBodyHtml = isHtml;

            //邮件的优先级(高|正常|低)
            mail.Priority = MailPriority.High;
            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            SmtpClient client = new SmtpClient();
            client.Host = MailHost;

            //设置端口
            if (string.IsNullOrEmpty(Port)) { client.Port = 25; }
            else { client.Port = Convert.ToInt32(Port); }

            client.Credentials = new System.Net.NetworkCredential(From, PassWord);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;

            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session.Add("MailError", ex.Message);
                return false;
            }

        }
    }
}
