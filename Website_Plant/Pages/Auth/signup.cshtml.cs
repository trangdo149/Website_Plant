using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using Website_Plant.MyHelpers;

namespace Website_Plant.Pages.Auth
{
    [RequireNoAuth]
    [BindProperties]
    public class signupModel : PageModel
    {
		[Required(ErrorMessage = "*Bắt buộc")]
		public string fullName { get; set; } = "";
		[Required(ErrorMessage = "*Bắt buộc")]
		[EmailAddress]
		public string email { get; set; } = "";
        public string? phone { get; set; } = "";
		[Required(ErrorMessage = "*Bắt buộc")]
		public string address { get; set; } = "";
		[Required(ErrorMessage = "*Bắt buộc")]
        [MinLength(6, ErrorMessage ="Mật khẩu phải có ít nhất 6 ký tự")]
		public string password { get; set; } = "";
		[Required(ErrorMessage = "*Bắt buộc")]
		[Compare("password", ErrorMessage = "Mật khẩu nhập lại không khớp")]
		public string ConfirmPassword { get; set; } = "";

		public string errorMessage = "";
		public string successMessage = "";

		public void OnGet()
        {
        }
		public void OnPost()
		{
			if (!ModelState.IsValid)
			{
				errorMessage = "Hãy nhập đầy đủ thông tin";
				return;
			}

			if (phone == null) phone = "";


			string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

			try
			{
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "INSERT INTO users" +
						"(fullName, email, phone, address, password, role) VALUES" +
						"(@fullName, @email, @phone, @address, @password,'client');";

					var passwordHasher = new PasswordHasher<IdentityUser>();
					string hashedPassword = passwordHasher.HashPassword(new IdentityUser(), password);

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@fullName", fullName);
						command.Parameters.AddWithValue("@email", email);
						command.Parameters.AddWithValue("@phone", phone);
						command.Parameters.AddWithValue("@address", address);
						command.Parameters.AddWithValue("@password", hashedPassword);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.Message.Contains(email))
				{
					errorMessage = "Email này đã được sử dụng";
				}
				else
				{
					errorMessage = ex.Message;
				}
				return;
			}

			try
			{
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "select * from users where email=@email";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@email", email);
						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								int id = reader.GetInt32(0);
								string fullName = reader.GetString(1);
								string email = reader.GetString(2);
								string phone = reader.GetString(3);
								string address = reader.GetString(4);
                                //string hashedPassword = reader.GetString(5);
                                string role = reader.GetString(6);
								string created_at = reader.GetDateTime(7).ToString("dd/MM/yyyy");

								HttpContext.Session.SetInt32("id", id);
								HttpContext.Session.SetString("fullName", fullName);
								HttpContext.Session.SetString("email", email);
								HttpContext.Session.SetString("phone", phone);
								HttpContext.Session.SetString("address", address);
								HttpContext.Session.SetString("role", role);
								HttpContext.Session.SetString("created_at", created_at);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
				return;
			}

			successMessage = "Tạo tài khoản thành công";

			Response.Redirect("/");
		}
	}
}
