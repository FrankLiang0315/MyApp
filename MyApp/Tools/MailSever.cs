using System.Net;
using System.Net.Mail;

namespace MyApp.Tools;

public class MailSever
{
    private readonly IConfiguration _configuration;

    public MailSever(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void SendMail(string emails, string title, string content, string paths="")
    {
        // 設置SMTP伺服器和端口
        string smtpServer = _configuration["SMTP:Sever"];
        int smtpPort = int.Parse(_configuration["SMTP:Port"]);

        // 設置寄件者的郵件地址和密碼
        string senderUserName = _configuration["SMTP:UserName"];
        string senderEmail = _configuration["SMTP:Email"];
        string senderPassword = _configuration["SMTP:Password"];
        
        var mail = new MailMessage();

        // 收件人 Email 地址
        foreach (var email in emails.Split(','))
        {
            mail.To.Add(email);
        }
        // 主旨
        mail.Subject = title;
        // 內文
        mail.Body = content;
        // 內文是否為 HTML
        mail.IsBodyHtml = true;
        // 優先權
        mail.Priority = MailPriority.Normal;

        // 發信來源,最好與你發送信箱相同,否則容易被其他的信箱判定為垃圾郵件.
        mail.From = new MailAddress(senderEmail, senderUserName);

        // // 附加檔案,如果沒有附加檔案不用這一趴
        // foreach (var path in paths.Split(','))
        // {
        //     Attachment attachment = new Attachment(path);
        //     mail.Attachments.Add(attachment);
        // }

        // Gmail 的 SMTP 設定
        var smtp = new SmtpClient(smtpServer, smtpPort)
        {
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            EnableSsl = true
        };

        // 投遞出去
        smtp.Send(mail);

        mail.Dispose();
    }
}