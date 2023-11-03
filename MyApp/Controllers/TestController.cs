using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyApp.Tools;

namespace MyApp.Controllers;

[ApiController]
public class TestController : InfoController
{
    [HttpGet]
    [Route("api/[controller]")]
    public async Task<ActionResult<string>> TestGet()
    {
        var fields = new Dictionary<string, string>();


        fields["MerchantID"] = "3002607";
        fields["MerchantTradeNo"] = "A12345akfjlnj277";
        fields["MerchantTradeDate"] = "2023/10/25 14:46:22";
        fields["PaymentType"] = "aio";
        fields["TotalAmount"] = "500";
        fields["TradeDesc"] = "測試說明";
        fields["ItemName"] = "測試商品1 * 2 ＝ 200#測試商品2 * 2 ＝ 300";
        fields["ReturnURL"] = "https://localhost:7009/";
        fields["ChoosePayment"] = "Credit";
        fields["EncryptType"] = "1";
        
        fields["CheckMacValue"] = PrepareCheckValue(fields);


        string htmlContent = Payment.PrepareHtmlString(fields);

        // return Ok(fields["CheckMacValue"]);
        return new ContentResult
        {
            Content = htmlContent,
            ContentType = "text/html",
            StatusCode = 200
        };
    }

    [HttpPost]
    [Route("api/[controller]")]
    public async Task<ActionResult<string>> TestPost()
    {
        return Redirect("https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5");
    }
    

    private string PrepareCheckValue(Dictionary<string, string> fields)
    {
        string HashKey = "pwFHCqoQZGmho4w6";
        string HashIV = "EkRm7iFT261dpevs";
        
        var sortedFields = fields.OrderBy((field) => field.Key).ToDictionary(x => x.Key, x => x.Value);
        var checkValueList = new List<string>();
        string checkValue = "";
        foreach (var keyValuePair in sortedFields)
        {
            checkValueList.Add($"{keyValuePair.Key}={keyValuePair.Value}");
        }
        checkValue = string.Join("&", checkValueList);
        checkValue = $"HashKey={HashKey}&" + checkValue + $"&HashIV={HashIV}";
        checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
        checkValue = Payment.ComputeSha256Hash(checkValue).ToUpper();

        return checkValue;
    }
    
}