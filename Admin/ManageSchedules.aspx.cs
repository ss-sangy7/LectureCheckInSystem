using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LectureCheckInSystem.Admin
{
    public partial class ManageSchedules : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Only admin can access this page
            if (Session["LecturerID"] == null || Session["Role"]?.ToString() != "Admin")
                Response.Redirect("~/Accounts/Login.aspx");

            if (!IsPostBack)
            {
                LoadSchedules();
                LoadLecturers();  // populate dropdown for modal
            }
        }

        private void LoadSchedules()
        {
            string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT s.ScheduleID, s.ScheduleDate, s.CourseCode, s.Room, 
                           FORMAT(s.StartTime, 'hh\\:mm tt') AS StartTime,
                           l.FirstName + ' ' + l.LastName AS LecturerName
                    FROM Schedules s
                    INNER JOIN Lecturers l ON s.LecturerID = l.LecturerID
                    ORDER BY s.ScheduleDate DESC, s.StartTime";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvSchedules.DataSource = dt;
                gvSchedules.DataBind();
            }
        }

        private void LoadLecturers()
        {
            string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT LecturerID, FirstName + ' ' + LastName AS FullName FROM Lecturers ORDER BY FirstName";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlLecturer.DataSource = dt;
                ddlLecturer.DataTextField = "FullName";
                ddlLecturer.DataValueField = "LecturerID";
                ddlLecturer.DataBind();
                ddlLecturer.Items.Insert(0, new ListItem("-- Select Lecturer --", ""));
            }
        }

        protected void gvSchedules_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int scheduleId = Convert.ToInt32(e.CommandArgument);
            string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;

            if (e.CommandName == "DeleteSchedule")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "DELETE FROM Schedules WHERE ScheduleID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", scheduleId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadSchedules();
                ShowMessage("Schedule deleted successfully.");
            }
            else if (e.CommandName == "EditSchedule")
            {
                // Load schedule data into modal for editing
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "SELECT ScheduleDate, CourseCode, Room, StartTime, LecturerID FROM Schedules WHERE ScheduleID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", scheduleId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtDate.Text = Convert.ToDateTime(reader["ScheduleDate"]).ToString("yyyy-MM-dd");
                        txtCourseCode.Text = reader["CourseCode"].ToString();
                        txtCourseName.Text = reader["CourseName"].ToString();
                        txtRoom.Text = reader["Room"].ToString();
                        txtTime.Text = TimeSpan.Parse(reader["StartTime"].ToString()).ToString(@"hh\:mm");
                        ddlLecturer.SelectedValue = reader["LecturerID"].ToString();
                    }
                    reader.Close();
                }
                lblModalTitle.Text = "Edit Schedule";
                ViewState["EditID"] = scheduleId;
                pnlModal.Visible = true;
            }
        }

        protected void btnAddSchedule_Click(object sender, EventArgs e)
        {
            ClearModal();
            lblModalTitle.Text = "Add Schedule";
            pnlModal.Visible = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtCourseCode.Text) ||
                string.IsNullOrWhiteSpace(txtRoom.Text) ||
                string.IsNullOrWhiteSpace(txtDate.Text) ||
                string.IsNullOrWhiteSpace(txtTime.Text) ||
                string.IsNullOrWhiteSpace(ddlLecturer.SelectedValue))
            {
                ShowMessage("All fields are required.");
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;
            DateTime scheduleDate = DateTime.Parse(txtDate.Text);
            TimeSpan startTime = TimeSpan.Parse(txtTime.Text);
            int lecturerId = Convert.ToInt32(ddlLecturer.SelectedValue);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                if (lblModalTitle.Text == "Add Schedule")
                {
                    string query = @"
                        INSERT INTO Schedules (CourseCode, CourseName, Room, ScheduleDate, StartTime, LecturerID)
                        VALUES (@CourseCode, @CourseName, @Room, @ScheduleDate, @StartTime, @LecturerID)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CourseCode", txtCourseCode.Text);
                    cmd.Parameters.AddWithValue("@CourseName", txtCourseName.Text);
                    cmd.Parameters.AddWithValue("@Room", txtRoom.Text);
                    cmd.Parameters.AddWithValue("@ScheduleDate", scheduleDate);
                    cmd.Parameters.AddWithValue("@StartTime", startTime);
                    cmd.Parameters.AddWithValue("@LecturerID", lecturerId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    ShowMessage("Schedule added successfully.");
                }
                else // Edit
                {
                    int editId = (int)ViewState["EditID"];
                    string query = @"
                        UPDATE Schedules 
                        SET CourseCode = @CourseCode, CourseName = @CourseName, Room = @Room, ScheduleDate = @ScheduleDate, 
                            StartTime = @StartTime, LecturerID = @LecturerID
                        WHERE ScheduleID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CourseCode", txtCourseCode.Text);
                    cmd.Parameters.AddWithValue("@CourseName", txtCourseName.Text);
                    cmd.Parameters.AddWithValue("@Room", txtRoom.Text);
                    cmd.Parameters.AddWithValue("@ScheduleDate", scheduleDate);
                    cmd.Parameters.AddWithValue("@StartTime", startTime);
                    cmd.Parameters.AddWithValue("@LecturerID", lecturerId);
                    cmd.Parameters.AddWithValue("@ID", editId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    ShowMessage("Schedule updated successfully.");
                }
            }

            pnlModal.Visible = false;
            LoadSchedules(); // Refresh grid
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlModal.Visible = false;
        }

        private void ClearModal()
        {
            txtDate.Text = "";
            txtCourseName.Text = "";
            txtCourseCode.Text = "";
            txtRoom.Text = "";
            txtTime.Text = "";
            ddlLecturer.SelectedIndex = 0;
            ViewState["EditID"] = null;
        }

        private void ShowMessage(string msg)
        {
            ClientScript.RegisterStartupScript(GetType(), "alert", $"alert('{msg}');", true);
        }
    }
}