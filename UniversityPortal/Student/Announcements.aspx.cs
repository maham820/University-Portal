using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UniversityPortal.Student
{
    public partial class Announcements : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

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
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT w.Message, w.SentDate, 'General' AS CourseName
                                FROM Warnings w
                                WHERE w.StudentId = @StudentId
                                ORDER BY w.SentDate DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@StudentId", studentId);
                DataTable dt = new DataTable();
                da.Fill(dt);

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
        }

        private void LoadAnnouncements()
        {
            int studentId = (int)Session["UserId"];
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT a.Title, a.Content, a.CreatedDate, c.CourseName, u.FullName AS TeacherName
                                FROM Announcements a
                                INNER JOIN Courses c ON a.CourseId = c.CourseId
                                INNER JOIN Users u ON a.TeacherId = u.UserId
                                WHERE c.CourseId IN (SELECT CourseId FROM Enrollments WHERE StudentId = @StudentId)
                                ORDER BY a.CreatedDate DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@StudentId", studentId);
                DataTable dt = new DataTable();
                da.Fill(dt);

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
}