using System.Security.Cryptography;
using System.Text;

namespace MyApp.Tools;

public static class Payment
{
    public static string ComputeSha256Hash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2")); // 将每个字节转换为两位十六进制数
            }

            return builder.ToString();
        }
    }
    
    public static string ComputeMD5(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            // 将字符串转换为字节数组
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // 计算 MD5 散列值
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // 将字节数组转换为十六进制字符串
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                stringBuilder.Append(hashBytes[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }

    public static string PrepareHtmlString(string url, Dictionary<string, string> fields)
    {
        string htmlString = "<html>" +
                            "<head>" +
                            "<meta charset='UTF-8'>" +
                            "<script>" +
                            "window.onload = function() {document.getElementById('ecForm').submit();}</script>" +
                            "</head>" +
                            "<body>" +
                            $"<form id='ecForm' action='{url}' method='post' hidden enctype='application/x-www-form-urlencoded'>";
        foreach (var keyValuePair in fields)
        {
            htmlString += $"<input name='{keyValuePair.Key}' value='{keyValuePair.Value}'></input>";
        }

        htmlString += "</form></body></html>";

        return htmlString;
    }
}