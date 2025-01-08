using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Net.Sockets;
using System.Net;
using Website_Plant.MyHelpers;

namespace Website_Plant.Pages
{
    public class CheckoutVnpayModel : PageModel
    {
        public string DeliveryAddress { get; set; } = "";
        public string Total { get; set; } = "";
        public string ProductIdentifiers { get; set; } = "";
        public void OnGet()
        {
            DeliveryAddress = TempData["DeliveryAddress"]?.ToString() ?? "";
            Total = TempData["Total"]?.ToString() ?? "";
            ProductIdentifiers = TempData["ProductIdentifiers"]?.ToString() ?? "";

            TempData.Keep(); // Giữ lại dữ liệu TempData khi tải lại trang
            if (DeliveryAddress == "" || Total == "" || ProductIdentifiers == "")
            {
                Console.WriteLine($"Total: {Total}, DeliveryAddress: {DeliveryAddress}, ProductIdentifiers: {ProductIdentifiers}");
                Response.Redirect("/");
                return;
            }
            Console.WriteLine($"Total: {Total}, DeliveryAddress: {DeliveryAddress}, ProductIdentifiers: {ProductIdentifiers}");

        }
        public void OnPostRedirectToVnpay()
        {
            DeliveryAddress = TempData["DeliveryAddress"]?.ToString() ?? "";
            Total = TempData["Total"]?.ToString() ?? "";
            ProductIdentifiers = TempData["ProductIdentifiers"]?.ToString() ?? "";

            TempData.Keep(); // Giữ lại dữ liệu TempData khi tải lại trang
            if (DeliveryAddress == "" || Total == "" || ProductIdentifiers == "")
            {
                Console.WriteLine($"Total: {Total}, DeliveryAddress: {DeliveryAddress}, ProductIdentifiers: {ProductIdentifiers}");
                Response.Redirect("/");
                return;
            }
            // Cấu hình thông tin VNPay
            string vnp_Returnurl = "http://localhost:5237/thankyou"; // URL nhận kết quả từ VNPay
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; // URL thanh toán VNPay
            string vnp_TmnCode = "JH6ZQB6C"; // Mã website của bạn
            string vnp_HashSecret = "CEVA3S04FEYIX7P466RXHJHKE37JAWDG"; // Chuỗi bí mật của bạn

            // Kiểm tra và xử lý giá trị Total
            if (string.IsNullOrEmpty(Total))
            {
                throw new Exception("Giá trị Total không được truyền hoặc rỗng.");
            }

            // Loại bỏ các ký tự không hợp lệ nếu cần
            string cleanedTotal = new string(Total.Where(char.IsDigit).ToArray());

            if (!decimal.TryParse(cleanedTotal, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal totalDecimal))
            {
                throw new Exception($"Giá trị Total không hợp lệ: {Total}");
            }

            // Chuyển đổi Total sang đơn vị VNPay yêu cầu (VNĐ x 100)
            string totalAmount = (totalDecimal * 100).ToString("0");

            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", totalAmount);
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", GetIpAddress(HttpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Don hang mua tu web plant");
            vnpay.AddRequestData("vnp_OrderType", "other");

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); // Mã tham chiếu giao dịch

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            // Chuyển hướng đến VNPay
            Response.Redirect(paymentUrl);
        }
        public static string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;

                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();
                    return ipAddress;
                }
            }
            catch (Exception ex)
            {
                return "Invalid IP:" + ex.Message;
            }

            return "127.0.0.1";
        }
    }
}
