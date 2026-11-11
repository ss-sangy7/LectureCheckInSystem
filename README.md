# Lecture Check-In System

An experimental ASP.NET Web Forms application for managing lecturer accounts and lecture schedules, and recording lecturer check-ins. It has separate lecturer and administrator pages, with SQL Server LocalDB used for persistence.

## How it works

The application uses ASP.NET Web Forms: each `.aspx` page defines the interface, and its C# code-behind handles page loads, form submissions, and database operations. Database access is performed with ADO.NET (`SqlConnection`, `SqlCommand`, and `SqlDataAdapter`), using the `LecturerCheckInDB` connection string in `Web.config`.

1. A user registers from `Accounts/Register.aspx`. The code-behind validates the form, hashes the password with BCrypt, and inserts the account into `Lecturers`.
2. On `Accounts/Login.aspx`, the app looks up the username, verifies the BCrypt hash, and checks that the selected role matches the database role. It stores the user ID, name, and role in the ASP.NET session. Lecturers are sent to check-in; admins go to the dashboard.
3. A lecturer's `Lecturer/CheckIn.aspx` page loads today's schedules and displays whether each has a check-in record for that lecturer. Selecting Check In inserts a row in `CheckIns`. `Lecturer/Profile.aspx` loads and updates profile details and allows a password change after verifying the current password.
4. Admin pages require a session with the `Admin` role. `Admin/Dashboard.aspx` displays counts, recent check-ins, and a seven-day trend. `Admin/ManageUsers.aspx` supports user management, and `Admin/ManageSchedules.aspx` supports schedule creation, editing, and deletion.
5. `LectureSystem.Master` and its code-behind provide shared layout and role-dependent navigation. Session state is configured in `Web.config` and expires after 30 minutes.

The database code expects these tables and relationships: `Lecturers` stores account details and password hashes; `Schedules` stores course, date, time, room, and lecturer; and `CheckIns` associates a lecturer with a schedule and check-in time. The application code relies on the database to prevent duplicate check-ins (a duplicate-key SQL error is handled in the check-in page).

## Repository layout

```text
Accounts/       Registration and login pages
Admin/          Admin dashboard, user management, and schedule management
Images/         Images used by the shared layout
Lecturer/       Lecturer check-in and profile pages
Properties/     Assembly metadata
Styles/         Shared site styles
LectureSystem.Master*  Shared Web Forms layout and code-behind
LectureCheckInSystem.csproj  Classic ASP.NET Web Application project
LectureCheckInSystem.slnx    Visual Studio solution
Web.config      Database connection, .NET Framework and session configuration
packages.config NuGet dependencies
```

Each Web Forms page typically has an `.aspx` markup file, a `.aspx.cs` code-behind file, and a generated `.designer.cs` file. Page-specific CSS sits alongside pages; shared styling is in `Styles/`.

## Requirements

- Windows with Visual Studio and the **ASP.NET and web development** workload
- .NET Framework 4.7.2 targeting/developer pack
- SQL Server LocalDB

## Run locally

1. Clone the repository and open `LectureCheckInSystem.slnx` in Visual Studio.
2. Restore the NuGet packages listed in `packages.config` (including `BCrypt.Net-Next`).
3. Set up the SQL Server LocalDB database with the tables expected by the application. The project file references `App_Data/LectureCheckInDB.mdf`; ensure this database is available in your checkout or create/configure an equivalent LocalDB database.
4. Update the `LecturerCheckInDB` connection string in `Web.config` to point to the database on your machine. The checked-in value contains a machine-specific absolute file path and will not work on most computers.
5. Set the web project as the startup project and run it with IIS Express from Visual Studio. The configured local HTTPS URL is `https://localhost:44362/`.

There is no documented schema script or test command in the repository. The database file is referenced by the project, but may not be present in every checkout; database setup is therefore required before the pages that access data can run.

## Notes

- This is an experimental project, not a production-ready attendance system. Review account authorization, password policy, database setup, and error handling before deploying it.
- `Accounts/Register.aspx` accepts a role from the registration form. Restricting who can create administrator accounts should be addressed before using this beyond local experimentation.
- The check-in code-behind currently calls `ExecuteNonQuery()` twice for a successful insert; review this behavior before relying on check-in submissions.
