<%@ Page Title="Enroll Courses" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Enrollment.aspx.cs" Inherits="UniversityPortal.Student.Enrollment" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Course Enrollment</h1>
    
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </asp:Panel>

    <div class="card">
        <h3>Available Courses</h3>
        <asp:GridView ID="gvAvailableCourses" runat="server" AutoGenerateColumns="False" OnRowCommand="gvAvailableCourses_RowCommand">
            <Columns>
                <asp:BoundField DataField="CourseName" HeaderText="Course Name" />
                <asp:BoundField DataField="CourseCode" HeaderText="Code" />
                <asp:BoundField DataField="CreditHours" HeaderText="Credits" />
                <asp:BoundField DataField="TeacherName" HeaderText="Teacher" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEnroll" runat="server" CommandName="Enroll" 
                            CommandArgument='<%# Eval("CourseId") %>' CssClass="btn btn-success" 
                            Text="Enroll" OnClientClick="return confirm('Enroll in this course?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <div class="card">
        <h3>My Enrolled Courses</h3>
        <asp:GridView ID="gvEnrolledCourses" runat="server" AutoGenerateColumns="False" 
            OnRowCommand="gvEnrolledCourses_RowCommand" DataKeyNames="EnrollmentId">
            <Columns>
                <asp:BoundField DataField="CourseName" HeaderText="Course Name" />
                <asp:BoundField DataField="CourseCode" HeaderText="Code" />
                <asp:BoundField DataField="CreditHours" HeaderText="Credits" />
                <asp:BoundField DataField="EnrollmentDate" HeaderText="Enrolled On" DataFormatString="{0:MM/dd/yyyy}" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDrop" runat="server" CommandName="Drop" 
                            CommandArgument='<%# Eval("EnrollmentId") %>' CssClass="btn btn-danger" 
                            Text="Drop" OnClientClick="return confirm('Drop this course?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>