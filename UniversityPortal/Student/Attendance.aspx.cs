using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;

namespace UniversityPortal.Student
{
    public partial class Attendance : System.Web.UI.Page
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
                LoadAttendanceData();
            }
        }

        private void LoadAttendanceData()
        {
            int studentId = (int)Session["UserId"];
            List<CourseAttendance> courseAttendanceList = new List<CourseAttendance>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Get all courses the student is enrolled in
                    string coursesQuery = @"SELECT e.EnrollmentId, c.CourseId, c.CourseName, c.CourseCode, 
                                           u.FullName AS TeacherName
                                           FROM Enrollments e
                                           INNER JOIN Courses c ON e.CourseId = c.CourseId
                                           LEFT JOIN Users u ON c.TeacherId = u.UserId
                                           WHERE e.StudentId = @StudentId";

                    using (SqlCommand cmd = new SqlCommand(coursesQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            int enrollmentId = (int)reader["EnrollmentId"];
                            CourseAttendance ca = new CourseAttendance
                            {
                                EnrollmentId = enrollmentId,
                                CourseId = (int)reader["CourseId"],
                                CourseName = reader["CourseName"].ToString(),
                                CourseCode = reader["CourseCode"].ToString(),
                                TeacherName = reader["TeacherName"] != DBNull.Value ? reader["TeacherName"].ToString() : "Not Assigned"
                            };

                            courseAttendanceList.Add(ca);
                        }
                        reader.Close();
                    }

                    // For each course, get attendance statistics and details
                    foreach (var ca in courseAttendanceList)
                    {
                        // Get attendance statistics
                        string statsQuery = @"SELECT 
                                            COUNT(*) as TotalClasses,
                                            SUM(CASE WHEN Status = 'Present' THEN 1 ELSE 0 END) as Present,
                                            SUM(CASE WHEN Status = 'Absent' THEN 1 ELSE 0 END) as Absent,
                                            SUM(CASE WHEN Status = 'Late' THEN 1 ELSE 0 END) as Late
                                            FROM Attendance
                                            WHERE EnrollmentId = @EnrollmentId";

                        using (SqlCommand cmd = new SqlCommand(statsQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@EnrollmentId", ca.EnrollmentId);
                            SqlDataReader reader = cmd.ExecuteReader();

                            if (reader.Read())
                            {
                                ca.TotalClasses = reader["TotalClasses"] != DBNull.Value ? (int)reader["TotalClasses"] : 0;
                                ca.Present = reader["Present"] != DBNull.Value ? (int)reader["Present"] : 0;
                                ca.Absent = reader["Absent"] != DBNull.Value ? (int)reader["Absent"] : 0;
                                ca.Late = reader["Late"] != DBNull.Value ? (int)reader["Late"] : 0;

                                // Calculate attendance percentage (Present + Late counted as attended)
                                if (ca.TotalClasses > 0)
                                {
                                    ca.AttendancePercentage = ((ca.Present + ca.Late) * 100.0m) / ca.TotalClasses;
                                }
                                else
                                {
                                    ca.AttendancePercentage = 0;
                                }
                            }
                            reader.Close();
                        }

                        // Get attendance details
                        string detailsQuery = @"SELECT AttendanceDate as ClassDate, Status,
                                               DATENAME(WEEKDAY, AttendanceDate) as DayOfWeek
                                               FROM Attendance
                                               WHERE EnrollmentId = @EnrollmentId
                                               ORDER BY AttendanceDate DESC";

                        using (SqlCommand cmd = new SqlCommand(detailsQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@EnrollmentId", ca.EnrollmentId);
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            ca.AttendanceDetails = dt;
                        }
                    }
                }

                // Calculate overall statistics
                if (courseAttendanceList.Count > 0 && courseAttendanceList.Any(c => c.TotalClasses > 0))
                {
                    int totalClasses = courseAttendanceList.Sum(c => c.TotalClasses);
                    int totalPresent = courseAttendanceList.Sum(c => c.Present);
                    int totalAbsent = courseAttendanceList.Sum(c => c.Absent);
                    int totalLate = courseAttendanceList.Sum(c => c.Late);

                    lblTotalClasses.Text = totalClasses.ToString();
                    lblTotalPresent.Text = totalPresent.ToString();
                    lblTotalAbsent.Text = totalAbsent.ToString();
                    lblTotalLate.Text = totalLate.ToString();

                    if (totalClasses > 0)
                    {
                        decimal overallPercentage = ((totalPresent + totalLate) * 100.0m) / totalClasses;
                        lblOverallPercentage.Text = overallPercentage.ToString("F2");
                    }
                    else
                    {
                        lblOverallPercentage.Text = "0.00";
                    }

                    rptCourseAttendance.DataSource = courseAttendanceList;
                    rptCourseAttendance.DataBind();
                    pnlNoData.Visible = false;
                }
                else
                {
                    pnlNoData.Visible = true;
                    rptCourseAttendance.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading attendance: " + ex.Message, "alert-danger");
            }
        }

        // Helper method to get status color for inline styling
        protected string GetStatusColor(string status)
        {
            switch (status?.ToLower())
            {
                case "present":
                    return "#27ae60";
                case "absent":
                    return "#e74c3c";
                case "late":
                    return "#f39c12";
                default:
                    return "#95a5a6";
            }
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMessage.Text = message;
            pnlMessage.CssClass = "alert " + cssClass;
            pnlMessage.Visible = true;
        }

        // Helper class to store course attendance data
        public class CourseAttendance
        {
            public int EnrollmentId { get; set; }
            public int CourseId { get; set; }
            public string CourseName { get; set; }
            public string CourseCode { get; set; }
            public string TeacherName { get; set; }
            public int TotalClasses { get; set; }
            public int Present { get; set; }
            public int Absent { get; set; }
            public int Late { get; set; }
            public decimal AttendancePercentage { get; set; }
            public DataTable AttendanceDetails { get; set; }
        }
    }
}