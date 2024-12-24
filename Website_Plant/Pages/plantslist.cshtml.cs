using Azure;
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
        
        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //string sqlCount = "select count(*) from Products";
                    //using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    //{
                    //    decimal count = (int)command.ExecuteScalar();
                    //    totalPages = (int)Math.Ceiling(count / pageSize);
                    //}
                    //string sql = "select * from plants";
                    //sql += " order by ProductId DESC";
                    //sql += " offset @skip ROWS FETCH NEXT @pageSize ROWS ONLY";
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
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        //command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        //command.Parameters.AddWithValue("@pageSize", pageSize);
                        if (!string.IsNullOrEmpty(Search))
                        {
                            command.Parameters.AddWithValue("@search", "%" + Search + "%");
                        }
                        if (!Category.Equals("AnyCategory"))
                        {
                            command.Parameters.AddWithValue("@danhmuc", Category);
                        }
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
        //public void OnGetSearch()
        //{
        //    try
        //    {
        //        listPlants.Clear();
        //        string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();
        //            //string sqlCount = "select count(*) from Products";
        //            //sqlCount += " where ProductName like @search";

        //            //using (SqlCommand command = new SqlCommand(sqlCount, connection))
        //            //{
        //            //    command.Parameters.AddWithValue("@search", "%" + Search + "%");

        //            //    decimal count = (int)command.ExecuteScalar();
        //            //    totalPages = (int)Math.Ceiling(count / pageSize);
        //            //}

        //            string sql = "select * from plants where tensp like @search";

        //            //sql += " order by ProductId DESC";
        //            //sql += " offset @skip ROWS FETCH NEXT @pageSize ROWS ONLY";
        //            using (SqlCommand command = new SqlCommand(sql, connection))
        //            {
        //                command.Parameters.AddWithValue("@search", "%" + Search + "%");
        //                //command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
        //                //command.Parameters.AddWithValue("@pageSize", pageSize);
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        PlantInfo plantInfo = new PlantInfo();
        //                        plantInfo.Id = reader.GetInt32(0);
        //                        plantInfo.tensp = reader.GetString(1);
        //                        plantInfo.mota = reader.GetString(2);
        //                        plantInfo.anhsang = reader.GetString(3);
        //                        plantInfo.tuoinuoc = reader.GetString(4);
        //                        plantInfo.nhietdo = reader.GetString(5);
        //                        plantInfo.danhmuc = reader.GetString(6);
        //                        plantInfo.gia = reader.GetDecimal(7);
        //                        plantInfo.ImageFileName = reader.GetString(8);
        //                        plantInfo.ngaytao = reader.GetDateTime(9).ToString("dd/MM/yyyy");
        //                        listPlants.Add(plantInfo);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
        //public void OnGetFilter()
        //{
        //    try
        //    {
        //        listPlants.Clear();
        //        string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();
        //            //string sqlCount = "select count(*) from Products";
        //            //sqlCount += " where ProductName like @search";

        //            //using (SqlCommand command = new SqlCommand(sqlCount, connection))
        //            //{
        //            //    command.Parameters.AddWithValue("@search", "%" + Search + "%");

        //            //    decimal count = (int)command.ExecuteScalar();
        //            //    totalPages = (int)Math.Ceiling(count / pageSize);
        //            //}

        //            string sql = "select * from plants where";
        //            if(Price.Equals("low"))
        //            {
        //                sql += " price <100000";
        //            }
        //            else if(Price.Equals("medium"))
        //            {
        //                sql += " price >=100000 and price <=200000";
        //            }
        //            else if(Price.Equals("high"))
        //            {
        //                sql += " price >200000";
        //            }
        //            if (!Category.Equals("AnyCategory"))
        //            {
        //                sql += " and danhmuc=@danhmuc";
        //            }

        //            //sql += " order by ProductId DESC";
        //            //sql += " offset @skip ROWS FETCH NEXT @pageSize ROWS ONLY";
        //            using (SqlCommand command = new SqlCommand(sql, connection))
        //            {
        //                command.Parameters.AddWithValue("@danhmuc", Category);
        //                //command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
        //                //command.Parameters.AddWithValue("@pageSize", pageSize);
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        PlantInfo plantInfo = new PlantInfo();
        //                        plantInfo.Id = reader.GetInt32(0);
        //                        plantInfo.tensp = reader.GetString(1);
        //                        plantInfo.mota = reader.GetString(2);
        //                        plantInfo.anhsang = reader.GetString(3);
        //                        plantInfo.tuoinuoc = reader.GetString(4);
        //                        plantInfo.nhietdo = reader.GetString(5);
        //                        plantInfo.danhmuc = reader.GetString(6);
        //                        plantInfo.gia = reader.GetDecimal(7);
        //                        plantInfo.ImageFileName = reader.GetString(8);
        //                        plantInfo.ngaytao = reader.GetDateTime(9).ToString("dd/MM/yyyy");
        //                        listPlants.Add(plantInfo);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
    }
}
