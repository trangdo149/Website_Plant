using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using Website_Plant.Pages.Admin.Plant;

namespace Website_Plant.Pages
{
    [BindProperties]
    public class CartModel : PageModel
    {
        [Required(ErrorMessage = "Hãy nhập địa chỉ giao hàng")]
        public string Address { get; set; } = "";
        [Required]
        public string PaymentMethod { get; set; } = "";
        public class OrderItem
        {
            public PlantInfo plantInfo = new PlantInfo();
            public int numCopies = 0;
            public decimal totalPrice = 0;
        }
        public List<OrderItem> listOrderItem = new List<OrderItem>();
        public decimal totalproduct = 0;
        public decimal shippingfee = 15000;
        public decimal total = 0;
        private Dictionary<String, int> getPlantDictionary()
        {
            var plantDictionary = new Dictionary<String, int>();
            //đọc sp trong giỏ hàng từ cookie
            string cookieValue = Request.Cookies["shopping_cart"] ?? "";
            if (cookieValue.Length > 0)
            {
                string[] plantIdArray = cookieValue.Split('-');
                for (int i = 0; i < plantIdArray.Length; i++)
                {
                    string plantId = plantIdArray[i];
                    if (plantDictionary.ContainsKey(plantId))
                    {
                        plantDictionary[plantId] += 1;
                    }
                    else
                    {
                        plantDictionary.Add(plantId, 1);
                    }
                }
            }
            return plantDictionary;
        }
        public void OnGet()
        {
            var plantDictionary = getPlantDictionary();
            string? action = Request.Query["action"];
            string? id = Request.Query["id"];
            if (action != null && id != null && plantDictionary.ContainsKey(id))
            {
                if (action.Equals("add"))
                {
                    plantDictionary[id] += 1; // tăng prodictionary có id của sp đó thêm 1
                }
                else if (action.Equals("sub"))
                {
                    if (plantDictionary[id] > 1) plantDictionary[id] -= 1;
                }
                else if (action.Equals("delete"))
                {
                    plantDictionary.Remove(id);
                }

                //tạo cookie mới
                string newCookieValue = "";
                foreach (var keyValuePair in plantDictionary) //thêm nhiều bản copy của cặp prodictionaary
                {
                    for (int i = 0; i < keyValuePair.Value; i++)// cho phép thêm id sp nhiều lần vào cookie
                    {
                        newCookieValue += "-" + keyValuePair.Key;
                    }
                }
                if (newCookieValue.Length > 0) // in case, xóa sp có slg 1
                {
                    newCookieValue = newCookieValue.Substring(1);
                }
                var cookieOptions = new CookieOptions(); // cookieOption cho phép đnghia thuộc tính của cookie
                cookieOptions.Expires = DateTime.Now.AddDays(365);
                cookieOptions.Path = "/";

                Response.Cookies.Append("shopping_cart", newCookieValue, cookieOptions);

                // chuyển hướng đến trang khi đã remove câu truy vấn từ url, hoặc khi giỏ hàng update cookie
                Response.Redirect(Request.Path.ToString());
                return;
            }
            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select * from plants where id=@id";
                    //truy vấn cho mỗi cặp (id, numcopies) trong dictionary
                    foreach (var keyValuePair in plantDictionary) // đọc id sp từ cặp dictionary
                    {
                        string plantID = keyValuePair.Key;
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@id", plantID);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    OrderItem item = new OrderItem();
                                    item.plantInfo.Id = reader.GetInt32(0);
                                    item.plantInfo.tensp = reader.GetString(1);
                                    item.plantInfo.mota = reader.GetString(2);
                                    item.plantInfo.anhsang = reader.GetString(3);
                                    item.plantInfo.tuoinuoc = reader.GetString(4);
                                    item.plantInfo.nhietdo = reader.GetString(5);
                                    item.plantInfo.danhmuc = reader.GetString(6);
                                    item.plantInfo.gia = reader.GetDecimal(7);
                                    item.plantInfo.ImageFileName = reader.GetString(8);
                                    item.plantInfo.ngaytao = reader.GetDateTime(9).ToString("dd/MM/yyyy");

                                    item.numCopies = keyValuePair.Value; // lưu numCopies
                                    item.totalPrice = item.numCopies * item.plantInfo.gia;

                                    listOrderItem.Add(item);

                                    totalproduct += item.totalPrice;
                                    total = totalproduct + shippingfee;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Address = HttpContext.Session.GetString("address") ?? "";
            TempData["Total"] = "" + total;
            TempData["ProductIdentifiers"] = "";
            TempData["DeliveryAddress"] = "";
            
        }
        public string errorMessage = "";
        public string successMessage = "";
        public void OnPost()
        {
            int client_id = HttpContext.Session.GetInt32("id") ?? 0;
            if (client_id < 1)
            {
                Response.Redirect("/Auth/signin");
                return;
            }
            if (!ModelState.IsValid)
            {
                errorMessage = "*Lỗi";
                return;
            }           
            var plantDictionary = getPlantDictionary();//đọc sp trong giỏ hàng từ cookie
            if (plantDictionary.Count < 1)
            {
                errorMessage = "Giỏ hàng trống";
                return;
            }
            string productIdentifiers = Request.Cookies["shopping_cart"] ?? "";
            TempData["ProductIdentifiers"] = productIdentifiers;
            TempData["DeliveryAddress"] = Address;
            if (PaymentMethod == "vnpay")
            {
                Response.Redirect("/CheckoutVnpay");
                return;
            }
            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    int newOrderId = 0;
                    string sqlOrder = "insert into orders (client_id, order_date, shipping_fee, delivery_address, payment_method, order_status) " +
                        "output inserted.id " +
                        "values (@client_id, current_timestamp, @shipping_fee, @delivery_address, @payment_method, N'Chờ xác nhận')";
                    using (SqlCommand command = new SqlCommand(sqlOrder, connection))
                    {
                        command.Parameters.AddWithValue("@client_id", client_id);
                        command.Parameters.AddWithValue("@shipping_fee", shippingfee);
                        command.Parameters.AddWithValue("@delivery_address", Address);
                        command.Parameters.AddWithValue("@payment_method", PaymentMethod);

                        newOrderId = (int)command.ExecuteScalar();

                    }
                    string sqlItem = "insert into order_items (order_id, plant_id, quantity, unit_price) " +
                        "values (@order_id, @plant_id, @quantity, @unit_price)";
                    foreach (var keyValuePair in plantDictionary)
                    {
                        string plantId = keyValuePair.Key;
                        int quantity = keyValuePair.Value;
                        decimal unitPrice = getPlantPrice(plantId);

                        using (SqlCommand command = new SqlCommand(sqlItem, connection))
                        {
                            command.Parameters.AddWithValue("@order_id", newOrderId);
                            command.Parameters.AddWithValue("@plant_id", plantId);
                            command.Parameters.AddWithValue("@quantity", quantity);
                            command.Parameters.AddWithValue("@unit_price", unitPrice);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            //xóa cookie khỏi trình duyệt
            Response.Cookies.Delete("shopping_cart");

            Response.Redirect("/thankyou");
        }
        private decimal getPlantPrice(string plantId)
        {
            decimal price = 0;
            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select gia from plants where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", plantId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                price = reader.GetDecimal(0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return price;
        }
    }
}
