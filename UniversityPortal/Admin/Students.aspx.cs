using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using UniversityPortal.Data;

namespace UniversityPortal.Admin
{
    public partial class Students : System.Web.UI.Page
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
                LoadStudents();
            }
        }

        private void LoadStudents()
        {
            string query = "SELECT UserId, FullName, Username, Email, CreatedDate FROM Users WHERE Role = 'Student' ORDER BY FullName";
            DataTable dt = DbConnection.GetData(query);
            gvStudents.DataSource = dt;
            gvStudents.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string query;
                SqlParameter[] parameters;

                if (string.IsNullOrEmpty(hfUserId.Value))
                {
                    query = @"INSERT INTO Users (Username, Password, FullName, Email, Role) 
                             VALUES (@Username, @Password, @FullName, @Email, 'Student')";

                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@FullName", txtFullName.Text.Trim()),
                        new SqlParameter("@Username", txtUsername.Text.Trim()),
                        new SqlParameter("@Email", txtEmail.Text.Trim()),
                        new SqlParameter("@Password", txtPassword.Text)
                    };
                }
                else
                {
                    if (string.IsNullOrEmpty(txtPassword.Text))
                    {
                        query = @"UPDATE Users SET FullName=@FullName, Username=@Username, Email=@Email 
                                 WHERE UserId=@UserId";

                        parameters = new SqlParameter[]
                        {
                            new SqlParameter("@FullName", txtFullName.Text.Trim()),
                            new SqlParameter("@Username", txtUsername.Text.Trim()),
                            new SqlParameter("@Email", txtEmail.Text.Trim()),
                            new SqlParameter("@UserId", int.Parse(hfUserId.Value))
                        };
                    }
                    else
                    {
                        query = @"UPDATE Users SET FullName=@FullName, Username=@Username, 
                                 Password=@Password, Email=@Email WHERE UserId=@UserId";

                        parameters = new SqlParameter[]
                        {
                            new SqlParameter("@FullName", txtFullName.Text.Trim()),
                            new SqlParameter("@Username", txtUsername.Text.Trim()),
                            new SqlParameter("@Email", txtEmail.Text.Trim()),
                            new SqlParameter("@Password", txtPassword.Text),
                            new SqlParameter("@UserId", int.Parse(hfUserId.Value))
                        };
                    }
                }

                DbConnection.ExecuteCommand(query, parameters);
                ShowMessage("Student saved successfully!", "alert-success");
                ClearForm();
                LoadStudents();
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
            string query = "SELECT * FROM Users WHERE UserId = @UserId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                SqlDataReader reader = DbConnection.GetReader(query, conn, parameters);
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

        protected void gvStudents_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int userId = int.Parse(gvStudents.DataKeys[e.RowIndex].Value.ToString());

            string query = "DELETE FROM Users WHERE UserId = @UserId";
            DbConnection.ExecuteCommand(query, new SqlParameter("@UserId", userId));
            ShowMessage("Student deleted successfully!", "alert-success");
            LoadStudents();
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