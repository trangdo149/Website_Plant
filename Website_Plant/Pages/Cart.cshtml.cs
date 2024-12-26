using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Website_Plant.Pages.Admin.Plant;

namespace Website_Plant.Pages
{
    public class CartModel : PageModel
    {
        public class OrderItem
        {
            public PlantInfo plantInfo = new PlantInfo();
            public int numCopies = 0;
            public decimal totalPrice = 0;
        }
        public List<OrderItem> listOrderItem = new List<OrderItem>();
        private Dictionary<String, int> getPlantDictionary()
        {
            var plantDictionary = new Dictionary<String, int>();
            //đọc sp trong giỏ hàng từ cookie
            string cookieValue = Request.Cookies["shopping_cart"] ?? "";
            if (cookieValue.Length > 0)
            {
                string[] plantIdArray = cookieValue.Split('-');
                for (int i = 0; i < plantIdArray.Length; i++) // tìm numCopies
                {
                    string plantId = plantIdArray[i];
                    if (plantDictionary.ContainsKey(plantId))//nếu id sp đã có trong dictionary thì tăng prdictionary thêm 1
                    {
                        plantDictionary[plantId] += 1;
                    }
                    else
                    {
                        plantDictionary.Add(plantId, 1); //nếu ko có thì thêm id sp và numCopies vào
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

                                    //totalproduct += item.totalPrice;
                                    //total = totalproduct + shippingfee;
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
        }
    }
}
