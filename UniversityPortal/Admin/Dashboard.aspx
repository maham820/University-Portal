<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="UniversityPortal.Admin.Dashboard" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Admin Dashboard</h1>
    
    <div class="dashboard-stats">
        <div class="stat-card">
            <h3>Total Students</h3>
            <div class="number"><asp:Label ID="lblTotalStudents" runat="server"></asp:Label></div>
        </div>
        <div class="stat-card">
            <h3>Total Teachers</h3>
            <div class="number"><asp:Label ID="lblTotalTeachers" runat="server"></asp:Label></div>
        </div>
        <div class="stat-card">
            <h3>Total Courses</h3>
            <div class="number"><asp:Label ID="lblTotalCourses" runat="server"></asp:Label></div>
        </div>
        <div class="stat-card">
            <h3>Total Enrollments</h3>
            <div class="number"><asp:Label ID="lblTotalEnrollments" runat="server"></asp:Label></div>
        </div>
    </div>

    <div class="card">
        <h3>Recent Activity</h3>
        <asp:GridView ID="gvRecentEnrollments" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="StudentName" HeaderText="Student" />
                <asp:BoundField DataField="CourseName" HeaderText="Course" />
                <asp:BoundField DataField="EnrollmentDate" HeaderText="Enrolled On" DataFormatString="{0:MM/dd/yyyy}" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>