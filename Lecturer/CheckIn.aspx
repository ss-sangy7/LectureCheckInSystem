<%@ Page Title="" Language="C#" MasterPageFile="~/LectureSystem.Master" AutoEventWireup="true" CodeBehind="CheckIn.aspx.cs" Inherits="LectureCheckInSystem.Lecturer.CheckIn" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="CheckIn.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="checkin-container">
        <div class="welcome-section">
            <h2>Welcome back, <asp:Label ID="lblLecturerName" runat="server" />!</h2>
            <p><asp:Label ID="lblDate" runat="server" /></p>
        </div>

        <div class="schedule-grid">
            <asp:GridView ID="gvSchedule" runat="server" AutoGenerateColumns="False" CssClass="grid" DataKeyNames="ScheduleID"
                OnRowCommand="gvSchedule_RowCommand">
                <Columns>
                    <asp:BoundField DataField="CourseCode" HeaderText="Course" />
                    <asp:BoundField DataField="Room" HeaderText="Room" />
                    <asp:BoundField DataField="StartTime" HeaderText="Time" DataFormatString="{0:hh\:mm tt}" />
                    <asp:BoundField DataField="Status" HeaderText="Status" />
                    <asp:ButtonField Text="Check In" CommandName="CheckIn" ButtonType="Button" 
                        ItemStyle-CssClass="checkin-btn" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>