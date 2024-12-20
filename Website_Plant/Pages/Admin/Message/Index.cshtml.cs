using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Website_Plant.Pages.Admin.Message
{
    public class IndexModel : PageModel
    {
        public List<Contact> Contacts { get; private set; } = new List<Contact>();
        public string ErrorMessage { get; private set; } = "";

        public void OnGet()
        {
            string connectionString = "Data Source=LAPTOP-H0QVT377\\SQLEXPRESS07;Initial Catalog=ContactDB;Integrated Security=True;Trust Server Certificate=True";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Contact";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Contact contact = new Contact
                                {
                                    ID = reader.GetInt32(0), // Lấy ID từ cột đầu tiên
                                    FullName = reader.GetString(1), // Lấy Họ và Tên từ cột thứ hai
                                    Email = reader.GetString(2), // Lấy Email từ cột thứ ba
                                    PhoneNumber = reader.GetString(3) // Lấy Số điện thoại từ cột thứ tư
                                };

                                Contacts.Add(contact);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }

    public class Contact
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
