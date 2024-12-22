using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using Website_Plant.MyHelpers;

namespace Website_Plant.Pages.Admin.Plant
{
    [RequireAuth(RequiredRole = "admin")]
    public class EditModel : PageModel
    {
        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "*Bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên sản phẩm không vượt quá 100 kí tự")]
        public string tensp { get; set; } = "";

        [BindProperty]
        public string? mota { get; set; } = "";

        [BindProperty, Required]
        public string anhsang { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "*Bắt buộc")]
        [MaxLength(100, ErrorMessage = "không vượt quá 100 kí tự")]
        public string tuoinuoc { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "*Bắt buộc")]
        [MaxLength(100, ErrorMessage = "không vượt quá 100 kí tự")]
        public string nhietdo { get; set; } = "";

        [BindProperty, Required]
        public string danhmuc { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "*Bắt buộc")]
        public decimal gia { get; set; }

        [BindProperty]
        public string ImageFileName { get; set; } = "";

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public string errorMessage = "";
        public string successMessage = "";
        private IWebHostEnvironment webHostEnvironment;

        public EditModel(IWebHostEnvironment environment)
        {
            webHostEnvironment = environment;
        }
        public void OnGet()
        {
            string requestId = Request.Query["id"];
            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select * from plants where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", requestId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Id = reader.GetInt32(0);
                                tensp = reader.GetString(1);
                                mota = reader.GetString(2);
                                anhsang = reader.GetString(3);
                                tuoinuoc = reader.GetString(4);
                                nhietdo = reader.GetString(5);
                                danhmuc = reader.GetString(6);
                                gia = reader.GetDecimal(7);
                                ImageFileName = reader.GetString(8);
                            }
                            else
                            {
                                Response.Redirect("Admin/Plant");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Admin/Plant/Index");
            }
        }
        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Hãy nhập đầy đủ thông tin ";
                return;
            }
            if (mota == null) mota = "";
            string newFileName = ImageFileName;
            if (ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(ImageFile.FileName);

                string imageFolder = webHostEnvironment.WebRootPath + "/image/plants/";
                string imageFullPath = Path.Combine(imageFolder, newFileName);


                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    ImageFile.CopyTo(stream);
                }

                string oldimgFullPath = Path.Combine(imageFolder, ImageFileName);
                System.IO.File.Delete(oldimgFullPath);
                Console.WriteLine("Delete Image" + oldimgFullPath);
            }
            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "update plants set tensp=@tensp, mota=@mota, anhsang=@anhsang, tuoinuoc=@tuoinuoc, nhietdo=@nhietdo, danhmuc=@danhmuc, gia=@gia, image_filename=@image_filename where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@tensp", tensp);
                        command.Parameters.AddWithValue("@mota", mota);
                        command.Parameters.AddWithValue("@anhsang", anhsang);
                        command.Parameters.AddWithValue("@tuoinuoc", tuoinuoc);
                        command.Parameters.AddWithValue("@nhietdo", nhietdo);
                        command.Parameters.AddWithValue("@danhmuc", danhmuc);
                        command.Parameters.AddWithValue("@gia", gia);
                        command.Parameters.AddWithValue("@image_filename", newFileName);
                        command.Parameters.AddWithValue("@id", Id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            successMessage = "Sửa thông tin sản phẩm thành công";
            Response.Redirect("/Admin/Plant");
        }
    }
}
