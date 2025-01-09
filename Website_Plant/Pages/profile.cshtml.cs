using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Website_Plant.MyHelpers;

namespace Website_Plant.Pages
{
    [RequireAuth]
    [BindProperties]
    public class profileModel : PageModel
    {
        [Required(ErrorMessage = "*Bắt buộc")]
        public string fullName { get; set; } = "";
		[Required(ErrorMessage = "*Bắt buộc"), EmailAddress]
		public string email { get; set; } = "";
        public string? phone { get; set; } = "";
		[Required(ErrorMessage = "*Bắt buộc")]
		public string address { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
			fullName = HttpContext.Session.GetString("fullName")?? "";
			email = HttpContext.Session.GetString("email")?? "";
			phone = HttpContext.Session.GetString("phone");
			address = HttpContext.Session.GetString("address") ?? "";
		}
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Dữ liệu không hợp lệ";
				return Page();
			}
            if (phone == null) phone = "";
            string submitButton = Request.Form["action"];
            string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
            if (submitButton.Equals("profile"))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "update users set fullName=@fullName, email=@email, phone=@phone, address=@address where id=@id";
                        int? id = HttpContext.Session.GetInt32("id");
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@fullName", fullName);
							command.Parameters.AddWithValue("@email", email);
							command.Parameters.AddWithValue("@phone", phone);
							command.Parameters.AddWithValue("@address", address);
							command.Parameters.AddWithValue("@id", id);
                            command.ExecuteNonQuery();
						}
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
					return Page();
				}
                HttpContext.Session.SetString("fullName", fullName);
				HttpContext.Session.SetString("email", email);
				HttpContext.Session.SetString("phone", phone);
				HttpContext.Session.SetString("address", address);
				successMessage = "Cập nhật thành công";
			}
			return RedirectToPage("/profile");
		}
    }
}
