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
            int noteId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "EditNote")
            {
                LoadNoteForEdit(noteId);
            }
            else if (e.CommandName == "DeleteNote")
            {
                int studentId = (int)Session["UserId"];
                string query = "DELETE FROM StudentNotes WHERE NoteId = @NoteId AND StudentId = @StudentId";
                DbConnection.ExecuteCommand(query,
                    new SqlParameter("@NoteId", noteId),
                    new SqlParameter("@StudentId", studentId));

                ShowMessage("Note deleted successfully!", "alert-success");
                LoadNotes();
            }
        }

        private void LoadNoteForEdit(int noteId)
        {
            int studentId = (int)Session["UserId"];
            string query = "SELECT * FROM StudentNotes WHERE NoteId = @NoteId AND StudentId = @StudentId";
            DataTable dt = DbConnection.GetData(query,
                new SqlParameter("@NoteId", noteId),
                new SqlParameter("@StudentId", studentId));

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                hfNoteId.Value = row["NoteId"].ToString();
                txtTitle.Text = row["Title"].ToString();
                txtContent.Text = row["Content"].ToString();
                lblFormTitle.Text = "Edit Note";
            }
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