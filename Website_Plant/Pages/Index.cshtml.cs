using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Website_Plant.Pages.Admin.Plant;

namespace Website_Plant.Pages
{
    public class IndexModel : PageModel
    {
        public List<PlantInfo> listNewestPlants = new List<PlantInfo>();
        public List<PlantInfo> listTopSales = new List<PlantInfo>();

        public void OnGet()
        {
			try
			{
				string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "select top 5 * from plants order by ngaytao desc";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
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
								listNewestPlants.Add(plantInfo);
							}
						}
					}

					sql = "select top 5 plants.*, (" +
						"select sum(order_items.quantity) from order_items where plants.id = order_items.plant_id" +
						") as total_sales " +
						"from plants " +
						"order by total_sales desc";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
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
								listTopSales.Add(plantInfo);
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
