using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using UniversityPortal.Data;

namespace UniversityPortal.Student
{
    public partial class Attendance : System.Web.UI.Page
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
                LoadAttendanceData();
            }
        }

        private void LoadAttendanceData()
        {
            int studentId = (int)Session["UserId"];
            List<CourseAttendance> courseAttendanceList = new List<CourseAttendance>();

            try
            {
                // Get all courses the student is enrolled in
                string coursesQuery = @"SELECT e.EnrollmentId, c.CourseId, c.CourseName, c.CourseCode, 
                                       u.FullName AS TeacherName
                                       FROM Enrollments e
                                       INNER JOIN Courses c ON e.CourseId = c.CourseId
                                       LEFT JOIN Users u ON c.TeacherId = u.UserId
                                       WHERE e.StudentId = @StudentId";

                DataTable dtCourses = DbConnection.GetData(coursesQuery, new SqlParameter("@StudentId", studentId));

                foreach (DataRow row in dtCourses.Rows)
                {
                    int enrollmentId = (int)row["EnrollmentId"];
                    CourseAttendance ca = new CourseAttendance
                    {
                        EnrollmentId = enrollmentId,
                        CourseId = (int)row["CourseId"],
                        CourseName = row["CourseName"].ToString(),
                        CourseCode = row["CourseCode"].ToString(),
                        TeacherName = row["TeacherName"] != DBNull.Value ? row["TeacherName"].ToString() : "Not Assigned"
                    };

                    // Get attendance statistics
                    string statsQuery = @"SELECT 
                                        COUNT(*) as TotalClasses,
                                        SUM(CASE WHEN Status = 'Present' THEN 1 ELSE 0 END) as Present,
                                        SUM(CASE WHEN Status = 'Absent' THEN 1 ELSE 0 END) as Absent,
                                        SUM(CASE WHEN Status = 'Late' THEN 1 ELSE 0 END) as Late
                                        FROM Attendance
                                        WHERE EnrollmentId = @EnrollmentId";

                    DataTable dtStats = DbConnection.GetData(statsQuery, new SqlParameter("@EnrollmentId", enrollmentId));

                    if (dtStats.Rows.Count > 0)
                    {
                        DataRow statsRow = dtStats.Rows[0];
                        ca.TotalClasses = statsRow["TotalClasses"] != DBNull.Value ? (int)statsRow["TotalClasses"] : 0;
                        ca.Present = statsRow["Present"] != DBNull.Value ? (int)statsRow["Present"] : 0;
                        ca.Absent = statsRow["Absent"] != DBNull.Value ? (int)statsRow["Absent"] : 0;
                        ca.Late = statsRow["Late"] != DBNull.Value ? (int)statsRow["Late"] : 0;

                        if (ca.TotalClasses > 0)
                        {
                            ca.AttendancePercentage = ((ca.Present + ca.Late) * 100.0m) / ca.TotalClasses;
                        }
                        else
                        {
                            ca.AttendancePercentage = 0;
                        }
                    }

                    // Get attendance details
                    string detailsQuery = @"SELECT AttendanceDate as ClassDate, Status,
                                           DATENAME(WEEKDAY, AttendanceDate) as DayOfWeek
                                           FROM Attendance
                                           WHERE EnrollmentId = @EnrollmentId
                                           ORDER BY AttendanceDate DESC";

                    ca.AttendanceDetails = DbConnection.GetData(detailsQuery, new SqlParameter("@EnrollmentId", enrollmentId));

                    courseAttendanceList.Add(ca);
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