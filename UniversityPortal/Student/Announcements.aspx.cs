using System;
using System.Data;
using System.Data.SqlClient;
using UniversityPortal.Data;

namespace UniversityPortal.Student
{
    public partial class Announcements : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"]?.ToString() != "Student")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadWarnings();
                LoadAnnouncements();
            }
        }

        private void LoadWarnings()
        {
            int studentId = (int)Session["UserId"];
            string query = @"SELECT w.Message, w.SentDate, 'General' AS CourseName
                            FROM Warnings w
                            WHERE w.StudentId = @StudentId
                            ORDER BY w.SentDate DESC";

            DataTable dt = DbConnection.GetData(query, new SqlParameter("@StudentId", studentId));

            if (dt.Rows.Count > 0)
            {
                gvWarnings.DataSource = dt;
                gvWarnings.DataBind();
                lblNoWarnings.Visible = false;
            }
            else
            {
                gvWarnings.Visible = false;
                lblNoWarnings.Visible = true;
            }
        }

        private void LoadAnnouncements()
        {
            int studentId = (int)Session["UserId"];
            string query = @"SELECT a.Title, a.Content, a.CreatedDate, c.CourseName, u.FullName AS TeacherName
                            FROM Announcements a
                            INNER JOIN Courses c ON a.CourseId = c.CourseId
                            INNER JOIN Users u ON a.TeacherId = u.UserId
                            WHERE c.CourseId IN (SELECT CourseId FROM Enrollments WHERE StudentId = @StudentId)
                            ORDER BY a.CreatedDate DESC";

            DataTable dt = DbConnection.GetData(query, new SqlParameter("@StudentId", studentId));

            if (dt.Rows.Count > 0)
            {
                rptAnnouncements.DataSource = dt;
                rptAnnouncements.DataBind();
                lblNoAnnouncements.Visible = false;
            }
            else
            {
                rptAnnouncements.Visible = false;
                lblNoAnnouncements.Visible = true;
            }
        }
    }
}