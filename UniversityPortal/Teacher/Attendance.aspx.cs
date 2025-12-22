using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace UniversityPortal.Teacher
{
    public partial class Attendance : System.Web.UI.Page
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
                txtClassDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private void LoadCourses()
        {
            int teacherId = (int)Session["UserId"];
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT CourseId, CourseName, CourseCode FROM Courses WHERE TeacherId = @TeacherId";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@TeacherId", teacherId);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlCourse.DataSource = dt;
                ddlCourse.DataTextField = "CourseName";
                ddlCourse.DataValueField = "CourseId";
                ddlCourse.DataBind();
                ddlCourse.Items.Insert(0, new ListItem("-- Select Course --", ""));
            }
        }

        protected void ddlCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlCourse.SelectedValue))
            {
                pnlAttendance.Visible = true;
                LoadStudents();
                LoadAttendanceHistory();
            }
            else
            {
                pnlAttendance.Visible = false;
            }
        }

        private void LoadStudents()
        {
            int courseId = int.Parse(ddlCourse.SelectedValue);
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT e.EnrollmentId, u.FullName AS StudentName, u.Email
                                FROM Enrollments e
                                INNER JOIN Users u ON e.StudentId = u.UserId
                                WHERE e.CourseId = @CourseId
                                ORDER BY u.FullName";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@CourseId", courseId);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvStudents.DataSource = dt;
                gvStudents.DataBind();
            }
        }

        protected void btnSaveAttendance_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime attendanceDate = DateTime.Parse(txtClassDate.Text);

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    foreach (GridViewRow row in gvStudents.Rows)
                    {
                        int enrollmentId = int.Parse(gvStudents.DataKeys[row.RowIndex].Value.ToString());
                        DropDownList ddlStatus = (DropDownList)row.FindControl("ddlStatus");
                        string status = ddlStatus.SelectedValue;

                        // Check if attendance already exists for this date
                        string checkQuery = "SELECT COUNT(*) FROM Attendance WHERE EnrollmentId=@EnrollmentId AND AttendanceDate=@AttendanceDate";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId);
                        checkCmd.Parameters.AddWithValue("@AttendanceDate", attendanceDate);

                        int count = (int)checkCmd.ExecuteScalar();

                        if (count == 0)
                        {
                            string insertQuery = @"INSERT INTO Attendance (EnrollmentId, AttendanceDate, Status) 
                                                  VALUES (@EnrollmentId, @AttendanceDate, @Status)";
                            SqlCommand cmd = new SqlCommand(insertQuery, conn);
                            cmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId);
                            cmd.Parameters.AddWithValue("@AttendanceDate", attendanceDate);
                            cmd.Parameters.AddWithValue("@Status", status);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    ShowMessage("Attendance saved successfully!", "alert-success");
                    LoadAttendanceHistory();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, "alert-danger");
            }
        }

        private void LoadAttendanceHistory()
        {
            int courseId = int.Parse(ddlCourse.SelectedValue);
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT a.AttendanceId, a.AttendanceDate AS ClassDate, u.FullName AS StudentName, a.Status
                                FROM Attendance a
                                INNER JOIN Enrollments e ON a.EnrollmentId = e.EnrollmentId
                                INNER JOIN Users u ON e.StudentId = u.UserId
                                WHERE e.CourseId = @CourseId
                                ORDER BY a.AttendanceDate DESC, u.FullName";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@CourseId", courseId);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvAttendanceHistory.DataSource = dt;
                gvAttendanceHistory.DataBind();
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int attendanceId = int.Parse(btn.CommandArgument);
            
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "DELETE FROM Attendance WHERE AttendanceId = @AttendanceId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AttendanceId", attendanceId);
                    cmd.ExecuteNonQuery();
                    ShowMessage("Attendance record deleted!", "alert-success");
                    LoadAttendanceHistory();
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