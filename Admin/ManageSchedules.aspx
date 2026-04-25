<%@ Page Title="" Language="C#" MasterPageFile="~/LectureSystem.Master" AutoEventWireup="true" CodeBehind="ManageSchedules.aspx.cs" Inherits="LectureCheckInSystem.Admin.ManageSchedules" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="AdminStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="manage-container">
        <h1>Schedule List</h1>
        <div class="toolbar">
            <asp:Button ID="btnAddSchedule" runat="server" Text="+ Add Schedule" CssClass="btn-add" OnClick="btnAddSchedule_Click" />
        </div>
        <asp:GridView ID="gvSchedules" runat="server" AutoGenerateColumns="False" CssClass="data-grid" DataKeyNames="ScheduleID" OnRowCommand="gvSchedules_RowCommand">
            <Columns>
                <asp:BoundField DataField="ScheduleDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="CourseCode" HeaderText="Course" />
                <asp:BoundField DataField="Room" HeaderText="Room" />
                <asp:BoundField DataField="StartTime" HeaderText="Time" />
                <asp:BoundField DataField="LecturerName" HeaderText="Assigned Lecturer" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="EditSchedule" CommandArgument='<%# Eval("ScheduleID") %>' CssClass="btn-edit" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="DeleteSchedule" CommandArgument='<%# Eval("ScheduleID") %>' CssClass="btn-delete" OnClientClick="return confirm('Delete this schedule?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <!-- Modal for Add/Edit Schedule (hidden by default) -->
    <asp:Panel ID="pnlModal" runat="server" CssClass="modal" Visible="false">
        <div class="modal-content">
            <h3><asp:Label ID="lblModalTitle" runat="server" Text="Add Schedule" /></h3>
            <div class="form-group">
                <label>Date</label>
                <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="input-field" />
            </div>
            <div class="form-group">
                <label>Course Name</label>
                <asp:TextBox ID="txtCourseName" runat="server" CssClass="input-field" />
            </div>
            <div class="form-group">
                <label>Course Code</label>
                <asp:TextBox ID="txtCourseCode" runat="server" CssClass="input-field" />
            </div>
            <div class="form-group">
                <label>Room</label>
                <asp:TextBox ID="txtRoom" runat="server" CssClass="input-field" />
            </div>
            <div class="form-group">
                <label>Time</label>
                <asp:TextBox ID="txtTime" runat="server" TextMode="Time" CssClass="input-field" />
            </div>
            <div class="form-group">
                <label>Lecturer</label>
                <asp:DropDownList ID="ddlLecturer" runat="server" CssClass="input-field" />
            </div>
            <div class="modal-buttons">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn-add" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-delete" OnClick="btnCancel_Click" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>
