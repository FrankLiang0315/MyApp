using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Model.Order;
using MyApp.Model.OrderItem;
using MyApp.Tools;
using NuGet.Protocol;

namespace MyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogisticsController : InfoController
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly string _url = "https://logistics-stage.ecpay.com.tw/Express/Create";
    private readonly string _payService = "ECPAY";
    private readonly string _merchantId = "2000933";
    private readonly string _HashKey = "XBERn1YOvpM9nfZc";
    private readonly string _HashIV = "h1ONHk4P4yqbl5LK";

    public LogisticsController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet]
    [Route("{orderNumber}")]
    public IActionResult LogisticsUrl(string orderNumber)
    {
        var order = _context.Orders.Include(o => o.OrderItems)
            .FirstOrDefault((order) =>
                order.OrderNumber == orderNumber && (order.Status == Order.OrderStatus.PendingLogistics));
        if (order == null)
        {
            return NotFound();
        }

        Dictionary<string, string> fields = PrepareFields(order);

        if (fields.Count == 0)
        {
            return Ok("no item");
        }

        

        string htmlContent = Payment.PrepareHtmlString(_url, fields);

        // order.Status = Order.OrderStatus.PendingPayment;
        // _context.SaveChangesAsync();

        return new ContentResult
        {
            Content = htmlContent,
            ContentType = "text/html",
            StatusCode = 200
        };
    }

    public class LogisticsReplyRequest
    {
        public string? MerchantID { get; set; }
        public string? MerchantTradeNo { get; set; }
        public string? RtnCode { get; set; }
        public string? RtnMsg { get; set; }
        public string? AllPayLogisticsID { get; set; }
        public string? LogisticsType { get; set; }
        public string? LogisticsSubType { get; set; }
        public string? GoodsAmount { get; set; }
        public string? UpdateStatusDate { get; set; }
        public string? CVSPaymentNo { get; set; }
        public string? CVSValidationNo { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
        public string? ReceiverCellPhone { get; set; }
        public string? ReceiverEmail { get; set; }
        public string? ReceiverAddress { get; set; }
        public string? BookingNote { get; set; }
        public string? CheckMacValue { get; set; }
    }

    public class LogisticsNotifyRequest
    {
        public string? MerchantID { get; set; }
        public string? MerchantTradeNo { get; set; }
        public string? RtnCode { get; set; }
        public string? RtnMsg { get; set; }
        public string? AllPayLogisticsID { get; set; }
        public string? LogisticsType { get; set; }
        public string? LogisticsSubType { get; set; }
        public string? GoodsAmount { get; set; }
        public string? UpdateStatusDate { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverCellPhone { get; set; }
        public string? ReceiverAddress { get; set; }
        public string? BookingNote { get; set; }
        public string? CheckMacValue { get; set; }
    }


    [HttpPost]
    [Route("{orderNumber}")]
    public async Task<IActionResult> LogisticsReply(string orderNumber, [FromForm] LogisticsReplyRequest request)
    {
        string orderManagementUrl = $"{_configuration["URL:FrontEnd"]}/admin/order-management";
        var order = await _context.Orders.FirstOrDefaultAsync((o) => o.OrderNumber == orderNumber);
        if (order != null)
        {
            if (request.RtnCode == "300")
            {
                order.Status = Order.OrderStatus.Shipping;
                await _context.SaveChangesAsync();
                return Redirect(
                    $"{orderManagementUrl}?logisticsStatus=1&orderNumber={orderNumber}");
            }

            return Redirect(
                $"{orderManagementUrl}?logisticsStatus=0&orderNumber={orderNumber}&errorMessage={request?.RtnMsg}");
        }

        return Redirect(
            $"{orderManagementUrl}?logisticsStatus=0&orderNumber={orderNumber}&errorMessage=找不到此訂單");
    }

    [HttpPost]
    [Route("notify/{payService}/{orderNumber}")]
    public async Task<IActionResult> LogisticsNotify(string payService, string orderNumber,
        [FromForm] LogisticsNotifyRequest request)
    {
        MyLog.Info("logistics_notify", request.ToJson());

        return Ok(request.ToJson());
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

        fields["MerchantID"] = _merchantId;
        fields["MerchantTradeDate"] = order.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss");
        fields["LogisticsType"] = "HOME";
        fields["MerchantTradeNo"] = order.OrderNumber;
        fields["LogisticsSubType"] = "TCAT";
        fields["GoodsAmount"] = order.Price.ToString();
        fields["GoodsName"] = "狗糧";
        fields["SenderName"] = "憨吉寵物";
        fields["SenderPhone"] = "047772277";
        fields["SenderZipCode"] = "505";
        fields["SenderAddress"] = "彰化縣鹿港鎮彰鹿路6段78號";
        fields["ReceiverName"] = order.ReceiverName;
        fields["ReceiverCellPhone"] = order.ReceiverTel;
        fields["ReceiverZipCode"] = "500";
        fields["ReceiverAddress"] = order.ReceiverAdd;
        fields["ClientReplyURL"] = $"{_configuration["URL:BackEnd"]}/api/Logistics/{order.OrderNumber}";
        fields["ServerReplyURL"] = $"{_configuration["URL:BackEnd"]}/api/Logistics/notify/{_payService}/{order.OrderNumber}";
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
        checkValue = $"HashKey={_HashKey}&" + checkValue + $"&HashIV={_HashIV}";
        checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
        checkValue = Payment.ComputeMD5(checkValue).ToUpper();

        return checkValue;
    }
}