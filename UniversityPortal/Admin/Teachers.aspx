<%@ Page Title="Manage Teachers" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Teachers.aspx.cs" Inherits="UniversityPortal.Admin.Teachers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Manage Teachers</h1>
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert"><asp:Label ID="lblMessage" runat="server"></asp:Label></asp:Panel>
    <div class="card">
        <h3><asp:Label ID="lblFormTitle" runat="server" Text="Add New Teacher"></asp:Label></h3>
        <asp:HiddenField ID="hfUserId" runat="server" />
        <div class="form-group"><label>Full Name</label><asp:TextBox ID="txtFullName" runat="server" placeholder="e.g., Dr. John Smith"></asp:TextBox></div>
        <div class="form-group"><label>Username</label><asp:TextBox ID="txtUsername" runat="server" placeholder="e.g., john.smith"></asp:TextBox></div>
        <div class="form-group"><label>Password</label><asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Enter password"></asp:TextBox></div>
        <div class="form-group"><label>Email</label><asp:TextBox ID="txtEmail" runat="server" TextMode="Email" placeholder="e.g., john@university.com"></asp:TextBox></div>
        <asp:Button ID="btnSave" runat="server" Text="Save Teacher" CssClass="btn btn-success" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
    </div>
    <div class="card">
        <h3>All Teachers</h3>
        <asp:GridView ID="gvTeachers" runat="server" AutoGenerateColumns="False" DataKeyNames="UserId" OnRowCommand="gvTeachers_RowCommand" OnRowDeleting="gvTeachers_RowDeleting">
            <Columns>
                <asp:BoundField DataField="UserId" HeaderText="ID" />
                <asp:BoundField DataField="FullName" HeaderText="Full Name" />
                <asp:BoundField DataField="Username" HeaderText="Username" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="CreatedDate" HeaderText="Created On" DataFormatString="{0:MM/dd/yyyy}" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditTeacher" CommandArgument='<%# Eval("UserId") %>' CssClass="btn btn-primary" Text="Edit" />
                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CommandArgument='<%# Eval("UserId") %>' CssClass="btn btn-danger" Text="Delete" OnClientClick="return confirm('Delete?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>