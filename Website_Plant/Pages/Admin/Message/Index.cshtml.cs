using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Website_Plant.Pages.Admin.Message
{
    public class IndexModel : PageModel
    {
        public List<Contact> Contacts = new List<Contact>();
        public int page = 1;
        public int totalPage = 0;
        private readonly int pageSize = 6;

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
                    string sqlCount = "select count(*) from Contact";
                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        decimal count = (int)command.ExecuteScalar();
                        totalPage = (int)Math.Ceiling(count / pageSize);
                    }
                    string sql = "SELECT * FROM Contact order by id desc";
                    sql += " offset @skip ROWS FETCH NEXT @pageSize ROWS ONLY";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Contact contact = new Contact();
                                contact.ID = reader.GetInt32(0); 
                                contact.FullName = reader.GetString(1);
                                contact.Email = reader.GetString(2);
                                contact.PhoneNumber = reader.GetString(3);
                                contact.Message = reader.GetString(4);
                                contact.CreatedAt = reader.GetDateTime(5).ToString("dd/MM/yyyy");
                                Contacts.Add(contact);
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

    public class Contact
    {
        public int ID { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Message { get; set; } = "";
        public string CreatedAt { get; set; } = "";
    }
}
