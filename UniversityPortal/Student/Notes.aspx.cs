// CODE BEHIND - Notes.aspx.cs
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace UniversityPortal.Student
{
    public partial class Notes : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

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
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT NoteId, Title, Content, CreatedDate 
                                FROM StudentNotes 
                                WHERE StudentId = @StudentId 
                                ORDER BY CreatedDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvNotes.DataSource = dt;
                    gvNotes.DataBind();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int studentId = (int)Session["UserId"];
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string query;

                    if (string.IsNullOrEmpty(hfNoteId.Value))
                    {
                        // Insert
                        query = "INSERT INTO StudentNotes (StudentId, Title, Content) VALUES (@StudentId, @Title, @Content)";
                    }
                    else
                    {
                        // Update
                        query = "UPDATE StudentNotes SET Title=@Title, Content=@Content WHERE NoteId=@NoteId AND StudentId=@StudentId";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@Content", txtContent.Text.Trim());

                        if (!string.IsNullOrEmpty(hfNoteId.Value))
                            cmd.Parameters.AddWithValue("@NoteId", int.Parse(hfNoteId.Value));

                        cmd.ExecuteNonQuery();
                        ShowMessage("Note saved successfully!", "alert-success");
                        ClearForm();
                        LoadNotes();
                    }
                }
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
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM StudentNotes WHERE NoteId = @NoteId AND StudentId = @StudentId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NoteId", noteId);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        hfNoteId.Value = reader["NoteId"].ToString();
                        txtTitle.Text = reader["Title"].ToString();
                        txtContent.Text = reader["Content"].ToString();
                        lblFormTitle.Text = "Edit Note";
                    }
                }
            }
        }

        protected void gvNotes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int noteId = int.Parse(gvNotes.DataKeys[e.RowIndex].Value.ToString());
            int studentId = (int)Session["UserId"];

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "DELETE FROM StudentNotes WHERE NoteId = @NoteId AND StudentId = @StudentId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NoteId", noteId);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.ExecuteNonQuery();
                    ShowMessage("Note deleted successfully!", "alert-success");
                    LoadNotes();
                }
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