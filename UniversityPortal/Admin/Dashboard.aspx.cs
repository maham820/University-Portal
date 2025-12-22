using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace UniversityPortal.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"]?.ToString() != "Admin")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadStatistics();
                LoadRecentEnrollments();
            }
        }

        private void LoadStatistics()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM Users WHERE Role = 'Student'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        lblTotalStudents.Text = cmd.ExecuteScalar().ToString();
                    }

                    query = "SELECT COUNT(*) FROM Users WHERE Role = 'Teacher'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        lblTotalTeachers.Text = cmd.ExecuteScalar().ToString();
                    }

                    query = "SELECT COUNT(*) FROM Courses";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        lblTotalCourses.Text = cmd.ExecuteScalar().ToString();
                    }

                    query = "SELECT COUNT(*) FROM Enrollments";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        lblTotalEnrollments.Text = cmd.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
            }
        }

        private void LoadRecentEnrollments()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = @"SELECT TOP 10 u.FullName as StudentName, c.CourseName, e.EnrollmentDate 
                                    FROM Enrollments e 
                                    INNER JOIN Users u ON e.StudentId = u.UserId 
                                    INNER JOIN Courses c ON e.CourseId = c.CourseId 
                                    ORDER BY e.EnrollmentDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        gvRecentEnrollments.DataSource = dt;
                        gvRecentEnrollments.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
            }
        }
    }
}