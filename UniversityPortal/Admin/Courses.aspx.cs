using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace UniversityPortal.Admin
{
    public partial class Courses : System.Web.UI.Page
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
                LoadTeachers();
                LoadCourses();
            }
        }

        private void LoadTeachers()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT UserId, FullName FROM Users WHERE Role = 'Teacher'";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlTeacher.DataSource = dt;
                ddlTeacher.DataTextField = "FullName";
                ddlTeacher.DataValueField = "UserId";
                ddlTeacher.DataBind();
                ddlTeacher.Items.Insert(0, new ListItem("-- Select Teacher --", ""));
            }
        }

        private void LoadCourses()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT c.CourseId, c.CourseName, c.CourseCode, c.CreditHours, 
                                ISNULL(u.FullName, 'Not Assigned') as TeacherName 
                                FROM Courses c 
                                LEFT JOIN Users u ON c.TeacherId = u.UserId";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvCourses.DataSource = dt;
                gvCourses.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string query;

                    if (string.IsNullOrEmpty(hfCourseId.Value))
                    {
                        // Insert
                        query = @"INSERT INTO Courses (CourseName, CourseCode, CreditHours, TeacherId) 
                                 VALUES (@CourseName, @CourseCode, @CreditHours, @TeacherId)";
                    }
                    else
                    {
                        // Update
                        query = @"UPDATE Courses SET CourseName=@CourseName, CourseCode=@CourseCode, 
                                 CreditHours=@CreditHours, TeacherId=@TeacherId WHERE CourseId=@CourseId";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CourseName", txtCourseName.Text);
                        cmd.Parameters.AddWithValue("@CourseCode", txtCourseCode.Text);
                        cmd.Parameters.AddWithValue("@CreditHours", int.Parse(txtCreditHours.Text));
                        cmd.Parameters.AddWithValue("@TeacherId", string.IsNullOrEmpty(ddlTeacher.SelectedValue) ? (object)DBNull.Value : int.Parse(ddlTeacher.SelectedValue));

                        if (!string.IsNullOrEmpty(hfCourseId.Value))
                            cmd.Parameters.AddWithValue("@CourseId", int.Parse(hfCourseId.Value));

                        cmd.ExecuteNonQuery();
                        ShowMessage("Course saved successfully!", "alert-success");
                        ClearForm();
                        LoadCourses();
                    }
                }
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
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Courses WHERE CourseId = @CourseId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseId", courseId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
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
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int courseId = int.Parse(btn.CommandArgument);
            
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "DELETE FROM Courses WHERE CourseId = @CourseId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseId", courseId);
                    cmd.ExecuteNonQuery();
                    ShowMessage("Course deleted successfully!", "alert-success");
                    LoadCourses();
                }
            }
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