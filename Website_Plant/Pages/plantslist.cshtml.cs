using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Website_Plant.Pages.Admin.Plant;

namespace Website_Plant.Pages
{
    [BindProperties(SupportsGet = true)]
    public class plantslistModel : PageModel
    {
        public string? Search {  get; set; }
        public string Price { get; set; } = "AnyPrice";
        public string Category { get; set; } = "AnyCategory";
        public List<PlantInfo> listPlants = new List<PlantInfo>();
        public int page = 1;
        public int totalPage = 0;
        private readonly int pageSize = 8;

        public void OnGet()
        {
            page = 1;
            string requestPage = Request.Query["page"];
            if (requestPage != null)
            {
                try
                {
                    page = int.Parse(requestPage);
                }
                catch (Exception ex)
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
                    string sqlCount = "select count(*) from plants where 1=1";
                    if (!string.IsNullOrEmpty(Search))
                    {
                        sqlCount += " and tensp like @search";
                    }
                    if (!Price.Equals("AnyPrice"))
                    {
                        if (Price.Equals("low"))
                        {
                            sqlCount += " and gia < 100000";
                        }
                        else if (Price.Equals("medium"))
                        {
                            sqlCount += " and gia >= 100000 and gia <= 200000";
                        }
                        else if (Price.Equals("high"))
                        {
                            sqlCount += " and gia > 200000";
                        }
                    }
                    if (!Category.Equals("AnyCategory"))
                    {
                        sqlCount += " and danhmuc = @danhmuc";
                    }
                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        if (!string.IsNullOrEmpty(Search))
                        {
                            command.Parameters.AddWithValue("@search", "%" + Search + "%");
                        }
                        if (!Category.Equals("AnyCategory"))
                        {
                            command.Parameters.AddWithValue("@danhmuc", Category);
                        }
                        decimal count = (int)command.ExecuteScalar();
                        totalPage = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "select * from plants where 1=1";
                    if (!string.IsNullOrEmpty(Search))
                    {
                        sql += " and tensp like @search";
                    }
                    if (!Price.Equals("AnyPrice"))
                    {
                        if (Price.Equals("low"))
                        {
                            sql += " and gia < 100000";
                        }
                        else if (Price.Equals("medium"))
                        {
                            sql += " and gia >= 100000 and gia <= 200000";
                        }
                        else if (Price.Equals("high"))
                        {
                            sql += " and gia > 200000";
                        }
                    }
                    if (!Category.Equals("AnyCategory"))
                    {
                        sql += " and danhmuc = @danhmuc";
                    }
                    sql += " order by id DESC";
                    sql += " offset @skip ROWS FETCH NEXT @pageSize ROWS ONLY";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        if (!string.IsNullOrEmpty(Search))
                        {
                            command.Parameters.AddWithValue("@search", "%" + Search + "%");
                        }
                        if (!Category.Equals("AnyCategory"))
                        {
                            command.Parameters.AddWithValue("@danhmuc", Category);
                        }
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            listPlants.Clear();
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
