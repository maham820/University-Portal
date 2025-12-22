<%@ Page Title="Manage Grades" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Grades.aspx.cs" Inherits="UniversityPortal.Teacher.Grades" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Manage Grades</h1>
    
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </asp:Panel>

    <div class="card">
        <h3>Select Course to Manage Grades</h3>
        <div class="form-group">
            <label>Course</label>
            <asp:DropDownList ID="ddlCourse" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCourse_SelectedIndexChanged"></asp:DropDownList>
        </div>
    </div>

    <asp:Panel ID="pnlGrades" runat="server" Visible="false">
        <div class="card">
            <h3>Student Grades</h3>
            <p><strong>Grading System:</strong> Mids (25 marks) + Internals (25 marks) + Finals (50 marks) = 100 marks total</p>
            
            <asp:GridView ID="gvGrades" runat="server" AutoGenerateColumns="False" DataKeyNames="EnrollmentId">
                <Columns>
                    <asp:BoundField DataField="StudentName" HeaderText="Student Name" />
                    <asp:TemplateField HeaderText="Mids (0-25)">
                        <ItemTemplate>
                            <asp:TextBox ID="txtMids" runat="server" Text='<%# Eval("Mids") %>' TextMode="Number" Width="80px" step="0.01" min="0" max="25"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Internals (0-25)">
                        <ItemTemplate>
                            <asp:TextBox ID="txtInternals" runat="server" Text='<%# Eval("Internals") %>' TextMode="Number" Width="80px" step="0.01" min="0" max="25"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Finals (0-50)">
                        <ItemTemplate>
                            <asp:TextBox ID="txtFinals" runat="server" Text='<%# Eval("Finals") %>' TextMode="Number" Width="80px" step="0.01" min="0" max="50"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Total" HeaderText="Total (100)" ReadOnly="True" />
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDeleteGrade" runat="server" 
                                CommandArgument='<%# Eval("EnrollmentId") %>' 
                                CssClass="btn btn-danger" 
                                Text="Clear Grades" 
                                OnClientClick="return confirm('Clear all grades for this student?');" 
                                OnClick="btnDeleteGrade_Click" 
                                Visible='<%# Convert.ToDecimal(Eval("Total")) > 0 %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Button ID="btnSaveGrades" runat="server" Text="Save All Grades" CssClass="btn btn-success" OnClick="btnSaveGrades_Click" />
        </div>
    </asp:Panel>
</asp:Content>