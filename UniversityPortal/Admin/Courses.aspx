<%@ Page Title="Manage Courses" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Courses.aspx.cs" Inherits="UniversityPortal.Admin.Courses" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Manage Courses</h1>
    
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </asp:Panel>

    <div class="card">
        <h3><asp:Label ID="lblFormTitle" runat="server" Text="Add New Course"></asp:Label></h3>
        
        <asp:HiddenField ID="hfCourseId" runat="server" />
        
        <div class="form-group">
            <label>Course Name</label>
            <asp:TextBox ID="txtCourseName" runat="server" placeholder="e.g., Database Systems"></asp:TextBox>
        </div>
        
        <div class="form-group">
            <label>Course Code</label>
            <asp:TextBox ID="txtCourseCode" runat="server" placeholder="e.g., CS301"></asp:TextBox>
        </div>
        
        <div class="form-group">
            <label>Credit Hours</label>
            <asp:TextBox ID="txtCreditHours" runat="server" TextMode="Number" placeholder="3"></asp:TextBox>
        </div>
        
        <div class="form-group">
            <label>Assign Teacher</label>
            <asp:DropDownList ID="ddlTeacher" runat="server"></asp:DropDownList>
        </div>
        
        <asp:Button ID="btnSave" runat="server" Text="Save Course" CssClass="btn btn-success" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
    </div>

    <div class="card">
        <h3>All Courses</h3>
        <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="CourseId" HeaderText="ID" />
                <asp:BoundField DataField="CourseName" HeaderText="Course Name" />
                <asp:BoundField DataField="CourseCode" HeaderText="Code" />
                <asp:BoundField DataField="CreditHours" HeaderText="Credits" />
                <asp:BoundField DataField="TeacherName" HeaderText="Teacher" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" runat="server" 
                            CommandArgument='<%# Eval("CourseId") %>' 
                            CssClass="btn btn-primary" 
                            Text="Edit" 
                            OnClick="btnEdit_Click" />
                        <asp:LinkButton ID="btnDelete" runat="server" 
                            CommandArgument='<%# Eval("CourseId") %>' 
                            CssClass="btn btn-danger" 
                            Text="Delete" 
                            OnClick="btnDelete_Click"
                            OnClientClick="return confirm('Delete this course?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>