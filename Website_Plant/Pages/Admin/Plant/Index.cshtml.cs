using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Website_Plant.MyHelpers;

namespace Website_Plant.Pages.Admin.Plant
{
    [RequireAuth(RequiredRole = "admin")]
    public class IndexModel : PageModel
    {
        public List<PlantInfo> listPlants = new List<PlantInfo>();
        public string search = "";

        public int page = 1;
        public int totalPage = 0;
        private readonly int pageSize = 6;
        public void OnGet()
        {
            search = Request.Query["search"];
            if (search == null) search = "";
            page = 1;
            string requestPage = Request.Query["page"];
            if (requestPage != null)
            {
                try
                {
                    page = int.Parse(requestPage);
                }
                catch(Exception ex)
                {
                    page = 1;
                }
            }
            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlCount = "select count(*) from plants";
                    if (search.Length > 0)
                    {
                        sqlCount += " where tensp like @search";
                    }
                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + search + "%");

                        decimal count = (int)command.ExecuteScalar();
                        totalPage = (int)Math.Ceiling(count / pageSize);
                    }
                    string sql = "SELECT * FROM plants";
                    if (search.Length > 0)
                    {
                        sql += " where tensp like @search";
                    }
                    sql += " order by id desc";
                    sql += " offset @skip ROWS FETCH NEXT @pageSize ROWS ONLY";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + search + "%");
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);
                        using (SqlDataReader reader = command.ExecuteReader()) 
                        {
                            while (reader.Read()) 
                            {
                                PlantInfo plantInfo = new PlantInfo();
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

                                listPlants.Add(plantInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                ViewData["ErrorMessage"] = ex.Message;
            }
        }
    }
    public class PlantInfo
    {
        public int Id { get; set; }
        public string tensp { get; set; } = "";
        public string mota { get; set; } = "";
        public string anhsang { get; set; } = "";
        public string tuoinuoc { get; set; } = "";
        public string nhietdo { get; set; } = "";
        public string danhmuc { get; set; } = "";
        public decimal gia { get; set; }
        public string ImageFileName { get; set; } = "";
        public string ngaytao { get; set; } = "";
    }
}
