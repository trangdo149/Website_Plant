using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Website_Plant.MyHelpers;
using Website_Plant.Pages.Admin.Plant;

namespace Website_Plant.Pages.Admin.Order
{
    public class OrderItemInfo
    {
        public int id;
        public int orderId;
        public int plantId;
        public int quantity;
        public decimal unitPrice;
        public PlantInfo plantInfo = new PlantInfo();
    }
    public class OrderInfo
    {
        public int id;
        public int clientId;
        public string orderDate;
        public decimal shippingFee;
        public string deliveryAddress;
        public string paymentMethod;
        public string orderStatus;
        public List<OrderItemInfo> items = new List<OrderItemInfo>();
        public static List<OrderItemInfo> getOrderItems(int orderId)
        {
            List<OrderItemInfo> items = new List<OrderItemInfo>();

            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select order_items.*, plants.* from order_items, plants where order_items.order_id=@order_id and order_items.plant_id = plants.id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@order_id", orderId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrderItemInfo item = new OrderItemInfo();

                                item.id = reader.GetInt32(0);
                                item.orderId = reader.GetInt32(1);
                                item.plantId = reader.GetInt32(2);
                                item.quantity = reader.GetInt32(3);
                                item.unitPrice = reader.GetDecimal(4);

                                item.plantInfo.Id = reader.GetInt32(5);
                                item.plantInfo.tensp = reader.GetString(6);
                                item.plantInfo.mota = reader.GetString(7);
                                item.plantInfo.anhsang = reader.GetString(8);
                                item.plantInfo.tuoinuoc = reader.GetString(9);
                                item.plantInfo.nhietdo = reader.GetString(10);
                                item.plantInfo.danhmuc = reader.GetString(11);
                                item.plantInfo.gia = reader.GetDecimal(12);
                                item.plantInfo.ImageFileName = reader.GetString(13);
                                item.plantInfo.ngaytao = reader.GetDateTime(14).ToString("dd/MM/yyyy");

                                items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return items;
        }
    }
    [RequireAuth(RequiredRole = "admin")]
    public class IndexModel : PageModel
    {
        public List<OrderInfo> listOrders = new List<OrderInfo>();
        public int page = 1;
        public int totalPages = 0;
        private readonly int pageSize = 6;
        public void OnGet()
        {
            try
            {
                string requestPage = Request.Query["page"];
                page = int.Parse(requestPage);
            }
            catch (Exception ex)
            {
                page = 1;
            }
            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlCount = "select count(*) from orders";
                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }



                    string sql = "select * from orders order by id desc";
                    sql += " offset @skip rows fetch next @pageSize rows only";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrderInfo orderInfo = new OrderInfo();
                                orderInfo.id = reader.GetInt32(0);
                                orderInfo.clientId = reader.GetInt32(1);
                                orderInfo.orderDate = reader.GetDateTime(2).ToString("dd/MM/yyyy");
                                orderInfo.shippingFee = reader.GetDecimal(3);
                                orderInfo.deliveryAddress = reader.GetString(4);
                                orderInfo.paymentMethod = reader.GetString(5);
                                orderInfo.orderStatus = reader.GetString(6);

                                orderInfo.items = OrderInfo.getOrderItems(orderInfo.id);
                                listOrders.Add(orderInfo);
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
