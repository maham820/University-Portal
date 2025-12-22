// CODE BEHIND - SendWarning.aspx.cs
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace UniversityPortal.Admin
{
    public partial class SendWarning : System.Web.UI.Page
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
                LoadAttendanceData();
            }
        }

        private void LoadAttendanceData()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT 
                        e.StudentId,
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
                    GROUP BY e.StudentId, u.FullName, c.CourseName
                    HAVING COUNT(a.AttendanceId) > 0";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Filter for low attendance
                DataView dv = dt.DefaultView;
                dv.RowFilter = "AttendancePercentage < 60";
                gvLowAttendance.DataSource = dv;
                gvLowAttendance.DataBind();

                // All attendance
                gvAllAttendance.DataSource = dt;
                gvAllAttendance.DataBind();
            }
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
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string message = $"⚠️ ATTENDANCE WARNING: Your attendance in {courseName} is critically low at {percentage}%. You must improve your attendance to avoid academic consequences.";

                    string query = "INSERT INTO Warnings (StudentId, Message) VALUES (@StudentId, @Message)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        cmd.Parameters.AddWithValue("@Message", message);
                        cmd.ExecuteNonQuery();
                    }
                }

                ShowMessage("Warning sent successfully!", "alert-success");
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