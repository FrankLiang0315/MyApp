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
    private readonly IConfiguration _configuration;

    // private readonly HttpClient _httpClient;
    public TestController(IConfiguration configuration)
    {
        _configuration = configuration;
        // _httpClient = httpClient;
    }


    [HttpGet]
    [Route("api/[controller]")]
    public async Task<ActionResult<string>> TestGet()
    {
        var fields = new Dictionary<string, string>();


        fields["MerchantID"] = "2000933";
        fields["MerchantTradeDate"] = "2023/11/03 11:40:34";
        fields["LogisticsType"] = "HOME";
        fields["MerchantTradeNo"] = "12312sdaswww" + new Random().Next(10000);
        fields["LogisticsSubType"] = "TCAT";
        fields["GoodsAmount"] = "5000";
        fields["GoodsName"] = "物品1號";
        fields["SenderName"] = "Frank";
        fields["SenderPhone"] = "0477777";
        fields["SenderZipCode"] = "505";
        fields["SenderAddress"] = "彰化縣鹿港鎮彰鹿路8段12號";
        fields["ReceiverName"] = "梁山伯";
        fields["ReceiverCellPhone"] = "0931323222";
        fields["ReceiverZipCode"] = "600";
        fields["ReceiverAddress"] = "台中市西屯區市政路500號";
        fields["ClientReplyURL"] = "http://localhost";
        fields["ServerReplyURL"] = "http://localhost";
        fields["CheckMacValue"] = PrepareCheckValue(fields);


        string htmlContent = Payment.PrepareHtmlString("https://logistics-stage.ecpay.com.tw/Express/Create",fields);
        
        // return Ok(fields["CheckMacValue"]);
        return new ContentResult
        {
            Content = htmlContent,
            ContentType = "text/html",
            StatusCode = 200
        };
    }

    public class CVS
    {
        public string CVSStoreID { get; set; }
    }

    [HttpPost]
    [Route("api/[controller]")]
    public async Task<ActionResult<string>> TestPost([FromForm] CVS data)
    {
        return Redirect($"{_configuration["URL:FrontEnd"]}?id={data.CVSStoreID}");
    }


    private string PrepareCheckValue(Dictionary<string, string> fields)
    {
        string HashKey = "XBERn1YOvpM9nfZc";
        string HashIV = "h1ONHk4P4yqbl5LK";

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
        checkValue = Payment.ComputeMD5(checkValue).ToUpper();

        return checkValue;
    }
}