using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Website_Plant.MyHelpers;

namespace Website_Plant.Pages.Admin.Order
{
    [RequireAuth(RequiredRole = "admin")]
    public class DetailModel : PageModel
    {
        public OrderInfo orderInfo = new OrderInfo();
        public UserInfo userInfo = new UserInfo();
        public void OnGet(int id)
        {
            if (id < 1)
            {
                Response.Redirect("/Admin/Order/Index");
                return;
            }
            string orderStatus = Request.Query["order_status"];

            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (orderStatus != null)
                    {
                        string sqlUpdate = "update orders set order_status=@order_status where id=@id";
                        using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                        {
                            command.Parameters.AddWithValue("@order_status", orderStatus);
                            command.Parameters.AddWithValue("@id", id);
                            command.ExecuteNonQuery();
                        }
                    }

                    string sql = "select * from orders where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                orderInfo.id = reader.GetInt32(0);
                                orderInfo.clientId = reader.GetInt32(1);
                                orderInfo.orderDate = reader.GetDateTime(2).ToString("dd/MM/yyyy");
                                orderInfo.shippingFee = reader.GetDecimal(3);
                                orderInfo.deliveryAddress = reader.GetString(4);
                                orderInfo.paymentMethod = reader.GetString(5);
                                orderInfo.orderStatus = reader.GetString(6);

                                orderInfo.items = OrderInfo.getOrderItems(orderInfo.id);
                            }
                            else
                            {
                                Response.Redirect("/Admin/Order/Index");
                                return;
                            }
                        }
                    }
                    sql = "select * from users where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", orderInfo.clientId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userInfo.id = reader.GetInt32(0);
                                userInfo.fullName = reader.GetString(1);
                                userInfo.email = reader.GetString(2);
                                userInfo.phone = reader.GetString(3);
                                userInfo.address = reader.GetString(4);
                                userInfo.password = reader.GetString(5);
                                userInfo.role = reader.GetString(6);
                                userInfo.created_at = reader.GetDateTime(7).ToString("dd/MM/yyyy");

                            }
                            else
                            {
                                Console.WriteLine("Không tìm thấy khách hàng có mã=" + orderInfo.clientId);
                                Response.Redirect("/Admin/Order/Index");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Admin/Order/Index");
            }
        }
    }
    public class UserInfo
    {
        public int id;
        public string fullName;
        public string email;
        public string phone;
        public string address;
        public string password;
        public string role;
        public string created_at;
    }
}
