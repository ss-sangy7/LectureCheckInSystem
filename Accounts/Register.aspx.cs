using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using BCryptNet = BCrypt.Net.BCrypt;

namespace LectureCheckInSystem.Accounts
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // Server-side validation
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                ddlDepartment.SelectedValue == "")
            {
                ShowError("All fields are required.");
                return;
            }

            // Email format validation
            if (!IsValidEmail(txtEmail.Text))
            {
                ShowError("Please enter a valid email address.");
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                ShowError("Passwords do not match.");
                return;
            }

            if (txtPassword.Text.Length < 6)
            {
                ShowError("Password must be at least 6 characters.");
                return;
            }

            string hashedPassword = BCryptNet.HashPassword(txtPassword.Text);
            string selectedRole = ddlRole.SelectedValue;

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"INSERT INTO Lecturers (FirstName, LastName, Email, Username, PasswordHash, Department,Role)
                                 VALUES (@FirstName, @LastName, @Email, @Username, @PasswordHash, @Department,@Role)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                cmd.Parameters.AddWithValue("@Department", ddlDepartment.SelectedValue);
                cmd.Parameters.AddWithValue("@Role", selectedRole);
                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // Handle duplicate username/email (error number 2627 for unique constraint violation)
                    if (ex.Number == 2627)
                    {
                        ShowError("Username or Email already exists. Kindly choose another.");
                        return;
                    }
                    else
                    {
                        ShowError("An error occurred while creating your account. Kindly try again.");
                        return;
                    }
                }
            }

            ShowError("Account created successfully! You will now be redirected to the login page.");
            Response.Redirect("~/Accounts/Login.aspx");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ShowError(string message)
        {
            ClientScript.RegisterStartupScript(GetType(), "alert", $"alert('{message}');", true);
        }
    }
}