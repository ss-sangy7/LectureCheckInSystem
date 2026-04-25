<%@ Page Title="" Language="C#" MasterPageFile="~/LectureSystem.Master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="LectureCheckInSystem.Admin.ManageUsers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="AdminStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="manage-container">
        <h1>Manage Users</h1>
        <div class="toolbar">
            <asp:Button ID="btnAddUser" runat="server" Text="+ Add Lecturer" CssClass="btn-add" OnClick="btnAddUser_Click" />
        </div>

        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" CssClass="data-grid" DataKeyNames="LecturerID" OnRowCommand="gvUsers_RowCommand">
            <Columns>
                <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="Username" HeaderText="Username" />
                <asp:BoundField DataField="Department" HeaderText="Department" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="EditUser" CommandArgument='<%# Eval("LecturerID") %>' CssClass="btn-edit" />
                        <asp:Button ID="btnResetPwd" runat="server" Text="Reset Pwd" CommandName="ResetPassword" CommandArgument='<%# Eval("LecturerID") %>' CssClass="btn-reset" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="DeleteUser" CommandArgument='<%# Eval("LecturerID") %>' CssClass="btn-delete" OnClientClick="return confirm('Delete this user?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <!-- Modal for Add/Edit User -->
    <asp:Panel ID="pnlModal" runat="server" CssClass="modal" Visible="false">
        <div class="modal-content">
            <h3><asp:Label ID="lblModalTitle" runat="server" Text="Add Lecturer" /></h3>
            <div class="form-group">
                <label>First Name</label>
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="input-field" />
            </div>
            <div class="form-group">
                <label>Last Name</label>
                <asp:TextBox ID="txtLastName" runat="server" CssClass="input-field" />
            </div>
            <div class="form-group">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="input-field" />
            </div>
            <div class="form-group">
                <label>Username</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="input-field" />
            </div>
            <div class="form-group">
                <label>Department</label>
                <asp:TextBox ID="txtDepartment" runat="server" CssClass="input-field" />
            </div>
            <div class="form-group" id="passwordFields" runat="server">
                <label>Password (for new user)</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input-field" />
            </div>
            <div class="modal-buttons">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn-add" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-delete" OnClick="btnCancel_Click" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>
