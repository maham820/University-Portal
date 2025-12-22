using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using UniversityPortal.Data;

namespace UniversityPortal.Teacher
{
    public partial class Attendance : System.Web.UI.Page
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
                txtClassDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private void LoadCourses()
        {
            int teacherId = (int)Session["UserId"];
            string query = "SELECT CourseId, CourseName, CourseCode FROM Courses WHERE TeacherId = @TeacherId";

            DataTable dt = DbConnection.GetData(query, new SqlParameter("@TeacherId", teacherId));

            ddlCourse.DataSource = dt;
            ddlCourse.DataTextField = "CourseName";
            ddlCourse.DataValueField = "CourseId";
            ddlCourse.DataBind();
            ddlCourse.Items.Insert(0, new ListItem("-- Select Course --", ""));
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
            string query = @"SELECT e.EnrollmentId, u.FullName AS StudentName, u.Email
                            FROM Enrollments e
                            INNER JOIN Users u ON e.StudentId = u.UserId
                            WHERE e.CourseId = @CourseId
                            ORDER BY u.FullName";

            DataTable dt = DbConnection.GetData(query, new SqlParameter("@CourseId", courseId));
            gvStudents.DataSource = dt;
            gvStudents.DataBind();
        }

        protected void btnSaveAttendance_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime attendanceDate = DateTime.Parse(txtClassDate.Text);

                using (SqlConnection conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    foreach (GridViewRow row in gvStudents.Rows)
                    {
                        int enrollmentId = int.Parse(gvStudents.DataKeys[row.RowIndex].Value.ToString());
                        DropDownList ddlStatus = (DropDownList)row.FindControl("ddlStatus");
                        string status = ddlStatus.SelectedValue;

                        string checkQuery = "SELECT COUNT(*) FROM Attendance WHERE EnrollmentId=@EnrollmentId AND AttendanceDate=@AttendanceDate";
                        int count = (int)DbConnection.GetSingleValue(checkQuery,
                            new SqlParameter("@EnrollmentId", enrollmentId),
                            new SqlParameter("@AttendanceDate", attendanceDate));

                        if (count == 0)
                        {
                            string insertQuery = @"INSERT INTO Attendance (EnrollmentId, AttendanceDate, Status) 
                                                  VALUES (@EnrollmentId, @AttendanceDate, @Status)";

                            DbConnection.ExecuteCommand(insertQuery,
                                new SqlParameter("@EnrollmentId", enrollmentId),
                                new SqlParameter("@AttendanceDate", attendanceDate),
                                new SqlParameter("@Status", status));
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
            string query = @"SELECT a.AttendanceId, a.AttendanceDate AS ClassDate, u.FullName AS StudentName, a.Status
                            FROM Attendance a
                            INNER JOIN Enrollments e ON a.EnrollmentId = e.EnrollmentId
                            INNER JOIN Users u ON e.StudentId = u.UserId
                            WHERE e.CourseId = @CourseId
                            ORDER BY a.AttendanceDate DESC, u.FullName";

            DataTable dt = DbConnection.GetData(query, new SqlParameter("@CourseId", courseId));
            gvAttendanceHistory.DataSource = dt;
            gvAttendanceHistory.DataBind();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int attendanceId = int.Parse(btn.CommandArgument);

            string query = "DELETE FROM Attendance WHERE AttendanceId = @AttendanceId";
            DbConnection.ExecuteCommand(query, new SqlParameter("@AttendanceId", attendanceId));
            ShowMessage("Attendance record deleted!", "alert-success");
            LoadAttendanceHistory();
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMessage.Text = message;
            pnlMessage.CssClass = "alert " + cssClass;
            pnlMessage.Visible = true;
        }
    }
}