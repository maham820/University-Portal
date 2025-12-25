using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using UniversityPortal.Data;

namespace UniversityPortal.Admin
{
    public partial class SendWarning : System.Web.UI.Page
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
                LoadAttendanceData();
            }
        }

        private void LoadAttendanceData()
        {
            string query = @"
                SELECT 
                    e.StudentId,
                    e.EnrollmentId,
                    u.FullName as StudentName,
                    c.CourseName,
                    COUNT(a.AttendanceId) as TotalClasses,
                    SUM(CASE WHEN a.Status = 'Present' THEN 1 ELSE 0 END) as Present,
                    SUM(CASE WHEN a.Status = 'Absent' THEN 1 ELSE 0 END) as Absent,
                    SUM(CASE WHEN a.Status = 'Late' THEN 1 ELSE 0 END) as Late,
                    CASE 
                        WHEN COUNT(a.AttendanceId) > 0 
                        THEN (CAST(SUM(CASE WHEN a.Status = 'Present' THEN 1 ELSE 0 END) AS FLOAT) / COUNT(a.AttendanceId)) * 100
                        ELSE 0 
                    END as AttendancePercentage
                FROM Enrollments e
                INNER JOIN Users u ON e.StudentId = u.UserId
                INNER JOIN Courses c ON e.CourseId = c.CourseId
                LEFT JOIN Attendance a ON e.EnrollmentId = a.EnrollmentId
                GROUP BY e.StudentId, e.EnrollmentId, u.FullName, c.CourseName
                HAVING COUNT(a.AttendanceId) > 0";

            DataTable dt = DbConnection.GetData(query);

            // Filter for low attendance (changed from 60 to 50)
            DataView dv = dt.DefaultView;
            dv.RowFilter = "AttendancePercentage < 50";
            gvLowAttendance.DataSource = dv;
            gvLowAttendance.DataBind();

            // All attendance
            gvAllAttendance.DataSource = dt;
            gvAllAttendance.DataBind();
        }

        protected void btnSendWarning_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string[] args = btn.CommandArgument.Split(',');
            int studentId = int.Parse(args[0]);
            string courseName = args[1];
            string percentage = args[2];

            try
            {
                // Count existing warnings for this student
                string countQuery = "SELECT COUNT(*) FROM Warnings WHERE StudentId = @StudentId";
                int warningCount = (int)DbConnection.GetSingleValue(countQuery, new SqlParameter("@StudentId", studentId));

                string message;

                if (warningCount == 0)
                {
                    // 1st Warning
                    message = $"⚠️ WARNING #1: Your attendance in {courseName} is critically low at {percentage}%. You must improve your attendance immediately.";
                }
                else if (warningCount == 1)
                {
                    // 2nd Warning
                    message = $"⚠️ WARNING #2: Your attendance in {courseName} is critically low at {percentage}%. This is your second warning. Please improve your attendance.";
                }
                else if (warningCount == 2)
                {
                    // 3rd Warning - FINAL WARNING
                    message = $"🚨 FINAL WARNING #3: Your attendance in {courseName} is critically low at {percentage}%. This is your FINAL WARNING. One more warning and you will be struck off from the course.";
                }
                else if (warningCount >= 3)
                {
                    // 4th Warning - STRUCK OFF
                    message = $"🚨 STRUCK OFF NOTICE: You have been removed from {courseName} due to critically low attendance ({percentage}%) after receiving 3 warnings. Contact administration for more information.";
                    
                    // Insert the struck off warning first
                    string warningQuery = "INSERT INTO Warnings (StudentId, Message) VALUES (@StudentId, @Message)";
                    DbConnection.ExecuteCommand(warningQuery,
                        new SqlParameter("@StudentId", studentId),
                        new SqlParameter("@Message", message));

                    // Get enrollment ID and strike off student
                    string getEnrollmentQuery = @"SELECT e.EnrollmentId 
                                                 FROM Enrollments e 
                                                 INNER JOIN Courses c ON e.CourseId = c.CourseId 
                                                 WHERE e.StudentId = @StudentId AND c.CourseName = @CourseName";
                    
                    object enrollmentIdObj = DbConnection.GetSingleValue(getEnrollmentQuery, 
                        new SqlParameter("@StudentId", studentId),
                        new SqlParameter("@CourseName", courseName));

                    if (enrollmentIdObj != null)
                    {
                        int enrollmentId = Convert.ToInt32(enrollmentIdObj);
                        
                        // Delete the enrollment (CASCADE will handle Attendance and Grades)
                        string deleteQuery = "DELETE FROM Enrollments WHERE EnrollmentId = @EnrollmentId";
                        DbConnection.ExecuteCommand(deleteQuery, new SqlParameter("@EnrollmentId", enrollmentId));

                        ShowMessage($"4th warning sent! Student struck off from {courseName}!", "alert-danger");
                        LoadAttendanceData();
                        return;
                    }
                }
                else
                {
                    message = $"⚠️ ATTENDANCE WARNING: Your attendance in {courseName} is critically low at {percentage}%. You must improve your attendance or you will get struck off.";
                }

                // Insert warning for warnings 1-3
                string query = "INSERT INTO Warnings (StudentId, Message) VALUES (@StudentId, @Message)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@StudentId", studentId),
                    new SqlParameter("@Message", message)
                };

                DbConnection.ExecuteCommand(query, parameters);
                
                if (warningCount == 2)
                {
                    ShowMessage($"Final warning (3/3) sent successfully! Next warning will result in strike-off.", "alert-warning");
                }
                else
                {
                    ShowMessage($"Warning #{warningCount + 1} sent successfully!", "alert-success");
                }
                
                LoadAttendanceData();
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, "alert-danger");
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