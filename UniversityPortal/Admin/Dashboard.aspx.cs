using System;
using System.Data;
using System.Data.SqlClient;
using UniversityPortal.Data;

namespace UniversityPortal.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
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
                lblTotalStudents.Text = DbConnection.GetSingleValue(
                    "SELECT COUNT(*) FROM Users WHERE Role = 'Student'"
                ).ToString();

                lblTotalTeachers.Text = DbConnection.GetSingleValue(
                    "SELECT COUNT(*) FROM Users WHERE Role = 'Teacher'"
                ).ToString();

                lblTotalCourses.Text = DbConnection.GetSingleValue(
                    "SELECT COUNT(*) FROM Courses"
                ).ToString();

                lblTotalEnrollments.Text = DbConnection.GetSingleValue(
                    "SELECT COUNT(*) FROM Enrollments"
                ).ToString();
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
                string query = @"SELECT TOP 10 u.FullName as StudentName, c.CourseName, e.EnrollmentDate 
                                FROM Enrollments e 
                                INNER JOIN Users u ON e.StudentId = u.UserId 
                                INNER JOIN Courses c ON e.CourseId = c.CourseId 
                                ORDER BY e.EnrollmentDate DESC";

                DataTable dt = DbConnection.GetData(query);
                gvRecentEnrollments.DataSource = dt;
                gvRecentEnrollments.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
            }
        }
    }
}