<%@ Page Title="" Language="C#" MasterPageFile="~/LectureSystem.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="LectureCheckInSystem.Admin.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="AdminStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="admin-dashboard">
        <h1>Dashboard</h1>
        
        <!-- Stats Cards -->
        <div class="stats-grid">
            <div class="stat-card">
                <h3>Total Lecturers</h3>
                <p class="stat-number"><asp:Label ID="lblTotalLecturers" runat="server" Text="0" /></p>
            </div>
            <div class="stat-card">
                <h3>Today's Schedules</h3>
                <p class="stat-number"><asp:Label ID="lblTodaySchedules" runat="server" Text="0" /></p>
            </div>
            <div class="stat-card">
                <h3>Check-ins Completed</h3>
                <p class="stat-number"><asp:Label ID="lblCheckinsCompleted" runat="server" Text="0" /></p>
            </div>
            <div class="stat-card">
                <h3>Pending Check-ins</h3>
                <p class="stat-number"><asp:Label ID="lblPendingCheckins" runat="server" Text="0" /></p>
            </div>
        </div>

        <!-- Check-in Trends (simulated chart) -->
        <div class="trends-section">
            <h2>Check-in Trends (Last 7 Days)</h2>
            <div class="trend-bars">
                <asp:Repeater ID="rptTrends" runat="server" OnItemCommand="rptTrends_ItemCommand">
                    <ItemTemplate>
                        <div class="trend-item">
                            <span class="day"><%# Eval("Day") %></span>
                            <div class="bar-container">
                                <div class="bar" style="width: <%# Eval("Percent") %>%;"></div>
                                <span class="count"><%# Eval("Count") %></span>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Recent Check-in Records -->
        <div class="recent-section">
            <h2>Recent Check-in Records</h2>
            <asp:GridView ID="gvRecentCheckins" runat="server" AutoGenerateColumns="False" CssClass="data-grid" EmptyDataText="No check-ins yet.">
                <Columns>
                    <asp:BoundField DataField="LecturerName" HeaderText="Lecturer" />
                    <asp:BoundField DataField="Time" HeaderText="Time" />
                    <asp:BoundField DataField="Status" HeaderText="Status" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
