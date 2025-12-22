<%@ Page Title="Send Warnings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SendWarning.aspx.cs" Inherits="UniversityPortal.Admin.SendWarning" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Send Attendance Warnings</h1>
    
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </asp:Panel>

    <div class="card">
        <h3>Students with Low Attendance (&lt; 60%)</h3>
        <asp:GridView ID="gvLowAttendance" runat="server" AutoGenerateColumns="False" DataKeyNames="StudentId">
            <Columns>
                <asp:BoundField DataField="StudentName" HeaderText="Student Name" />
                <asp:BoundField DataField="CourseName" HeaderText="Course" />
                <asp:BoundField DataField="TotalClasses" HeaderText="Total Classes" />
                <asp:BoundField DataField="Present" HeaderText="Present" />
                <asp:BoundField DataField="Absent" HeaderText="Absent" />
                <asp:BoundField DataField="Late" HeaderText="Late" />
                <asp:BoundField DataField="AttendancePercentage" HeaderText="Attendance %" DataFormatString="{0:F2}%" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnSendWarning" runat="server" CommandName="SendWarning" 
                            CommandArgument='<%# Eval("StudentId") + "," + Eval("CourseName") + "," + Eval("AttendancePercentage") %>' 
                            CssClass="btn btn-warning" Text="Send Warning" OnClick="btnSendWarning_Click" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <div class="card">
        <h3>All Attendance Records</h3>
        <asp:GridView ID="gvAllAttendance" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="StudentName" HeaderText="Student" />
                <asp:BoundField DataField="CourseName" HeaderText="Course" />
                <asp:BoundField DataField="TotalClasses" HeaderText="Total Classes" />
                <asp:BoundField DataField="AttendancePercentage" HeaderText="Attendance %" DataFormatString="{0:F2}%" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
