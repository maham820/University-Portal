using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace UniversityPortal.Teacher
{
    public partial class Grades : System.Web.UI.Page
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
            }
        }

        private void LoadCourses()
        {
            int teacherId = (int)Session["UserId"];
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT CourseId, CourseName FROM Courses WHERE TeacherId = @TeacherId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlCourse.DataSource = dt;
                    ddlCourse.DataTextField = "CourseName";
                    ddlCourse.DataValueField = "CourseId";
                    ddlCourse.DataBind();
                    ddlCourse.Items.Insert(0, new ListItem("-- Select Course --", ""));
                }
            }
        }

        protected void ddlCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlCourse.SelectedValue))
            {
                LoadGrades();
                pnlGrades.Visible = true;
            }
            else
            {
                pnlGrades.Visible = false;
            }
        }

        private void LoadGrades()
        {
            int courseId = int.Parse(ddlCourse.SelectedValue);
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT e.EnrollmentId, u.FullName as StudentName,
                                ISNULL(g.Mids, 0) as Mids,
                                ISNULL(g.Internals, 0) as Internals,
                                ISNULL(g.Finals, 0) as Finals,
                                ISNULL(g.Mids + g.Internals + g.Finals, 0) as Total
                                FROM Enrollments e
                                INNER JOIN Users u ON e.StudentId = u.UserId
                                LEFT JOIN Grades g ON e.EnrollmentId = g.EnrollmentId
                                WHERE e.CourseId = @CourseId
                                ORDER BY u.FullName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseId", courseId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvGrades.DataSource = dt;
                    gvGrades.DataBind();
                }
            }
        }

        protected void gvGrades_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Can add row-specific commands here if needed
        }

        protected void btnSaveGrades_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    foreach (GridViewRow row in gvGrades.Rows)
                    {
                        int enrollmentId = int.Parse(gvGrades.DataKeys[row.RowIndex].Value.ToString());
                        TextBox txtMids = (TextBox)row.FindControl("txtMids");
                        TextBox txtInternals = (TextBox)row.FindControl("txtInternals");
                        TextBox txtFinals = (TextBox)row.FindControl("txtFinals");

                        decimal mids = string.IsNullOrEmpty(txtMids.Text) ? 0 : decimal.Parse(txtMids.Text);
                        decimal internals = string.IsNullOrEmpty(txtInternals.Text) ? 0 : decimal.Parse(txtInternals.Text);
                        decimal finals = string.IsNullOrEmpty(txtFinals.Text) ? 0 : decimal.Parse(txtFinals.Text);

                        // Validate ranges
                        if (mids < 0 || mids > 25 || internals < 0 || internals > 25 || finals < 0 || finals > 50)
                        {
                            ShowMessage("Invalid marks entered. Mids and Internals: 0-25, Finals: 0-50", "alert-danger");
                            return;
                        }

                        string query = @"IF NOT EXISTS (SELECT 1 FROM Grades WHERE EnrollmentId = @EnrollmentId)
                                        INSERT INTO Grades (EnrollmentId, Mids, Internals, Finals) VALUES (@EnrollmentId, @Mids, @Internals, @Finals)
                                        ELSE
                                        UPDATE Grades SET Mids = @Mids, Internals = @Internals, Finals = @Finals WHERE EnrollmentId = @EnrollmentId";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId);
                            cmd.Parameters.AddWithValue("@Mids", mids);
                            cmd.Parameters.AddWithValue("@Internals", internals);
                            cmd.Parameters.AddWithValue("@Finals", finals);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                ShowMessage("Grades saved successfully!", "alert-success");
                LoadGrades();
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