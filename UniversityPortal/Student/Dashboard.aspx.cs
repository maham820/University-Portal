using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace UniversityPortal.Student
{
    public partial class Dashboard : System.Web.UI.Page
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
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            int studentId = (int)Session["UserId"];

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Get enrolled courses count
                string query = "SELECT COUNT(*) FROM Enrollments WHERE StudentId = @StudentId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    lblEnrolledCourses.Text = cmd.ExecuteScalar().ToString();
                }

                // Get attendance rate
                query = @"SELECT 
                         CASE WHEN COUNT(*) = 0 THEN 0 
                         ELSE CAST(SUM(CASE WHEN a.Status = 'Present' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS INT) 
                         END
                         FROM Attendance a
                         INNER JOIN Enrollments e ON a.EnrollmentId = e.EnrollmentId
                         WHERE e.StudentId = @StudentId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    object result = cmd.ExecuteScalar();
                    lblAttendanceRate.Text = result != null ? result.ToString() : "0";
                }

                // Get warnings count
                query = "SELECT COUNT(*) FROM Warnings WHERE StudentId = @StudentId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    lblWarnings.Text = cmd.ExecuteScalar().ToString();
                }

                // Load enrolled courses
                query = @"SELECT c.CourseName, c.CourseCode, c.CreditHours, u.FullName as TeacherName
                         FROM Enrollments e
                         INNER JOIN Courses c ON e.CourseId = c.CourseId
                         LEFT JOIN Users u ON c.TeacherId = u.UserId
                         WHERE e.StudentId = @StudentId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvCourses.DataSource = dt;
                    gvCourses.DataBind();
                }

                // Load grades
                query = @"SELECT c.CourseName, 
                         ISNULL(g.Mids, 0) as Mids,
                         ISNULL(g.Internals, 0) as Internals,
                         ISNULL(g.Finals, 0) as Finals,
                         ISNULL(g.Mids + g.Internals + g.Finals, 0) as Total
                         FROM Enrollments e
                         INNER JOIN Courses c ON e.CourseId = c.CourseId
                         LEFT JOIN Grades g ON e.EnrollmentId = g.EnrollmentId
                         WHERE e.StudentId = @StudentId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvGrades.DataSource = dt;
                    gvGrades.DataBind();
                }
            }
        }
    }
}