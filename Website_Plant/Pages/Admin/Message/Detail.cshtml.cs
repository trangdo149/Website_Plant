using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Website_Plant.Pages.Admin.Message
{
    public class DetailModel : PageModel
    {
        public Contact contact = new Contact();

        public void OnGet()
        {
            string requestId = Request.Query["id"];
            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Contact where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", requestId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                contact.ID = reader.GetInt32(0);
                                contact.FullName = reader.GetString(1);
                                contact.PhoneNumber = reader.GetString(2);
                                contact.Email = reader.GetString(3);
                                contact.Message = reader.GetString(4);
                                contact.CreatedAt = reader.GetDateTime(5).ToString("dd/MM/yyyy");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("Admin/Message/Index");
            }
        }
    }
}
