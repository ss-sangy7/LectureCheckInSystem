<%@ Page Title="" Language="C#" MasterPageFile="~/LectureSystem.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="LectureCheckInSystem.Lecturer.Profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Profile.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="profile-container">
    <h2>Profile</h2>
    <div class="profile-info">
        <div class="form-group">
            <label>First Name</label>
            <asp:TextBox ID="txtFirstName" runat="server" CssClass="input-field" />
        </div>
        <div class="form-group">
            <label>Last Name</label>
            <asp:TextBox ID="txtLastName" runat="server" CssClass="input-field" />
        </div>
        <div class="form-group">
            <label>Email Address</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="input-field" />
        </div>
        <div class="form-group">
            <label>Department</label>
            <asp:Label ID="lblDepartmentStatic" runat="server" CssClass="readonly-field" />
        </div>
        <asp:Button ID="btnUpdateProfile" runat="server" Text="Update Profile" CssClass="btn-update" OnClick="btnUpdateProfile_Click" />
        <asp:Label ID="lblProfileMessage" runat="server" ForeColor="Green" />
    </div>

    <div class="change-password">
        <h3>Change Password</h3>
        <!-- existing password fields -->
        <div class="form-group">
            <label>Current Password</label>
            <asp:TextBox ID="txtCurrentPassword" runat="server" TextMode="Password" CssClass="input-field" />
        </div>
        <div class="form-group">
            <label>New Password</label>
            <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" CssClass="input-field" />
        </div>
        <div class="form-group">
            <label>Confirm New Password</label>
            <asp:TextBox ID="txtConfirmNewPassword" runat="server" TextMode="Password" CssClass="input-field" />
        </div>
        <asp:Button ID="btnUpdatePassword" runat="server" Text="Update Password" CssClass="btn-update" OnClick="btnUpdatePassword_Click" />
        <asp:Label ID="lblPasswordMessage" runat="server" ForeColor="Green" />
    </div>
</div>
</asp:Content>
