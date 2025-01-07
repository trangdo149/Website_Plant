using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Website_Plant.MyHelpers;
using Website_Plant.Pages.Admin.Order;

namespace Website_Plant.Pages.Client.Order
{
    [RequireAuth(RequiredRole = "client")]
    public class IndexModel : PageModel
    {
        public List<OrderInfo> listOrders = new List<OrderInfo>();

        public int page = 1;
        public int totalPages = 0;
        private readonly int pageSize = 3;

        public void OnGet()
        {
            int clientId = HttpContext.Session.GetInt32("id") ?? 0;
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

                    string sqlCount = "select count(*) from orders where client_id=@client_id";
                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@client_id", clientId);
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }



                    string sql = "select * from orders where client_id=@client_id order by id desc";
                    sql += " offset @skip rows fetch next @pageSize rows only";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@client_id", clientId);
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
