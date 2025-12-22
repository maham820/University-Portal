using System;
using System.Data;
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
                string query = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Username", txtUsername.Text.Trim()),
                    new SqlParameter("@Password", txtPassword.Text)
                };

                DataTable dt = DbConnection.GetData(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    Session["UserId"] = row["UserId"];
                    Session["Role"] = row["Role"];
                    Session["FullName"] = row["FullName"];

                    // Redirect based on role
                    string role = row["Role"].ToString();
                    if (role == "Admin")
                        Response.Redirect("~/Admin/Dashboard.aspx");
                    else if (role == "Teacher")
                        Response.Redirect("~/Teacher/Dashboard.aspx");
                    else if (role == "Student")
                        Response.Redirect("~/Student/Dashboard.aspx");
                }
                else
                {
                    ShowError("Invalid username or password");
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