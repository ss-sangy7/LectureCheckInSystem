using System;
using System.Collections.Generic;
using System.Configuration; // for reading config settings
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security; // for hashing (or use BCrypt)
using System.Web.UI;
using System.Web.UI.WebControls;
using BCryptNet = BCrypt.Net.BCrypt;


namespace LectureCheckInSystem.Accounts
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Its optional as a redirect if already logged in
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string selectedRole = ddlRole.SelectedValue;

            string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT LecturerID, FirstName, LastName, PasswordHash, Role FROM Lecturers WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int lecturerId = reader.GetInt32(0);
                    string firstName = reader.GetString(1);
                    string lastName = reader.GetString(2);
                    string storedHash = reader.GetString(3);
                    string dbRole = reader.GetString(4);

                    // Verify password (using BCrypt or SHA256)
                    if (BCryptNet.Verify(password, storedHash))
                    {
                        // lets check if the selected role matches the database role
                        if (selectedRole != dbRole)
                        {
                            ShowError($"You are registered as {dbRole}. Please select the correct role.");
                            return;
                        }

                        Session["LecturerID"] = lecturerId;
                        Session["FullName"] = $"{firstName} {lastName}";
                        Session["Role"] = dbRole;
                        Session["IsAdmin"] = (dbRole == "Admin");

                        if (dbRole == "Admin")
                            Response.Redirect("~/Admin/Dashboard.aspx");
                        else
                            Response.Redirect("~/Lecturer/CheckIn.aspx");
                    }
                    else
                        ShowError("Invalid username or password.");
                }
                else
                    ShowError("Invalid username or password.");
            }
        }



        private void ShowError(string msg)
        {
            ClientScript.RegisterStartupScript(GetType(), "alert", $"alert('{msg}');", true);
        }
    }
}