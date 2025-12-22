using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using UniversityPortal.Data;

namespace UniversityPortal.Admin
{
    public partial class Courses : System.Web.UI.Page
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
                LoadTeachers();
                LoadCourses();
            }
        }

        private void LoadTeachers()
        {
            string query = "SELECT UserId, FullName FROM Users WHERE Role = 'Teacher'";
            DataTable dt = DbConnection.GetData(query);

            ddlTeacher.DataSource = dt;
            ddlTeacher.DataTextField = "FullName";
            ddlTeacher.DataValueField = "UserId";
            ddlTeacher.DataBind();
            ddlTeacher.Items.Insert(0, new ListItem("-- Select Teacher --", ""));
        }

        private void LoadCourses()
        {
            string query = @"SELECT c.CourseId, c.CourseName, c.CourseCode, c.CreditHours, 
                            ISNULL(u.FullName, 'Not Assigned') as TeacherName 
                            FROM Courses c 
                            LEFT JOIN Users u ON c.TeacherId = u.UserId";

            DataTable dt = DbConnection.GetData(query);
            gvCourses.DataSource = dt;
            gvCourses.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string query;

                if (string.IsNullOrEmpty(hfCourseId.Value))
                {
                    query = @"INSERT INTO Courses (CourseName, CourseCode, CreditHours, TeacherId) 
                             VALUES (@CourseName, @CourseCode, @CreditHours, @TeacherId)";
                }
                else
                {
                    query = @"UPDATE Courses SET CourseName=@CourseName, CourseCode=@CourseCode, 
                             CreditHours=@CreditHours, TeacherId=@TeacherId WHERE CourseId=@CourseId";
                }

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@CourseName", txtCourseName.Text),
                    new SqlParameter("@CourseCode", txtCourseCode.Text),
                    new SqlParameter("@CreditHours", int.Parse(txtCreditHours.Text)),
                    new SqlParameter("@TeacherId", string.IsNullOrEmpty(ddlTeacher.SelectedValue) ? (object)DBNull.Value : int.Parse(ddlTeacher.SelectedValue))
                };

                if (!string.IsNullOrEmpty(hfCourseId.Value))
                {
                    Array.Resize(ref parameters, parameters.Length + 1);
                    parameters[parameters.Length - 1] = new SqlParameter("@CourseId", int.Parse(hfCourseId.Value));
                }

                DbConnection.ExecuteCommand(query, parameters);
                ShowMessage("Course saved successfully!", "alert-success");
                ClearForm();
                LoadCourses();
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, "alert-danger");
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int courseId = int.Parse(btn.CommandArgument);
            LoadCourseForEdit(courseId);
        }

        private void LoadCourseForEdit(int courseId)
        {
            string query = "SELECT * FROM Courses WHERE CourseId = @CourseId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CourseId", courseId)
            };

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                SqlDataReader reader = DbConnection.GetReader(query, conn, parameters);
                if (reader.Read())
                {
                    hfCourseId.Value = reader["CourseId"].ToString();
                    txtCourseName.Text = reader["CourseName"].ToString();
                    txtCourseCode.Text = reader["CourseCode"].ToString();
                    txtCreditHours.Text = reader["CreditHours"].ToString();
                    ddlTeacher.SelectedValue = reader["TeacherId"] == DBNull.Value ? "" : reader["TeacherId"].ToString();
                    lblFormTitle.Text = "Edit Course";
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int courseId = int.Parse(btn.CommandArgument);

            string query = "DELETE FROM Courses WHERE CourseId = @CourseId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CourseId", courseId)
            };

            DbConnection.ExecuteCommand(query, parameters);
            ShowMessage("Course deleted successfully!", "alert-success");
            LoadCourses();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            hfCourseId.Value = "";
            txtCourseName.Text = "";
            txtCourseCode.Text = "";
            txtCreditHours.Text = "";
            ddlTeacher.SelectedIndex = 0;
            lblFormTitle.Text = "Add New Course";
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMessage.Text = message;
            pnlMessage.CssClass = "alert " + cssClass;
            pnlMessage.Visible = true;
        }
    }
}