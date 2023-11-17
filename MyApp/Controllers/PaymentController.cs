using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Model;
using MyApp.Model.Order;
using MyApp.Model.OrderItem;
using MyApp.Model.Pet;
using MyApp.Tools;
using NuGet.Protocol;
using Serilog;

namespace MyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : InfoController
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly string _url = "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5";
    private readonly string _payService = "ECPAY";
    private readonly string _merchantId = "3002607";
    private readonly string _HashKey = "pwFHCqoQZGmho4w6";
    private readonly string _HashIV = "EkRm7iFT261dpevs";
    public PaymentController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }


    [HttpGet]
    [Route("{orderNumber}")]
    public IActionResult PayUrl(string orderNumber)
    {
        var order = _context.Orders.Include(o => o.OrderItems)
            .FirstOrDefault((order) =>
                order.OrderNumber == orderNumber && (order.Status == Order.OrderStatus.PendingPayment));
        if (order == null)
        {
            return NotFound();
        }

        Dictionary<string, string> fields = PrepareFields(order);

        if (fields.Count == 0)
        {
            return Ok("no item");
        }

        string url = _url;

        string htmlContent = Payment.PrepareHtmlString(url,fields);

        order.PaymentCount += 1;
        _context.SaveChangesAsync();

        return new ContentResult
        {
            Content = htmlContent,
            ContentType = "text/html",
            StatusCode = 200
        };
    }

    public class EcpayNotifyRequest
    {
        public string? MerchantID { get; set; } = "";
        public string? MerchantTradeNo { get; set; } = "";
        public string? StoreID { get; set; } = "";
        public int RtnCode { get; set; }
        public string? RtnMsg { get; set; } = "";
        public string? TradeNo { get; set; } = "";
        public int TradeAmt { get; set; }
        public string? PaymentDate { get; set; } = "";
        public string? PaymentType { get; set; } = "";
        public int PaymentTypeChargeFee { get; set; }
        public string? TradeDate { get; set; } = "";
        public int SimulatePaid { get; set; }
        public string? CheckMacValue { get; set; } = "";
        public string? CustomField1 { get; set; } = "";
        public string? CustomField2 { get; set; } = "";
        public string? CustomField3 { get; set; } = "";
        public string? CustomField4 { get; set; } = "";

    }

    [HttpPost]
    [Route("notify/{payService}/{orderNumber}")]
    public async Task<IActionResult> Notify(string payService, string orderNumber, [FromForm] EcpayNotifyRequest request)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        if (order == null || order.Status!=Order.OrderStatus.PendingPayment) return NotFound();
        
        MyLog.Info("payment_notify", request.ToJson());
        var fields = new Dictionary<string, string>();
        
        fields["MerchantID"] = request.MerchantID;
        fields["MerchantTradeNo"] = request.MerchantTradeNo;
        fields["StoreID"] = request.StoreID;
        fields["RtnCode"] = request.RtnCode.ToString();
        fields["RtnMsg"] = request.RtnMsg;
        fields["TradeNo"] = request.TradeNo;
        fields["TradeAmt"] = request.TradeAmt.ToString();
        fields["PaymentDate"] = request.PaymentDate;
        fields["PaymentType"] = request.PaymentType;
        fields["PaymentTypeChargeFee"] = request.PaymentTypeChargeFee.ToString();
        fields["TradeDate"] = request.TradeDate;
        fields["SimulatePaid"] = request.SimulatePaid.ToString();
        fields["CustomField1"] = request.CustomField1;
        fields["CustomField2"] = request.CustomField2;
        fields["CustomField3"] = request.CustomField3;
        fields["CustomField4"] = request.CustomField4;
        MyLog.Info("payment_notify", $"CheckValue:{PrepareCheckValue(fields)}");

        if (PrepareCheckValue(fields) != request.CheckMacValue) return NotFound();

        if (request.RtnCode == 1)
        {
            order.Status = Order.OrderStatus.PendingLogistics;
            await _context.SaveChangesAsync();
        }

        return Ok("1|OK");
    }

    private Dictionary<string, string> PrepareFields(Order order)
    {
        

        var ItemList = new List<string>();
        var fields = new Dictionary<string, string>();

        if (order.OrderItems == null)
        {
            return fields;
        }

        foreach (var orderItem in order.OrderItems)
        {
            // todo: 其他商品需新增
            string productName = orderItem.Product switch
            {
                OrderItem.ProductType.MainFood => "狗糧",
                _ => "狗糧"
            };
            ItemList.Add($"{productName} ＊ {orderItem.Months} ＝ {orderItem.Price}");
        }

        int paymentCount = order.PaymentCount + 1;

        fields["MerchantID"] = _merchantId;
        fields["MerchantTradeNo"] = order.OrderNumber + "tp" + paymentCount;
        fields["MerchantTradeDate"] = order.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss");
        fields["PaymentType"] = "aio";
        fields["TotalAmount"] = order.Price.ToString();
        fields["TradeDesc"] = $"{order.OrderNumber} 第{paymentCount}次嘗試付款";
        fields["ItemName"] = string.Join("#", ItemList);
        fields["ReturnURL"] = $"{_configuration["URL:BackEnd"]}/api/Payment/notify/{_payService}/{order.OrderNumber}";
        // fields["ChoosePayment"] = "Credit";
        fields["ChoosePayment"] = "ALL";
        fields["EncryptType"] = "1";
        fields["ClientBackURL"] = $"{_configuration["URL:FrontEnd"]}/order";

        fields["CheckMacValue"] = PrepareCheckValue(fields);
        return fields;
    }


    private string PrepareCheckValue(Dictionary<string, string> fields)
    {
        

        var sortedFields = fields.OrderBy((field) => field.Key).ToDictionary(x => x.Key, x => x.Value);
        var checkValueList = new List<string>();
        string checkValue = "";
        foreach (var keyValuePair in sortedFields)
        {
            checkValueList.Add($"{keyValuePair.Key}={keyValuePair.Value}");
        }

        checkValue = string.Join("&", checkValueList);
        Console.WriteLine(checkValue);
        checkValue = $"HashKey={_HashKey}&" + checkValue + $"&HashIV={_HashIV}";
        checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
        checkValue = Payment.ComputeSha256Hash(checkValue).ToUpper();

        return checkValue;
    }
}