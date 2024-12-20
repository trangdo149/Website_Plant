using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Website_Plant.Pages
{
    public class contactusModel : PageModel
    {
        public void OnGet()
        {
        }
        [BindProperty]
        public string FullName { get; set; }= "";
        [BindProperty]
        public string PhoneNumber { get; set; } = "";
        [BindProperty]
        [EmailAddress]
        public string Email { get; set; } = "";
        [BindProperty]
        public string Message { get; set; } = "";

        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";
        public void OnPost()
        {
            FullName = Request.Form["fullname"];
            PhoneNumber = Request.Form["phone"];
            Email = Request.Form["email"];
            Message = Request.Form["message"];

            if (FullName.Length == 0 || PhoneNumber.Length == 0 || Email.Length == 0 || Message.Length == 0)
            {
                ErrorMessage = "Vui lòng nhập hết thông tin";
                return;
            }
            else
            {
                SuccessMessage = "Lời nhắn của bạn đã được gửi";

          
            }
            try
            {
                string connectionString = "Data Source=Localhost\\sqlexpress;Initial Catalog=WebPlant;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Contact" +
                    "(fullname, phonenumber, email, message) VALUES " +
                    "(@fullname, @phonenumber,  @email, @message);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@fullname", FullName);
                        command.Parameters.AddWithValue("@phonenumber", PhoneNumber);
                        command.Parameters.AddWithValue("@email", Email);
                        command.Parameters.AddWithValue("@message", Message);

                        command .ExecuteNonQuery();
                    }
                }

                FullName = "";
                PhoneNumber = "";
                Email = "";
                Message = "";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
