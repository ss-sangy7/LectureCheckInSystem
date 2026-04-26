using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LectureCheckInSystem.Lecturer
{
    public partial class CheckIn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // If not logged in, redirect to login page
            if (Session["LecturerID"] == null)
                Response.Redirect("~/Accounts/Login.aspx");

            if (!IsPostBack)
            {
                lblLecturerName.Text = Session["FullName"].ToString();
                lblDate.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
                LoadSchedule();
            }
        }

        private void LoadSchedule()
        {
            int lecturerId = (int)Session["LecturerID"];
            string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT s.ScheduleID, s.CourseCode, s.Room, 
                           FORMAT(s.StartTime, 'hh\\:mm tt') AS StartTime,
                           CASE WHEN c.CheckInID IS NULL THEN 'Pending' ELSE 'Checked In' END AS Status
                    FROM Schedules s
                    LEFT JOIN CheckIns c ON s.ScheduleID = c.ScheduleID AND c.LecturerID = @LecturerID
                    WHERE s.ScheduleDate = CAST(GETDATE() AS DATE)
                    ORDER BY s.StartTime";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@LecturerID", lecturerId);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvSchedule.DataSource = dt;
                gvSchedule.DataBind();
            }
        }

        protected void gvSchedule_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CheckIn")
            {
                // Get the row index and schedule ID
                int index = Convert.ToInt32(e.CommandArgument);
                int scheduleId = (int)gvSchedule.DataKeys[index].Value;
                int lecturerId = (int)Session["LecturerID"];

                // Insert check-in record
                string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "INSERT INTO CheckIns (ScheduleID, LecturerID) VALUES (@ScheduleID, @LecturerID)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
                    cmd.Parameters.AddWithValue("@LecturerID", lecturerId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                // After check-in, log out immediately
                Session.Clear();
                Session.Abandon();


                string loginUrl = ResolveUrl("~/Accounts/Login.aspx");
                string script = $"alert('Check-in successful! You will now be logged out.'); window.location='{loginUrl}';";
            }
        }

        protected void gvSchedule_SelectedIndexChanged2(object sender, EventArgs e)
        {

        }
    }
}