using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LectureCheckInSystem
{
    public partial class LectureSystem : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LecturerID"] != null)
            {
                string role = Session["Role"] as string;
                bool isAdmin = (role == "Admin");

                // Show common links for all logged-in users (visible only for non-admins)
                lnkCheckIn.Visible = !isAdmin;   // only for lecturers
                lnkProfile.Visible = !isAdmin;

                // Admin links (visible only for admins)
                lnkDashboard.Visible = isAdmin;
                lnkManageUsers.Visible = isAdmin;
                lnkManageSchedules.Visible = isAdmin;

                btnLogout.Visible = true;
            }
            else
            {
                // Hide all navigation when not logged in
                lnkCheckIn.Visible = false;
                lnkProfile.Visible = false;
                lnkDashboard.Visible = false;
                lnkManageUsers.Visible = false;
                lnkManageSchedules.Visible = false;
                btnLogout.Visible = false;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Accounts/Login.aspx");
        }
    }
}