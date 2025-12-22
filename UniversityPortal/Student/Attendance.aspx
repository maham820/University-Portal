<%@ Page Title="My Attendance" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Attendance.aspx.cs" Inherits="UniversityPortal.Student.Attendance" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>My Attendance Records</h1>

    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </asp:Panel>

    <div class="card">
        <h3>Overall Attendance Summary</h3>
        <div class="dashboard-stats">
            <div class="stat-card" style="background: linear-gradient(135deg, #27ae60 0%, #219a52 100%);">
                <h3>Total Classes</h3>
                <div class="number"><asp:Label ID="lblTotalClasses" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="stat-card" style="background: linear-gradient(135deg, #3498db 0%, #2980b9 100%);">
                <h3>Present</h3>
                <div class="number"><asp:Label ID="lblTotalPresent" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="stat-card" style="background: linear-gradient(135deg, #e74c3c 0%, #c0392b 100%);">
                <h3>Absent</h3>
                <div class="number"><asp:Label ID="lblTotalAbsent" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="stat-card" style="background: linear-gradient(135deg, #f39c12 0%, #e67e22 100%);">
                <h3>Late</h3>
                <div class="number"><asp:Label ID="lblTotalLate" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="stat-card" style="background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);">
                <h3>Overall %</h3>
                <div class="number"><asp:Label ID="lblOverallPercentage" runat="server" Text="0"></asp:Label>%</div>
            </div>
        </div>
    </div>

    <asp:Repeater ID="rptCourseAttendance" runat="server">
        <ItemTemplate>
            <div class="card">
                <h3>📚 <%# Eval("CourseName") %> (<%# Eval("CourseCode") %>)</h3>
                <p style="color: #666; margin-bottom: 15px;">
                    <strong>Teacher:</strong> <%# Eval("TeacherName") %>
                </p>
                
                <div class="dashboard-stats">
                    <div class="stat-card">
                        <h3>Total Classes</h3>
                        <div class="number" style="color: #667eea;"><%# Eval("TotalClasses") %></div>
                    </div>
                    <div class="stat-card">
                        <h3>Present</h3>
                        <div class="number" style="color: #27ae60;"><%# Eval("Present") %></div>
                    </div>
                    <div class="stat-card">
                        <h3>Absent</h3>
                        <div class="number" style="color: #e74c3c;"><%# Eval("Absent") %></div>
                    </div>
                    <div class="stat-card">
                        <h3>Late</h3>
                        <div class="number" style="color: #f39c12;"><%# Eval("Late") %></div>
                    </div>
                    <div class="stat-card">
                        <h3>Attendance %</h3>
                        <div class="number" style='color: <%# Convert.ToDecimal(Eval("AttendancePercentage")) >= 60 ? "#27ae60" : "#e74c3c" %>;'>
                            <%# Eval("AttendancePercentage", "{0:F2}") %>%
                        </div>
                    </div>
                </div>

                <div style="margin-top: 20px;">
                    <h4 style="color: #555; margin-bottom: 10px;">Attendance Details</h4>
                    <asp:GridView ID="gvAttendanceDetails" runat="server" AutoGenerateColumns="False" 
                        DataSource='<%# Eval("AttendanceDetails") %>' CssClass="attendance-details-table">
                        <Columns>
                            <asp:BoundField DataField="ClassDate" HeaderText="Class Date" DataFormatString="{0:MM/dd/yyyy}" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <span style='padding: 5px 10px; border-radius: 3px; font-weight: bold; 
                                        background: <%# GetStatusColor(Eval("Status").ToString()) %>; 
                                        color: white;'>
                                        <%# Eval("Status") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="DayOfWeek" HeaderText="Day" />
                        </Columns>
                    </asp:GridView>
                </div>

                <%# Convert.ToDecimal(Eval("AttendancePercentage")) < 60 ? 
                    "<div style='background: #fff3cd; border-left: 4px solid #f39c12; padding: 15px; margin-top: 15px; border-radius: 5px;'>" +
                    "<strong style='color: #856404;'>⚠️ Warning:</strong> " +
                    "<span style='color: #856404;'>Your attendance is below 60%. Please attend classes regularly to avoid penalties.</span>" +
                    "</div>" : "" %>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <asp:Panel ID="pnlNoData" runat="server" Visible="false" CssClass="card">
        <div style="text-align: center; padding: 40px; color: #999;">
            <h3>No Attendance Records Found</h3>
            <p>You haven't been marked for attendance yet, or you're not enrolled in any courses.</p>
        </div>
    </asp:Panel>

    <style>
        .attendance-details-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 10px;
        }
        .attendance-details-table th {
            background: #f8f9fa;
            padding: 12px;
            text-align: left;
            border-bottom: 2px solid #ddd;
            color: #555;
            font-weight: 600;
        }
        .attendance-details-table td {
            padding: 10px 12px;
            border-bottom: 1px solid #eee;
        }
        .attendance-details-table tr:hover {
            background: #f9f9f9;
        }
    </style>
</asp:Content>