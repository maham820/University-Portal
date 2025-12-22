using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using UniversityPortal.Data;

namespace UniversityPortal.Teacher
{
    public partial class Announcements : System.Web.UI.Page
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
                LoadAnnouncements();
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

        private void LoadAnnouncements()
        {
            int teacherId = (int)Session["UserId"];
            string query = @"SELECT a.AnnouncementId, c.CourseName, a.Title, a.Content, a.CreatedDate
                            FROM Announcements a
                            INNER JOIN Courses c ON a.CourseId = c.CourseId
                            WHERE a.TeacherId = @TeacherId
                            ORDER BY a.CreatedDate DESC";

            DataTable dt = DbConnection.GetData(query, new SqlParameter("@TeacherId", teacherId));
            gvAnnouncements.DataSource = dt;
            gvAnnouncements.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlCourse.SelectedValue))
                {
                    ShowMessage("Please select a course", "alert-danger");
                    return;
                }

                int teacherId = (int)Session["UserId"];
                int courseId = int.Parse(ddlCourse.SelectedValue);
                string query;
                SqlParameter[] parameters;

                if (string.IsNullOrEmpty(hfAnnouncementId.Value))
                {
                    query = @"INSERT INTO Announcements (CourseId, TeacherId, Title, Content) 
                             VALUES (@CourseId, @TeacherId, @Title, @Content)";

                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@CourseId", courseId),
                        new SqlParameter("@TeacherId", teacherId),
                        new SqlParameter("@Title", txtTitle.Text.Trim()),
                        new SqlParameter("@Content", txtContent.Text.Trim())
                    };
                }
                else
                {
                    query = @"UPDATE Announcements SET CourseId=@CourseId, Title=@Title, Content=@Content 
                             WHERE AnnouncementId=@AnnouncementId AND TeacherId=@TeacherId";

                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@CourseId", courseId),
                        new SqlParameter("@TeacherId", teacherId),
                        new SqlParameter("@Title", txtTitle.Text.Trim()),
                        new SqlParameter("@Content", txtContent.Text.Trim()),
                        new SqlParameter("@AnnouncementId", int.Parse(hfAnnouncementId.Value))
                    };
                }

                DbConnection.ExecuteCommand(query, parameters);
                ShowMessage("Announcement posted successfully!", "alert-success");
                ClearForm();
                LoadAnnouncements();
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, "alert-danger");
            }
        }

        protected void gvAnnouncements_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditAnnouncement")
            {
                int announcementId = int.Parse(e.CommandArgument.ToString());
                LoadAnnouncementForEdit(announcementId);
            }
        }

        private void LoadAnnouncementForEdit(int announcementId)
        {
            int teacherId = (int)Session["UserId"];
            string query = "SELECT * FROM Announcements WHERE AnnouncementId = @AnnouncementId AND TeacherId = @TeacherId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@AnnouncementId", announcementId),
                new SqlParameter("@TeacherId", teacherId)
            };

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                SqlDataReader reader = DbConnection.GetReader(query, conn, parameters);
                if (reader.Read())
                {
                    hfAnnouncementId.Value = reader["AnnouncementId"].ToString();
                    ddlCourse.SelectedValue = reader["CourseId"].ToString();
                    txtTitle.Text = reader["Title"].ToString();
                    txtContent.Text = reader["Content"].ToString();
                    lblFormTitle.Text = "Edit Announcement";
                }
            }
        }

        protected void gvAnnouncements_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int announcementId = int.Parse(gvAnnouncements.DataKeys[e.RowIndex].Value.ToString());
            int teacherId = (int)Session["UserId"];

            string query = "DELETE FROM Announcements WHERE AnnouncementId = @AnnouncementId AND TeacherId = @TeacherId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@AnnouncementId", announcementId),
                new SqlParameter("@TeacherId", teacherId)
            };

            DbConnection.ExecuteCommand(query, parameters);
            ShowMessage("Announcement deleted successfully!", "alert-success");
            LoadAnnouncements();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            hfAnnouncementId.Value = "";
            ddlCourse.SelectedIndex = 0;
            txtTitle.Text = "";
            txtContent.Text = "";
            lblFormTitle.Text = "Create New Announcement";
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMessage.Text = message;
            pnlMessage.CssClass = "alert " + cssClass;
            pnlMessage.Visible = true;
        }
    }
}