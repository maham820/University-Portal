// CODE BEHIND - Notes.aspx.cs
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using UniversityPortal.Data;

namespace UniversityPortal.Student
{
    public partial class Notes : System.Web.UI.Page
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
                LoadNotes();
            }
        }

        private void LoadNotes()
        {
            int studentId = (int)Session["UserId"];
            string query = @"SELECT NoteId, Title, Content, CreatedDate 
                            FROM StudentNotes 
                            WHERE StudentId = @StudentId 
                            ORDER BY CreatedDate DESC";

            DataTable dt = DbConnection.GetData(query, new SqlParameter("@StudentId", studentId));
            gvNotes.DataSource = dt;
            gvNotes.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int studentId = (int)Session["UserId"];
                string query;
                SqlParameter[] parameters;

                if (string.IsNullOrEmpty(hfNoteId.Value))
                {
                    query = "INSERT INTO StudentNotes (StudentId, Title, Content) VALUES (@StudentId, @Title, @Content)";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@StudentId", studentId),
                        new SqlParameter("@Title", txtTitle.Text.Trim()),
                        new SqlParameter("@Content", txtContent.Text.Trim())
                    };
                }
                else
                {
                    query = "UPDATE StudentNotes SET Title=@Title, Content=@Content WHERE NoteId=@NoteId AND StudentId=@StudentId";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@StudentId", studentId),
                        new SqlParameter("@Title", txtTitle.Text.Trim()),
                        new SqlParameter("@Content", txtContent.Text.Trim()),
                        new SqlParameter("@NoteId", int.Parse(hfNoteId.Value))
                    };
                }

                DbConnection.ExecuteCommand(query, parameters);
                ShowMessage("Note saved successfully!", "alert-success");
                ClearForm();
                LoadNotes();
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, "alert-danger");
            }
        }

        protected void gvNotes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditNote")
            {
                int noteId = int.Parse(e.CommandArgument.ToString());
                LoadNoteForEdit(noteId);
            }
        }

        private void LoadNoteForEdit(int noteId)
        {
            int studentId = (int)Session["UserId"];
            string query = "SELECT * FROM StudentNotes WHERE NoteId = @NoteId AND StudentId = @StudentId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@NoteId", noteId),
                new SqlParameter("@StudentId", studentId)
            };

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                SqlDataReader reader = DbConnection.GetReader(query, conn, parameters);
                if (reader.Read())
                {
                    hfNoteId.Value = reader["NoteId"].ToString();
                    txtTitle.Text = reader["Title"].ToString();
                    txtContent.Text = reader["Content"].ToString();
                    lblFormTitle.Text = "Edit Note";
                }
            }
        }

        protected void gvNotes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int noteId = int.Parse(gvNotes.DataKeys[e.RowIndex].Value.ToString());
            int studentId = (int)Session["UserId"];

            string query = "DELETE FROM StudentNotes WHERE NoteId = @NoteId AND StudentId = @StudentId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@NoteId", noteId),
                new SqlParameter("@StudentId", studentId)
            };

            DbConnection.ExecuteCommand(query, parameters);
            ShowMessage("Note deleted successfully!", "alert-success");
            LoadNotes();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            hfNoteId.Value = "";
            txtTitle.Text = "";
            txtContent.Text = "";
            lblFormTitle.Text = "Add New Note";
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMessage.Text = message;
            pnlMessage.CssClass = "alert " + cssClass;
            pnlMessage.Visible = true;
        }
    }
}