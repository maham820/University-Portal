<%@ Page Title="My Notes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Notes.aspx.cs" Inherits="UniversityPortal.Student.Notes" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>My Notes</h1>
    
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </asp:Panel>

    <div class="card">
        <h3><asp:Label ID="lblFormTitle" runat="server" Text="Add New Note"></asp:Label></h3>
        
        <asp:HiddenField ID="hfNoteId" runat="server" />
        
        <div class="form-group">
            <label>Title</label>
            <asp:TextBox ID="txtTitle" runat="server" placeholder="Note title"></asp:TextBox>
        </div>
        
        <div class="form-group">
            <label>Content</label>
            <asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" Rows="6" placeholder="Write your note here..."></asp:TextBox>
        </div>
        
        <asp:Button ID="btnSave" runat="server" Text="Save Note" CssClass="btn btn-success" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
    </div>

    <div class="card">
        <h3>All Notes</h3>
        <asp:GridView ID="gvNotes" runat="server" AutoGenerateColumns="False" 
            OnRowCommand="gvNotes_RowCommand" OnRowDeleting="gvNotes_RowDeleting">
            <Columns>
                <asp:BoundField DataField="Title" HeaderText="Title" />
                <asp:BoundField DataField="Content" HeaderText="Content" />
                <asp:BoundField DataField="CreatedDate" HeaderText="Created On" DataFormatString="{0:MM/dd/yyyy hh:mm tt}" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditNote" 
                            CommandArgument='<%# Eval("NoteId") %>' CssClass="btn btn-primary" Text="Edit" />
                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" 
                            CommandArgument='<%# Eval("NoteId") %>' CssClass="btn btn-danger" 
                            Text="Delete" OnClientClick="return confirm('Delete this note?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
