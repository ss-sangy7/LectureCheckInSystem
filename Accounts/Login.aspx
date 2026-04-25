<%@ Page Title="" Language="C#" MasterPageFile="~/LectureSystem.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LectureCheckInSystem.Accounts.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <link href="Login.css" rel="stylesheet" />
    <title> Login - Lecturer Check-In</title>
    <script type="text/javascript">
        function validateLoginForm() {
            var username = document.getElementById('<%= txtUsername.ClientID %>');
            var password = document.getElementById('<%= txtPassword.ClientID %>');

            if (username.value.trim() === "") {
                alert("Please enter Username.");
                username.focus();
                return false;
            }
            if (password.value === "") {
                alert("Please enter Password.");
                password.focus();
                return false;
            }
            return true;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="login-card">
        <h2>Sign In</h2>
        <div class="form-group">
            <label>Login as</label>
    <asp:DropDownList ID="ddlRole" runat="server" CssClass="input-field">
        <asp:ListItem Text="Lecturer" Value="Lecturer" Selected="True" />
        <asp:ListItem Text="Admin" Value="Admin" />
    </asp:DropDownList>
            <label>Username</label>
            <asp:TextBox ID="txtUsername" runat="server"></asp:TextBox>
        </div>
        <div class="form-group">
            <label>Password</label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
        </div>
       <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="btn-primary" OnClick="btnSignIn_Click" OnClientClick="return validateLoginForm();" />
        <div class="register-link">
            Don't have an account? <a href="Register.aspx">Register here</a>
        </div>
    </div>
</asp:Content>
