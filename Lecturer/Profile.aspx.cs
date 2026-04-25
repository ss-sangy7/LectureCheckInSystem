using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using BCryptNet = BCrypt.Net.BCrypt;

namespace LectureCheckInSystem.Lecturer
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Ensure user is logged in
            if (Session["LecturerID"] == null)
                Response.Redirect("~/Accounts/Login.aspx");

            if (!IsPostBack)
                LoadProfile();
        }

        private void LoadProfile()
        {
            int lecturerId = (int)Session["LecturerID"];
            string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT FirstName, LastName, Email, Department FROM Lecturers WHERE LecturerID = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", lecturerId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtFirstName.Text = reader["FirstName"].ToString();
                    txtLastName.Text = reader["LastName"].ToString();
                    txtEmail.Text = reader["Email"].ToString();
                    lblDepartmentStatic.Text = reader["Department"].ToString();
                }
            }
        }
        protected void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblProfileMessage.ForeColor = System.Drawing.Color.Red;
                lblProfileMessage.Text = "All fields are required.";
                return;
            }

            int lecturerId = (int)Session["LecturerID"];
            string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "UPDATE Lecturers SET FirstName = @FirstName, LastName = @LastName, Email = @Email WHERE LecturerID = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@ID", lecturerId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            lblProfileMessage.ForeColor = System.Drawing.Color.Green;
            lblProfileMessage.Text = "Profile updated successfully.";
            // Update session name
            Session["FullName"] = $"{txtFirstName.Text} {txtLastName.Text}";
        }

        protected void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            // Validate new password fields
            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                lblPasswordMessage.ForeColor = System.Drawing.Color.Red;
                lblPasswordMessage.Text = "New password cannot be empty.";
                return;
            }
            if (txtNewPassword.Text.Length < 6)
            {
                lblPasswordMessage.ForeColor = System.Drawing.Color.Red;
                lblPasswordMessage.Text = "New password must be at least 6 characters.";
                return;
            }
            if (txtNewPassword.Text != txtConfirmNewPassword.Text)
            {
                lblPasswordMessage.ForeColor = System.Drawing.Color.Red;
                lblPasswordMessage.Text = "New passwords do not match.";
                return;
            }

            int lecturerId = (int)Session["LecturerID"];
            string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Verify current password
                string query = "SELECT PasswordHash FROM Lecturers WHERE LecturerID = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", lecturerId);
                string storedHash = cmd.ExecuteScalar()?.ToString();

                if (!BCryptNet.Verify(txtCurrentPassword.Text, storedHash))
                {
                    lblPasswordMessage.ForeColor = System.Drawing.Color.Red;
                    lblPasswordMessage.Text = "Current password is incorrect.";
                    return;
                }

                // Update to new password
                string newHash = BCryptNet.HashPassword(txtNewPassword.Text);
                string updateQuery = "UPDATE Lecturers SET PasswordHash = @NewHash WHERE LecturerID = @ID";
                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@NewHash", newHash);
                updateCmd.Parameters.AddWithValue("@ID", lecturerId);
                updateCmd.ExecuteNonQuery();

                lblPasswordMessage.ForeColor = System.Drawing.Color.Green;
                lblPasswordMessage.Text = "Password updated successfully.";
                txtCurrentPassword.Text = txtNewPassword.Text = txtConfirmNewPassword.Text = "";
            }
        }
    }
}