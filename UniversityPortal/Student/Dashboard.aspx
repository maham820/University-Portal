<%@ Page Title="Student Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="UniversityPortal.Student.Dashboard" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Student Dashboard</h1>

    <div class="dashboard-stats">
        <div class="stat-card">
            <h3>Enrolled Courses</h3>
            <div class="number"><asp:Label ID="lblEnrolledCourses" runat="server"></asp:Label></div>
        </div>
        <div class="stat-card">
            <h3>Attendance Rate</h3>
            <div class="number"><asp:Label ID="lblAttendanceRate" runat="server"></asp:Label>%</div>
        </div>
        <div class="stat-card">
            <h3>Warnings</h3>
            <div class="number"><asp:Label ID="lblWarnings" runat="server"></asp:Label></div>
        </div>
    </div>

    <div class="card">
        <h3>My Enrolled Courses</h3>
        <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="CourseName" HeaderText="Course" />
                <asp:BoundField DataField="CourseCode" HeaderText="Code" />
                <asp:BoundField DataField="CreditHours" HeaderText="Credits" />
                <asp:BoundField DataField="TeacherName" HeaderText="Teacher" />
            </Columns>
        </asp:GridView>
    </div>

    <div class="card">
        <h3>My Grades</h3>
        <asp:GridView ID="gvGrades" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="CourseName" HeaderText="Course" />
                <asp:BoundField DataField="Mids" HeaderText="Mids (25)" />
                <asp:BoundField DataField="Internals" HeaderText="Internals (25)" />
                <asp:BoundField DataField="Finals" HeaderText="Finals (50)" />
                <asp:BoundField DataField="Total" HeaderText="Total (100)" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>