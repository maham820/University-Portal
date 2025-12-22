using System;
using System.Data;
using System.Data.SqlClient;
using UniversityPortal.Data;

namespace UniversityPortal.Teacher
{
    public partial class Dashboard : System.Web.UI.Page
    {
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
            string query = @"SELECT c.CourseName, c.CourseCode, c.CreditHours,
                            COUNT(e.EnrollmentId) as StudentCount
                            FROM Courses c
                            LEFT JOIN Enrollments e ON c.CourseId = e.CourseId
                            WHERE c.TeacherId = @TeacherId
                            GROUP BY c.CourseName, c.CourseCode, c.CreditHours";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TeacherId", teacherId)
            };

            DataTable dt = DbConnection.GetData(query, new SqlParameter("@TeacherId", teacherId));
            gvCourses.DataSource = dt;
            gvCourses.DataBind();
        }

        private void LoadStudents()
        {
            int teacherId = (int)Session["UserId"];
            string query = @"SELECT u.FullName as StudentName, u.Email, c.CourseName, e.EnrollmentDate
                            FROM Enrollments e
                            INNER JOIN Users u ON e.StudentId = u.UserId
                            INNER JOIN Courses c ON e.CourseId = c.CourseId
                            WHERE c.TeacherId = @TeacherId
                            ORDER BY c.CourseName, u.FullName";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TeacherId", teacherId)
            };

            DataTable dt = DbConnection.GetData(query, new SqlParameter("@TeacherId", teacherId));
            gvStudents.DataSource = dt;
            gvStudents.DataBind();
        }
    }
}