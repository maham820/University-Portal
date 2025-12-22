using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;

namespace UniversityPortal.Student
{
    public partial class GpaCalculator : System.Web.UI.Page
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
                GenerateCourseFields(5); // Default 5 courses
            }
        }

        protected void btnGenerateCourses_Click(object sender, EventArgs e)
        {
            int numCourses = 1;
            if (int.TryParse(txtNumCourses.Text, out numCourses))
            {
                if (numCourses < 1) numCourses = 1;
                if (numCourses > 10) numCourses = 10;
                GenerateCourseFields(numCourses);
            }
        }

        private void GenerateCourseFields(int count)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Index", typeof(int));
            
            for (int i = 0; i < count; i++)
            {
                dt.Rows.Add(i);
            }

            rptCourses.DataSource = dt;
            rptCourses.DataBind();
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            double totalCredits = 0;
            double qualityPoints = 0;

            foreach (RepeaterItem item in rptCourses.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    TextBox txtCourseName = (TextBox)item.FindControl("txtCourseName");
                    TextBox txtCreditHours = (TextBox)item.FindControl("txtCreditHours");
                    TextBox txtMids = (TextBox)item.FindControl("txtMids");
                    TextBox txtInternals = (TextBox)item.FindControl("txtInternals");
                    TextBox txtFinals = (TextBox)item.FindControl("txtFinals");
                    Label lblTotal = (Label)item.FindControl("lblTotal");
                    Label lblGrade = (Label)item.FindControl("lblGrade");
                    Label lblGradePoints = (Label)item.FindControl("lblGradePoints");

                    // Skip if course name is empty
                    if (string.IsNullOrWhiteSpace(txtCourseName.Text))
                        continue;

                    // Get credit hours from user input
                    int creditHours = 3; // Default
                    if (!string.IsNullOrEmpty(txtCreditHours.Text))
                    {
                        int.TryParse(txtCreditHours.Text, out creditHours);
                    }

                    double mids = string.IsNullOrEmpty(txtMids.Text) ? 0 : double.Parse(txtMids.Text);
                    double internals = string.IsNullOrEmpty(txtInternals.Text) ? 0 : double.Parse(txtInternals.Text);
                    double finals = string.IsNullOrEmpty(txtFinals.Text) ? 0 : double.Parse(txtFinals.Text);

                    double total = mids + internals + finals;
                    lblTotal.Text = total.ToString("F2");

                    // Calculate grade and grade points
                    string grade = GetGrade(total);
                    double gradePoints = GetGradePoints(total);

                    lblGrade.Text = grade;
                    lblGradePoints.Text = gradePoints.ToString("F1");

                    totalCredits += creditHours;
                    qualityPoints += (gradePoints * creditHours);
                }
            }

            // Calculate GPA
            double gpa = totalCredits > 0 ? qualityPoints / totalCredits : 0;

            lblTotalCredits.Text = totalCredits.ToString("F0");
            lblQualityPoints.Text = qualityPoints.ToString("F2");
            lblGPA.Text = gpa.ToString("F2");

            pnlResults.Visible = true;
        }

        private string GetGrade(double marks)
        {
            if (marks >= 85) return "A";
            if (marks >= 70) return "B";
            if (marks >= 60) return "C";
            if (marks >= 50) return "D";
            return "F";
        }

        private double GetGradePoints(double marks)
        {
            if (marks >= 85) return 4.0;
            if (marks >= 70) return 3.0;
            if (marks >= 60) return 2.0;
            if (marks >= 50) return 1.0;
            return 0.0;
        }
    }
}