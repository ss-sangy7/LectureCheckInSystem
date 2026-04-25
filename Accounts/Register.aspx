<%@ Page Title="" Language="C#" MasterPageFile="~/LectureSystem.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="LectureCheckInSystem.Accounts.Register" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Register.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function validateForm() {
            // Get field values
            var firstName = document.getElementById('<%= txtFirstName.ClientID %>').value.trim();
            var lastName = document.getElementById('<%= txtLastName.ClientID %>').value.trim();
            var email = document.getElementById('<%= txtEmail.ClientID %>').value.trim();
            var username = document.getElementById('<%= txtUsername.ClientID %>').value.trim();
            var password = document.getElementById('<%= txtPassword.ClientID %>').value;
            var confirmPassword = document.getElementById('<%= txtConfirmPassword.ClientID %>').value;
            var department = document.getElementById('<%= ddlDepartment.ClientID %>').value;

            // Check empty fields
            if (firstName === "") {
                alert("Please enter First Name.");
                return false;
            }
            if (lastName === "") {
                alert("Please enter Last Name.");
                return false;
            }
            if (email === "") {
                alert("Please enter Email Address.");
                return false;
            }
            if (username === "") {
                alert("Please enter Username.");
                return false;
            }
            if (password === "") {
                alert("Please enter Password.");
                return false;
            }
            if (confirmPassword === "") {
                alert("Please confirm Password.");
                return false;
            }
            if (department === "") {
                alert("Please select Department.");
                return false;
            }

            // Check password match
            if (password !== confirmPassword) {
                alert("Passwords do not match.");
                return false;
            }

            // Check password length
            if (password.length < 6) {
                alert("Password must be at least 6 characters.");
                return false;
            }

            // Basic email format check (simple, may improve later)
            var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailPattern.test(email)) {
                alert("Please enter a valid email address.");
                return false;
            }

            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="register-container">
        <h2>Create New Account</h2>
        
        <div class="form-row">
            <div class="form-group">
                <label>Role</label>
                    <asp:DropDownList ID="ddlRole" runat="server" CssClass="input-field">
                    <asp:ListItem Text="Lecturer" Value="Lecturer" Selected="True" />
                    <asp:ListItem Text="Admin" Value="Admin" />
                  </asp:DropDownList>
                <label>First Name</label>
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="input-field" />
            </div>
            <div class="form-group">
                <label>Last Name</label>
                <asp:TextBox ID="txtLastName" runat="server" CssClass="input-field" />
            </div>
        </div>
        
        <div class="form-group">
            <label>Email Address</label>
            <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="input-field" />
        </div>
        
        <div class="form-group">
            <label>Username</label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="input-field" />
        </div>
        
        <div class="form-group">
            <label>Department</label>
            <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="input-field">
                <asp:ListItem Text="-- Select Department --" Value="" />
                <asp:ListItem Text="Computer Science" Value="CS" />
                <asp:ListItem Text="Engineering" Value="ENG" />
                <asp:ListItem Text="Business" Value="BUS" />
                <asp:ListItem Text="Mathematics" Value="MATH" />
            </asp:DropDownList>
        </div>
        
        <div class="form-row">
            <div class="form-group">
                <label>Password</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input-field" />
            </div>
            <div class="form-group">
                <label>Confirm Password</label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="input-field" />
            </div>
        </div>
        
        <asp:Button ID="btnRegister" runat="server" Text="Create Account" CssClass="btn-register" OnClick="btnRegister_Click" OnClientClick="return validateForm();" />
     
        <div class="login-link">
            Already have an account? <a href="Login.aspx">Sign In</a>
        </div>
    </div>
</asp:Content>
