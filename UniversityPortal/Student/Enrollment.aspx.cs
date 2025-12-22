using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using UniversityPortal.Data;

namespace UniversityPortal.Student
{
    public partial class Enrollment : System.Web.UI.Page
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
                LoadAvailableCourses();
                LoadEnrolledCourses();
            }
        }

        private void LoadAvailableCourses()
        {
            int studentId = (int)Session["UserId"];
            string query = @"SELECT c.CourseId, c.CourseName, c.CourseCode, c.CreditHours, 
                            ISNULL(u.FullName, 'Not Assigned') as TeacherName
                            FROM Courses c
                            LEFT JOIN Users u ON c.TeacherId = u.UserId
                            WHERE c.CourseId NOT IN (SELECT CourseId FROM Enrollments WHERE StudentId = @StudentId)";

            DataTable dt = DbConnection.GetData(query, new SqlParameter("@StudentId", studentId));
            gvAvailableCourses.DataSource = dt;
            gvAvailableCourses.DataBind();
        }

        private void LoadEnrolledCourses()
        {
            int studentId = (int)Session["UserId"];
            string query = @"SELECT e.EnrollmentId, c.CourseName, c.CourseCode, c.CreditHours, e.EnrollmentDate
                            FROM Enrollments e
                            INNER JOIN Courses c ON e.CourseId = c.CourseId
                            WHERE e.StudentId = @StudentId";

            DataTable dt = DbConnection.GetData(query, new SqlParameter("@StudentId", studentId));
            gvEnrolledCourses.DataSource = dt;
            gvEnrolledCourses.DataBind();
        }

        protected void gvAvailableCourses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Enroll")
            {
                int courseId = int.Parse(e.CommandArgument.ToString());
                int studentId = (int)Session["UserId"];

                try
                {
                    string query = "INSERT INTO Enrollments (StudentId, CourseId, EnrollmentDate) VALUES (@StudentId, @CourseId, GETDATE())";
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@StudentId", studentId),
                        new SqlParameter("@CourseId", courseId)
                    };

                    DbConnection.ExecuteCommand(query, parameters);
                    ShowMessage("Enrolled successfully!", "alert-success");
                    LoadAvailableCourses();
                    LoadEnrolledCourses();
                }
                catch (Exception ex)
                {
                    ShowMessage("Error: " + ex.Message, "alert-danger");
                }
            }
        }

        protected void gvEnrolledCourses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Drop")
            {
                int enrollmentId = int.Parse(e.CommandArgument.ToString());
                try
                {
                    string query = "DELETE FROM Enrollments WHERE EnrollmentId = @EnrollmentId";
                    DbConnection.ExecuteCommand(query, new SqlParameter("@EnrollmentId", enrollmentId));
                    ShowMessage("Course dropped successfully!", "alert-success");
                    LoadAvailableCourses();
                    LoadEnrolledCourses();
                }
                catch (Exception ex)
                {
                    ShowMessage("Error: " + ex.Message, "alert-danger");
                }
            }
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMessage.Text = message;
            pnlMessage.CssClass = "alert " + cssClass;
            pnlMessage.Visible = true;
        }
    }
}