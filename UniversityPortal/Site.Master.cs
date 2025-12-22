using System;
using System.Web.UI;

namespace UniversityPortal
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                if (!Request.Url.AbsolutePath.Contains("Login.aspx"))
                {
                    Response.Redirect("~/Login.aspx");
                }
                pnlSidebar.Visible = false;
                return;
            }

            lblUserName.Text = Session["FullName"]?.ToString() ?? "";
            string role = Session["Role"]?.ToString() ?? "";

            // Show appropriate menu based on role
            phAdminMenu.Visible = role == "Admin";
            phTeacherMenu.Visible = role == "Teacher";
            phStudentMenu.Visible = role == "Student";
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}