using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCryptNet = BCrypt.Net.BCrypt;

namespace LectureCheckInSystem.Admin
{
    public partial class ManageUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Only admin can access this page
            if (Session["LecturerID"] == null || Session["Role"]?.ToString() != "Admin")
                Response.Redirect("~/Accounts/Login.aspx");

            if (!IsPostBack)
                LoadUsers();
        }

        private void LoadUsers()
        {
            string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT LecturerID, FirstName, LastName, Email, Username, Department 
                                 FROM Lecturers ORDER BY LecturerID";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvUsers.DataSource = dt;
                gvUsers.DataBind();
            }
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int userId = Convert.ToInt32(e.CommandArgument);
            string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;

            if (e.CommandName == "DeleteUser")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "DELETE FROM Lecturers WHERE LecturerID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", userId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadUsers(); // Refresh grid
                ShowMessage("User deleted successfully.");
            }
            else if (e.CommandName == "EditUser")
            {
                // Load user data into the modal for editing
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "SELECT FirstName, LastName, Email, Username, Department FROM Lecturers WHERE LecturerID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtFirstName.Text = reader["FirstName"].ToString();
                        txtLastName.Text = reader["LastName"].ToString();
                        txtEmail.Text = reader["Email"].ToString();
                        txtUsername.Text = reader["Username"].ToString();
                        txtDepartment.Text = reader["Department"].ToString();
                    }
                    reader.Close();
                }
                // Hide password fields when editing
                passwordFields.Visible = false;
                lblModalTitle.Text = "Edit Lecturer";
                ViewState["EditID"] = userId;
                pnlModal.Visible = true;
            }
            else if (e.CommandName == "ResetPassword")
            {
                // Generate a random temporary password (e.g., 8 characters)
                string tempPassword = GenerateRandomPassword();
                string hashedPassword = BCryptNet.HashPassword(tempPassword);

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "UPDATE Lecturers SET PasswordHash = @Hash WHERE LecturerID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Hash", hashedPassword);
                    cmd.Parameters.AddWithValue("@ID", userId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                ShowMessage($"Password reset successful. New temporary password: {tempPassword} (copy it now)");
            }
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            ClearModal();
            passwordFields.Visible = true;  // Show password field for new user
            lblModalTitle.Text = "Add Lecturer";
            pnlModal.Visible = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;

            if (lblModalTitle.Text == "Add Lecturer")
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    ShowMessage("Password is required for a new user.");
                    return;
                }
                string hashedPassword = BCryptNet.HashPassword(txtPassword.Text);

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = @"INSERT INTO Lecturers (FirstName, LastName, Email, Username, PasswordHash, Department, Role)
                                     VALUES (@FirstName, @LastName, @Email, @Username, @PasswordHash, @Department, 'Lecturer')";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    cmd.Parameters.AddWithValue("@Department", txtDepartment.Text);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                ShowMessage("User added successfully.");
            }
            else // Editing existing user
            {
                int editId = (int)ViewState["EditID"];
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = @"UPDATE Lecturers 
                                     SET FirstName = @FirstName, LastName = @LastName, Email = @Email, 
                                         Username = @Username, Department = @Department
                                     WHERE LecturerID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@Department", txtDepartment.Text);
                    cmd.Parameters.AddWithValue("@ID", editId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                ShowMessage("User updated successfully.");
            }

            pnlModal.Visible = false;
            LoadUsers(); // Refresh grid
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlModal.Visible = false;
        }

        private void ClearModal()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtUsername.Text = "";
            txtDepartment.Text = "";
            txtPassword.Text = "";
            ViewState["EditID"] = null;
        }

        private string GenerateRandomPassword(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";
            Random random = new Random();
            char[] password = new char[length];
            for (int i = 0; i < length; i++)
                password[i] = chars[random.Next(chars.Length)];
            return new string(password);
        }

        private void ShowMessage(string msg)
        {
            ClientScript.RegisterStartupScript(GetType(), "alert", $"alert('{msg}');", true);
        }

        // Optional: leave empty if not used, but can be kept
        protected void gvUsers_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}