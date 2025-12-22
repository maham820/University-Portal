<%@ Page Title="GPA Calculator" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GpaCalculator.aspx.cs" Inherits="UniversityPortal.Student.GpaCalculator" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>GPA Calculator</h1>
    
    <div class="card">
        <h3>Calculate Expected GPA</h3>
        <p>Enter your course details and expected marks to calculate your potential GPA. Grade points: A=4.0, B=3.0, C=2.0, D=1.0, F=0.0</p>
        
        <asp:Panel ID="pnlCalculator" runat="server">
            <div style="margin-bottom: 15px;">
                <label>Number of Courses:</label>
                <asp:TextBox ID="txtNumCourses" runat="server" TextMode="Number" Width="100px" Text="1" min="1" max="10"></asp:TextBox>
                <asp:Button ID="btnGenerateCourses" runat="server" Text="Generate Fields" OnClick="btnGenerateCourses_Click" CssClass="btn" />
            </div>

            <table>
                <thead>
                    <tr>
                        <th>Course Name</th>
                        <th>Credit Hours</th>
                        <th>Expected Mids (25)</th>
                        <th>Expected Internals (25)</th>
                        <th>Expected Finals (50)</th>
                        <th>Total (100)</th>
                        <th>Grade</th>
                        <th>Grade Points</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptCourses" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtCourseName" runat="server" Width="150px" placeholder="Course name"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCreditHours" runat="server" TextMode="Number" Width="80px" Text="3" min="1" max="6"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMids" runat="server" TextMode="Number" Width="80px" Text="0" min="0" max="25" step="0.5"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtInternals" runat="server" TextMode="Number" Width="80px" Text="0" min="0" max="25" step="0.5"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtFinals" runat="server" TextMode="Number" Width="80px" Text="0" min="0" max="50" step="0.5"></asp:TextBox>
                                </td>
                                <td><asp:Label ID="lblTotal" runat="server" Text="0"></asp:Label></td>
                                <td><asp:Label ID="lblGrade" runat="server" Text="-"></asp:Label></td>
                                <td><asp:Label ID="lblGradePoints" runat="server" Text="0"></asp:Label></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
            
            <asp:Button ID="btnCalculate" runat="server" Text="Calculate GPA" CssClass="btn btn-primary" OnClick="btnCalculate_Click" />
        </asp:Panel>
    </div>

    <asp:Panel ID="pnlResults" runat="server" Visible="false">
        <div class="card">
            <h3>GPA Results</h3>
            <div class="dashboard-stats">
                <div class="stat-card">
                    <h3>Total Credit Hours</h3>
                    <div class="number"><asp:Label ID="lblTotalCredits" runat="server"></asp:Label></div>
                </div>
                <div class="stat-card">
                    <h3>Quality Points</h3>
                    <div class="number"><asp:Label ID="lblQualityPoints" runat="server"></asp:Label></div>
                </div>
                <div class="stat-card">
                    <h3>Expected GPA</h3>
                    <div class="number" style="color: #27ae60;"><asp:Label ID="lblGPA" runat="server"></asp:Label></div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <div class="card">
        <h3>Grading Scale</h3>
        <table>
            <tr><th>Marks</th><th>Grade</th><th>Grade Points</th></tr>
            <tr><td>85-100</td><td>A</td><td>4.0</td></tr>
            <tr><td>70-84</td><td>B</td><td>3.0</td></tr>
            <tr><td>60-69</td><td>C</td><td>2.0</td></tr>
            <tr><td>50-59</td><td>D</td><td>1.0</td></tr>
            <tr><td>0-49</td><td>F</td><td>0.0</td></tr>
        </table>
    </div>
</asp:Content>