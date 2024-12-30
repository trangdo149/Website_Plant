using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
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
        }
        public void OnPostRedirectToVnpay()
        {
            // Cấu hình thông tin VNPay
            string vnp_Returnurl = "http://localhost:5237/CallbackVnpay"; // URL nhận kết quả từ VNPay
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
            vnpay.AddRequestData("vnp_IpAddr", Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng tại Website Plant");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); // Mã tham chiếu giao dịch

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            // Chuyển hướng đến VNPay
            Response.Redirect(paymentUrl);
        }
    }
}
