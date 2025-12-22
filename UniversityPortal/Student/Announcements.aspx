<%@ Page Title="Announcements" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Announcements.aspx.cs" Inherits="UniversityPortal.Student.Announcements" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Announcements & Warnings</h1>

    <div class="card">
        <h3>⚠️ Attendance Warnings</h3>
        <asp:GridView ID="gvWarnings" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="CourseName" HeaderText="Course" />
                <asp:BoundField DataField="Message" HeaderText="Warning Message" />
                <asp:BoundField DataField="SentDate" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy hh:mm tt}" />
            </Columns>
        </asp:GridView>
        <asp:Label ID="lblNoWarnings" runat="server" Text="No warnings at this time." Visible="false" style="color: #27ae60;"></asp:Label>
    </div>

    <div class="card">
        <h3>📢 Course Announcements</h3>
        <asp:Repeater ID="rptAnnouncements" runat="server">
            <ItemTemplate>
                <div style="border-left: 4px solid #667eea; padding-left: 15px; margin-bottom: 20px;">
                    <h4 style="color: #667eea;"><%# Eval("Title") %></h4>
                    <p style="color: #666; font-size: 12px;">
                        Course: <strong><%# Eval("CourseName") %></strong> | 
                        Posted by: <strong><%# Eval("TeacherName") %></strong> | 
                        Date: <%# Eval("CreatedDate", "{0:MM/dd/yyyy hh:mm tt}") %>
                    </p>
                    <p style="margin-top: 10px;"><%# Eval("Content") %></p>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Label ID="lblNoAnnouncements" runat="server" Text="No announcements yet." Visible="false"></asp:Label>
    </div>
</asp:Content>