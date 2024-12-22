using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using Website_Plant.MyHelpers;

namespace Website_Plant.Pages.Admin.Plant
{
    [RequireAuth(RequiredRole = "admin")]
    public class CreateModel : PageModel
    {
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
        [Required(ErrorMessage = "*Bắt buộc")]
        public IFormFile ImageFile { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        private IWebHostEnvironment webHostEnvironment;

        public CreateModel(IWebHostEnvironment environment)
        {
            webHostEnvironment = environment;
        }
        public void OnGet()
        {            
        }
        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Hãy nhập đầy đủ thông tin ";
                return;
            }
            if (mota == null) mota = "";

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(ImageFile.FileName);

            string imageFolder = webHostEnvironment.WebRootPath + "/image/plants/";

            string imageFullPath = Path.Combine(imageFolder, newFileName);
            Console.WriteLine("New image: " + imageFullPath);

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                ImageFile.CopyTo(stream);
            }
            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO plants " +
                    "(tensp, mota, anhsang, tuoinuoc, nhietdo, danhmuc, gia, image_filename) VALUES " +
                    "(@tensp, @mota, @anhsang, @tuoinuoc, @nhietdo, @danhmuc, @gia, @image_filename);";

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

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            successMessage = "Thêm sản phẩm thành công";
            Response.Redirect("/Admin/Plant/Index");
        }
    }
}
