<%@ Page Title="Manage Announcements" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Announcements.aspx.cs" Inherits="UniversityPortal.Teacher.Announcements" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Manage Announcements</h1>
    
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </asp:Panel>

    <div class="card">
        <h3><asp:Label ID="lblFormTitle" runat="server" Text="Create New Announcement"></asp:Label></h3>
        
        <asp:HiddenField ID="hfAnnouncementId" runat="server" />
        
        <div class="form-group">
            <label>Select Course</label>
            <asp:DropDownList ID="ddlCourse" runat="server"></asp:DropDownList>
        </div>
        
        <div class="form-group">
            <label>Title</label>
            <asp:TextBox ID="txtTitle" runat="server" placeholder="Announcement title"></asp:TextBox>
        </div>
        
        <div class="form-group">
            <label>Content</label>
            <asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" Rows="5" placeholder="Announcement content"></asp:TextBox>
        </div>
        
        <asp:Button ID="btnSave" runat="server" Text="Post Announcement" CssClass="btn btn-success" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
    </div>

    <div class="card">
        <h3>My Announcements</h3>
        <asp:GridView ID="gvAnnouncements" runat="server" AutoGenerateColumns="False" DataKeyNames="AnnouncementId">
            <Columns>
                <asp:BoundField DataField="CourseName" HeaderText="Course" />
                <asp:BoundField DataField="Title" HeaderText="Title" />
                <asp:BoundField DataField="Content" HeaderText="Content" />
                <asp:BoundField DataField="CreatedDate" HeaderText="Posted On" DataFormatString="{0:MM/dd/yyyy hh:mm tt}" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" runat="server" 
                            CommandArgument='<%# Eval("AnnouncementId") %>' CssClass="btn btn-primary" Text="Edit" 
                            OnClick="btnEdit_Click" />
                        <asp:LinkButton ID="btnDelete" runat="server" 
                            CommandArgument='<%# Eval("AnnouncementId") %>' CssClass="btn btn-danger" 
                            Text="Delete" OnClientClick="return confirm('Delete this announcement?');" 
                            OnClick="btnDelete_Click" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>