using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using Website_Plant.MyHelpers;
using Website_Plant.Services;

namespace Website_Plant.Pages.Auth
{
    [RequireNoAuth]
    [BindProperties]
	public class signinModel : PageModel
    {
		private readonly IConfiguration configuration;

		[Required(ErrorMessage = "Email không được để trống")]
		[EmailAddress]
		public string email { get; set; } = "";
		[Required(ErrorMessage = "Mật khẩu không được để trống")]
		public string password { get; set; } = "";
		public string? RecapchaToken { get; set; }

		public string errorMessage = "";
		public string successMessage = "";

		public signinModel(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

        public void OnGet()
        {
        }
		public async Task OnPost()
		{
			if (!ModelState.IsValid)
			{
				errorMessage = "Dữ liệu không hợp lệ";
				return;
			}
			if(RecapchaToken == null)
			{
				errorMessage = "Hãy xác minh không phải robot";
				return;
			}
			Console.WriteLine("We got the token: " + RecapchaToken);

			string secretKey = configuration["ReCapchaSettings:SecretKey"];
			bool success = await ReCaptchaService.verifyReCaptchaV2(RecapchaToken, secretKey);
			if (!success)
			{
				errorMessage = "Recaptcha không hợp lệ";
				return;
			}

			try
			{
				string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "SELECT * FROM users WHERE email=@email";

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
								string hashedPassword = reader.GetString(5);
								string role = reader.GetString(6);
								string created_at = reader.GetDateTime(7).ToString("MM/dd/yyyy");

								var passwordHasher = new PasswordHasher<IdentityUser>();
								var result = passwordHasher.VerifyHashedPassword(new IdentityUser(), hashedPassword, password);

								if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
								{
									HttpContext.Session.SetInt32("id", id);
									HttpContext.Session.SetString("fullName", fullName);
									HttpContext.Session.SetString("email", email);
									HttpContext.Session.SetString("phone", phone);
									HttpContext.Session.SetString("address", address);
									HttpContext.Session.SetString("role", role);
									HttpContext.Session.SetString("created_at", created_at);
                                    if (role == "admin")
                                    {
                                        Response.Redirect("/Admin/Plant/Index");
                                    }
                                    else
                                    {
                                        Response.Redirect("/");
                                    }
                                    return;
								}

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

			errorMessage = "Email hoặc mật khẩu không chính xác";
		}
	}
}
