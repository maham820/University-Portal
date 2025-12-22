using System;
using System.Data.SqlClient;
using UniversityPortal.Data;

namespace UniversityPortal
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] != null)
            {
                RedirectBasedOnRole(Session["Role"].ToString());
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter username and password");
                return;
            }

            try
            {
                string query = "SELECT UserId, FullName, Role FROM Users WHERE Username = @Username AND Password = @Password";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Username", username),
                    new SqlParameter("@Password", password)
                };

                using (SqlConnection conn = DbConnection.GetConnection())
                {
                    SqlDataReader reader = DbConnection.GetReader(query, conn, parameters);
                    if (reader.Read())
                    {
                        Session["UserId"] = reader["UserId"];
                        Session["FullName"] = reader["FullName"];
                        Session["Role"] = reader["Role"];

                        RedirectBasedOnRole(reader["Role"].ToString());
                    }
                    else
                    {
                        ShowError("Invalid username or password");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error: " + ex.Message);
            }
        }

        private void RedirectBasedOnRole(string role)
        {
            switch (role)
            {
                case "Admin":
                    Response.Redirect("~/Admin/Dashboard.aspx");
                    break;
                case "Teacher":
                    Response.Redirect("~/Teacher/Dashboard.aspx");
                    break;
                case "Student":
                    Response.Redirect("~/Student/Dashboard.aspx");
                    break;
                default:
                    ShowError("Invalid role");
                    break;
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            pnlError.Visible = true;
        }
    }
}