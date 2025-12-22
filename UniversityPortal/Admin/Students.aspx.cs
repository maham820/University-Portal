using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace UniversityPortal.Admin
{
    public partial class Students : System.Web.UI.Page
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
                LoadStudents();
            }
        }

        private void LoadStudents()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT UserId, FullName, Username, Email, CreatedDate FROM Users WHERE Role = 'Student' ORDER BY FullName";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvStudents.DataSource = dt;
                gvStudents.DataBind();
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

                    if (string.IsNullOrEmpty(hfUserId.Value))
                    {
                        // Insert
                        query = @"INSERT INTO Users (Username, Password, FullName, Email, Role) 
                                 VALUES (@Username, @Password, @FullName, @Email, 'Student')";
                    }
                    else
                    {
                        // Update
                        if (string.IsNullOrEmpty(txtPassword.Text))
                        {
                            query = @"UPDATE Users SET FullName=@FullName, Username=@Username, Email=@Email 
                                     WHERE UserId=@UserId";
                        }
                        else
                        {
                            query = @"UPDATE Users SET FullName=@FullName, Username=@Username, 
                                     Password=@Password, Email=@Email WHERE UserId=@UserId";
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());

                        if (!string.IsNullOrEmpty(txtPassword.Text))
                            cmd.Parameters.AddWithValue("@Password", txtPassword.Text);

                        if (!string.IsNullOrEmpty(hfUserId.Value))
                            cmd.Parameters.AddWithValue("@UserId", int.Parse(hfUserId.Value));

                        cmd.ExecuteNonQuery();
                        ShowMessage("Student saved successfully!", "alert-success");
                        ClearForm();
                        LoadStudents();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, "alert-danger");
            }
        }

        protected void gvStudents_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditStudent")
            {
                int userId = int.Parse(e.CommandArgument.ToString());
                LoadStudentForEdit(userId);
            }
        }

        private void LoadStudentForEdit(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Users WHERE UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        hfUserId.Value = reader["UserId"].ToString();
                        txtFullName.Text = reader["FullName"].ToString();
                        txtUsername.Text = reader["Username"].ToString();
                        txtEmail.Text = reader["Email"].ToString();
                        txtPassword.Text = "";
                        lblFormTitle.Text = "Edit Student";
                    }
                }
            }
        }

        protected void gvStudents_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int userId = int.Parse(gvStudents.DataKeys[e.RowIndex].Value.ToString());
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "DELETE FROM Users WHERE UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                    ShowMessage("Student deleted successfully!", "alert-success");
                    LoadStudents();
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            hfUserId.Value = "";
            txtFullName.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtEmail.Text = "";
            lblFormTitle.Text = "Add New Student";
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMessage.Text = message;
            pnlMessage.CssClass = "alert " + cssClass;
            pnlMessage.Visible = true;
        }
    }
}