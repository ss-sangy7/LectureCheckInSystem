using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LectureCheckInSystem.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Only admin can access this page
            if (Session["LecturerID"] == null || Session["Role"]?.ToString() != "Admin")
                Response.Redirect("~/Accounts/Login.aspx");

            if (!IsPostBack)
                LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            string connStr = ConfigurationManager.ConnectionStrings["LecturerCheckInDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                { //lets add db values to the dashboard stats (for now we will use dummy values)
                    // ---- 1. Total Lecturers ----
                    string sqlTotalLecturers = "SELECT COUNT(*) FROM Lecturers";
                    SqlCommand cmdTotal = new SqlCommand(sqlTotalLecturers, conn);
                    int totalLecturers = (int)cmdTotal.ExecuteScalar();
                    lblTotalLecturers.Text = totalLecturers.ToString();

                    // ---- 2. Today's Schedules ----
                    string sqlTodaySchedules = @"
                    SELECT COUNT(*) FROM Schedules 
                    WHERE ScheduleDate = CAST(GETDATE() AS DATE)";
                    SqlCommand cmdTodaySched = new SqlCommand(sqlTodaySchedules, conn);
                    int todaySchedules = (int)cmdTodaySched.ExecuteScalar();
                    lblTodaySchedules.Text = todaySchedules.ToString();

                    // ---- 3. Completed Check-ins Today ----
                    string sqlCompleted = @"
                    SELECT COUNT(DISTINCT c.CheckInID)
                    FROM CheckIns c
                    INNER JOIN Schedules s ON c.ScheduleID = s.ScheduleID
                    WHERE s.ScheduleDate = CAST(GETDATE() AS DATE)";
                    SqlCommand cmdCompleted = new SqlCommand(sqlCompleted, conn);
                    int completedCheckins = (int)cmdCompleted.ExecuteScalar();
                    lblCheckinsCompleted.Text = completedCheckins.ToString();

                    // ---- 4. Pending Check-ins Today ----
                    // Schedules today that do NOT have a matching check-in
                    string sqlPending = @"
                    SELECT COUNT(*)
                    FROM Schedules s
                    WHERE s.ScheduleDate = CAST(GETDATE() AS DATE)
                      AND NOT EXISTS (
                          SELECT 1 FROM CheckIns c 
                          WHERE c.ScheduleID = s.ScheduleID
                      )";
                    SqlCommand cmdPending = new SqlCommand(sqlPending, conn);
                    int pendingCheckins = (int)cmdPending.ExecuteScalar();
                    lblPendingCheckins.Text = pendingCheckins.ToString();

                    // ---- 5. Trends (Last 7 days) ----
                    // For each of the last 7 days, count how many check-ins occurred that day
                    DataTable trends = new DataTable();
                    trends.Columns.Add("Day", typeof(string));
                    trends.Columns.Add("Count", typeof(int));
                    trends.Columns.Add("Percent", typeof(int));

                    for (int i = 6; i >= 0; i--)  // last 7 days from today backwards

                    {
                        DateTime targetDate = DateTime.Today.AddDays(-i);
                        string dayName = targetDate.ToString("ddd"); // e.g., "Mon"

                        string sqlDailyCount = @"
                        SELECT COUNT(*)
                        FROM CheckIns c
                        INNER JOIN Schedules s ON c.ScheduleID = s.ScheduleID
                        WHERE s.ScheduleDate = @date";
                        SqlCommand cmdDaily = new SqlCommand(sqlDailyCount, conn);
                        cmdDaily.Parameters.AddWithValue("@date", targetDate);
                        int count = (int)cmdDaily.ExecuteScalar();

                        // Calculate a relative bar width (max of any day = 100%)
                        // We'll first collect all counts, but for simplicity we'll calculate after loop.
                        // We'll store counts in a list and compute max later.
                        trends.Rows.Add(dayName, count, 0);
                    }

                    // Compute max count for percentage scaling
                    int maxCount = 0;
                    foreach (DataRow row in trends.Rows)
                        maxCount = Math.Max(maxCount, Convert.ToInt32(row["Count"]));

                    foreach (DataRow row in trends.Rows)
                    {
                        int count = Convert.ToInt32(row["Count"]);
                        int percent = maxCount > 0 ? (count * 100 / maxCount) : 0;
                        row["Percent"] = percent;
                    }

                    rptTrends.DataSource = trends;
                    rptTrends.DataBind();

                    // ---- 6. Recent Check-in Records (last 5) ----
                    string sqlRecent = @"
                    SELECT TOP 5 
                        l.FirstName + ' ' + l.LastName AS LecturerName,
                        FORMAT(c.CheckInTime, 'hh:mm tt') AS Time,
                        'Checked In' AS Status
                    FROM CheckIns c
                    INNER JOIN Lecturers l ON c.LecturerID = l.LecturerID
                    ORDER BY c.CheckInTime DESC";
                    SqlDataAdapter daRecent = new SqlDataAdapter(sqlRecent, conn);
                    DataTable recent = new DataTable();
                    daRecent.Fill(recent);
                    gvRecentCheckins.DataSource = recent;
                    gvRecentCheckins.DataBind();
                }
            }
        }

        protected void rptTrends_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }
    }
}