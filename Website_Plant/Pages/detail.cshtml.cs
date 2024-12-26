using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Website_Plant.Pages.Admin.Plant;

namespace Website_Plant.Pages
{
    public class detailModel : PageModel
    {
        public PlantInfo plantInfo = new PlantInfo();
        public void OnGet(int? id)
        {
            if (id == null)
            {
                Response.Redirect("/");
                return;
            }
            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select * from plants where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                plantInfo.Id = reader.GetInt32(0);
                                plantInfo.tensp = reader.GetString(1);
                                plantInfo.mota = reader.GetString(2);
                                plantInfo.anhsang = reader.GetString(3);
                                plantInfo.tuoinuoc = reader.GetString(4);
                                plantInfo.nhietdo = reader.GetString(5);
                                plantInfo.danhmuc = reader.GetString(6);
                                plantInfo.gia = reader.GetDecimal(7);
                                plantInfo.ImageFileName = reader.GetString(8);
                                plantInfo.ngaytao = reader.GetDateTime(9).ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                Response.Redirect("/");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/");
                return;
            }
        }
    }
}
