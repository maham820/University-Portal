<%@ Page Title="Manage Attendance" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Attendance.aspx.cs" Inherits="UniversityPortal.Teacher.Attendance" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Manage Attendance</h1>
    
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </asp:Panel>

    <div class="card">
        <h3>Select Course</h3>
        <div class="form-group">
            <label>Course</label>
            <asp:DropDownList ID="ddlCourse" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCourse_SelectedIndexChanged"></asp:DropDownList>
        </div>
    </div>

    <asp:Panel ID="pnlAttendance" runat="server" Visible="false">
        <div class="card">
            <h3>Mark Attendance</h3>
            <div class="form-group">
                <label>Class Date</label>
                <asp:TextBox ID="txtClassDate" runat="server" TextMode="Date"></asp:TextBox>
            </div>

            <asp:GridView ID="gvStudents" runat="server" AutoGenerateColumns="False" DataKeyNames="EnrollmentId">
                <Columns>
                    <asp:BoundField DataField="StudentName" HeaderText="Student Name" />
                    <asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlStatus" runat="server">
                                <asp:ListItem Value="Present">Present</asp:ListItem>
                                <asp:ListItem Value="Absent">Absent</asp:ListItem>
                                <asp:ListItem Value="Late">Late</asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:Button ID="btnSaveAttendance" runat="server" Text="Save Attendance" CssClass="btn btn-success" OnClick="btnSaveAttendance_Click" />
        </div>

        <div class="card">
            <h3>Attendance History</h3>
            <asp:GridView ID="gvAttendanceHistory" runat="server" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="ClassDate" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy}" />
                    <asp:BoundField DataField="StudentName" HeaderText="Student" />
                    <asp:BoundField DataField="Status" HeaderText="Status" />
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDelete" runat="server" 
                                CommandArgument='<%# Eval("AttendanceId") %>' 
                                CssClass="btn btn-danger" 
                                Text="Delete" 
                                OnClick="btnDelete_Click"
                                OnClientClick="return confirm('Delete this record?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </asp:Panel>
</asp:Content>