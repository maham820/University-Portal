using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace UniversityPortal.Teacher
{
    public partial class Dashboard : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"]?.ToString() != "Teacher")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadCourses();
                LoadStudents();
            }
        }

        private void LoadCourses()
        {
            int teacherId = (int)Session["UserId"];
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT c.CourseName, c.CourseCode, c.CreditHours,
                                COUNT(e.EnrollmentId) as StudentCount
                                FROM Courses c
                                LEFT JOIN Enrollments e ON c.CourseId = e.CourseId
                                WHERE c.TeacherId = @TeacherId
                                GROUP BY c.CourseName, c.CourseCode, c.CreditHours";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvCourses.DataSource = dt;
                    gvCourses.DataBind();
                }
            }
        }

        private void LoadStudents()
        {
            int teacherId = (int)Session["UserId"];
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT u.FullName as StudentName, u.Email, c.CourseName, e.EnrollmentDate
                                FROM Enrollments e
                                INNER JOIN Users u ON e.StudentId = u.UserId
                                INNER JOIN Courses c ON e.CourseId = c.CourseId
                                WHERE c.TeacherId = @TeacherId
                                ORDER BY c.CourseName, u.FullName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvStudents.DataSource = dt;
                    gvStudents.DataBind();
                }
            }
        }
    }
}