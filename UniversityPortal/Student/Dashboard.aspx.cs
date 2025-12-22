using System;
using System.Data;
using System.Data.SqlClient;
using UniversityPortal.Data;

namespace UniversityPortal.Student
{
    public partial class Dashboard : System.Web.UI.Page
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
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            int studentId = (int)Session["UserId"];

            // Get enrolled courses count
            lblEnrolledCourses.Text = DbConnection.GetSingleValue(
                "SELECT COUNT(*) FROM Enrollments WHERE StudentId = @StudentId",
                new SqlParameter("@StudentId", studentId)
            ).ToString();

            // Get attendance rate
            string attendanceQuery = @"SELECT 
                     CASE WHEN COUNT(*) = 0 THEN 0 
                     ELSE CAST(SUM(CASE WHEN a.Status = 'Present' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS INT) 
                     END
                     FROM Attendance a
                     INNER JOIN Enrollments e ON a.EnrollmentId = e.EnrollmentId
                     WHERE e.StudentId = @StudentId";

            object result = DbConnection.GetSingleValue(attendanceQuery, new SqlParameter("@StudentId", studentId));
            lblAttendanceRate.Text = result != null ? result.ToString() : "0";

            // Get warnings count
            lblWarnings.Text = DbConnection.GetSingleValue(
                "SELECT COUNT(*) FROM Warnings WHERE StudentId = @StudentId",
                new SqlParameter("@StudentId", studentId)
            ).ToString();

            // Load enrolled courses
            string coursesQuery = @"SELECT c.CourseName, c.CourseCode, c.CreditHours, u.FullName as TeacherName
                     FROM Enrollments e
                     INNER JOIN Courses c ON e.CourseId = c.CourseId
                     LEFT JOIN Users u ON c.TeacherId = u.UserId
                     WHERE e.StudentId = @StudentId";

            DataTable dtCourses = DbConnection.GetData(coursesQuery, new SqlParameter("@StudentId", studentId));
            gvCourses.DataSource = dtCourses;
            gvCourses.DataBind();

            // Load grades with GPA
            string gradesQuery = @"SELECT c.CourseName, 
                     ISNULL(g.Mids, 0) as Mids,
                     ISNULL(g.Internals, 0) as Internals,
                     ISNULL(g.Finals, 0) as Finals,
                     ISNULL(g.Mids + g.Internals + g.Finals, 0) as Total
                     FROM Enrollments e
                     INNER JOIN Courses c ON e.CourseId = c.CourseId
                     LEFT JOIN Grades g ON e.EnrollmentId = g.EnrollmentId
                     WHERE e.StudentId = @StudentId";

            DataTable dtGrades = DbConnection.GetData(gradesQuery, new SqlParameter("@StudentId", studentId));

            // Add GPA and Letter Grade columns
            dtGrades.Columns.Add("Percentage", typeof(decimal));
            dtGrades.Columns.Add("LetterGrade", typeof(string));
            dtGrades.Columns.Add("GPA", typeof(decimal));

            decimal totalGPA = 0;
            int courseCount = 0;

            foreach (DataRow row in dtGrades.Rows)
            {
                decimal total = Convert.ToDecimal(row["Total"]);
                decimal percentage = total; // Total is already out of 100

                row["Percentage"] = Math.Round(percentage, 2);
                row["LetterGrade"] = GetLetterGrade(percentage);
                decimal courseGPA = GetGPA(percentage);
                row["GPA"] = courseGPA;

                // Calculate cumulative GPA
                totalGPA += courseGPA;
                courseCount++;
            }

            gvGrades.DataSource = dtGrades;
            gvGrades.DataBind();

            // Display overall GPA if there are courses
            if (courseCount > 0)
            {
                decimal overallGPA = totalGPA / courseCount;
                // If you have a label for overall GPA, uncomment and use:
                // lblOverallGPA.Text = overallGPA.ToString("F2");
            }
        }

        private string GetLetterGrade(decimal percentage)
        {
            if (percentage == 0) return "NA";
            if (percentage >= 85) return "A";
            if (percentage >= 80) return "A-";
            if (percentage >= 75) return "B+";
            if (percentage >= 70) return "B";
            if (percentage >= 65) return "B-";
            if (percentage >= 61) return "C+";
            if (percentage >= 58) return "C";
            if (percentage >= 55) return "C-";
            if (percentage >= 50) return "D";
            return "F";
        }

        private decimal GetGPA(decimal percentage)
        {
            if (percentage >= 85) return 4.00m;
            if (percentage >= 80) return 3.70m;
            if (percentage >= 75) return 3.30m;
            if (percentage >= 70) return 3.00m;
            if (percentage >= 65) return 2.70m;
            if (percentage >= 61) return 2.30m;
            if (percentage >= 58) return 2.00m;
            if (percentage >= 55) return 1.70m;
            if (percentage >= 50) return 1.00m;
            return 0.00m;
        }
    }
}