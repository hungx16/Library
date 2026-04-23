using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _3NF_Library.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "seq_Author");

            migrationBuilder.CreateSequence(
                name: "seq_Publisher");

            migrationBuilder.CreateTable(
                name: "tbl_ActivityLog",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ActorRole = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    ActorID = table.Column<int>(type: "int", nullable: false),
                    LogMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Acti__5E5499A81C57E284", x => x.LogID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Admin_Account",
                columns: table => new
                {
                    AdminID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Branch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Admi__719FE4E8ECFCD79F", x => x.AdminID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Attendance",
                columns: table => new
                {
                    AttendanceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountRole = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Atte__8B69263C0008E528", x => x.AttendanceID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Author",
                columns: table => new
                {
                    AuthorID = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false, defaultValueSql: "('TG'+right('00000'+CONVERT([varchar](5),NEXT VALUE FOR [seq_Author]),(5)))"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DOB = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Specialty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Auth__70DAFC14F3E0DB7C", x => x.AuthorID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Category",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShortName = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Cate__19093A2B450E4A15", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Librarian_Account",
                columns: table => new
                {
                    LibrarianID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Branch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Libr__E4D86D9D029EEAE1", x => x.LibrarianID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Publisher",
                columns: table => new
                {
                    PublisherID = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false, defaultValueSql: "('NXB'+right('00000'+CONVERT([varchar](5),NEXT VALUE FOR [seq_Publisher]),(5)))"),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Publ__4C657E4B6D2B2745", x => x.PublisherID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Reader_Account",
                columns: table => new
                {
                    ReaderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DOB = table.Column<DateOnly>(type: "date", nullable: true),
                    Organization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, defaultValue: "1223334444"),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MemberSince = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    Phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Read__8E67A5812D9BCACD", x => x.ReaderID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Book",
                columns: table => new
                {
                    BookID = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BookTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AuthorID = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    PublisherID = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: true),
                    PublishedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Value = table.Column<long>(type: "bigint", nullable: true, defaultValue: 0L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Book__3DE0C227F201C9BA", x => x.BookID);
                    table.ForeignKey(
                        name: "FK__tbl_Book__Author__619B8048",
                        column: x => x.AuthorID,
                        principalTable: "tbl_Author",
                        principalColumn: "AuthorID");
                    table.ForeignKey(
                        name: "FK__tbl_Book__Catego__6383C8BA",
                        column: x => x.CategoryID,
                        principalTable: "tbl_Category",
                        principalColumn: "CategoryID");
                    table.ForeignKey(
                        name: "FK__tbl_Book__Publis__628FA481",
                        column: x => x.PublisherID,
                        principalTable: "tbl_Publisher",
                        principalColumn: "PublisherID");
                });

            migrationBuilder.CreateTable(
                name: "tbl_Borrow_Order",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReaderID = table.Column<int>(type: "int", nullable: true),
                    DateBorrow = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    DateReturn = table.Column<DateOnly>(type: "date", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Borr__C3905BAF21FA1906", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK__tbl_Borro__Reade__6EF57B66",
                        column: x => x.ReaderID,
                        principalTable: "tbl_Reader_Account",
                        principalColumn: "ReaderID");
                });

            migrationBuilder.CreateTable(
                name: "tbl_Borrow_Detail",
                columns: table => new
                {
                    DetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    BookID = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Borrowing")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Borr__135C314DDA07383D", x => x.DetailID);
                    table.ForeignKey(
                        name: "FK__tbl_Borro__BookI__74AE54BC",
                        column: x => x.BookID,
                        principalTable: "tbl_Book",
                        principalColumn: "BookID");
                    table.ForeignKey(
                        name: "FK__tbl_Borro__Order__73BA3083",
                        column: x => x.OrderID,
                        principalTable: "tbl_Borrow_Order",
                        principalColumn: "OrderID");
                });

            migrationBuilder.CreateTable(
                name: "tbl_Fine",
                columns: table => new
                {
                    FineID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DetailID = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Amount = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Unpaid")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Fine__9D4A9BCC67C221AF", x => x.FineID);
                    table.ForeignKey(
                        name: "FK__tbl_Fine__Detail__787EE5A0",
                        column: x => x.DetailID,
                        principalTable: "tbl_Borrow_Detail",
                        principalColumn: "DetailID");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__tbl_Admi__536C85E476F99257",
                table: "tbl_Admin_Account",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_AuthorName",
                table: "tbl_Author",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IDX_BookTitle",
                table: "tbl_Book",
                column: "BookTitle");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Book_AuthorID",
                table: "tbl_Book",
                column: "AuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Book_CategoryID",
                table: "tbl_Book",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Book_PublisherID",
                table: "tbl_Book",
                column: "PublisherID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Borrow_Detail_BookID",
                table: "tbl_Borrow_Detail",
                column: "BookID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Borrow_Detail_OrderID",
                table: "tbl_Borrow_Detail",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Borrow_Order_ReaderID",
                table: "tbl_Borrow_Order",
                column: "ReaderID");

            migrationBuilder.CreateIndex(
                name: "UQ__tbl_Cate__A6160FD14E284F43",
                table: "tbl_Category",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Fine_DetailID",
                table: "tbl_Fine",
                column: "DetailID");

            migrationBuilder.CreateIndex(
                name: "UQ__tbl_Libr__536C85E48F78F773",
                table: "tbl_Librarian_Account",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_ReaderName",
                table: "tbl_Reader_Account",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.InsertData(
                table: "tbl_Admin_Account",
                columns: new[] { "AdminID", "Username", "Password", "FullName", "Branch", "IsActive" },
                values: new object[] { 1, "admin", "admin123", "System Admin", "Main Branch", true });

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[vw_Book_Public]', N'V') IS NOT NULL
    DROP VIEW [dbo].[vw_Book_Public];
EXEC(N'
CREATE VIEW [dbo].[vw_Book_Public]
AS
SELECT
    b.BookID,
    b.[Language],
    b.BookTitle,
    b.AuthorID,
    b.PublisherID,
    b.CategoryID,
    b.PublishedDate,
    b.[Value],
    CAST(ISNULL(b.Quantity, 0) AS int) AS CurrentlyAvailable,
    ISNULL(c.CategoryName, N''Chua phan loai'') AS CategoryName,
    CAST(LTRIM(RTRIM(ISNULL(a.LastName, N'''') + CASE WHEN a.LastName IS NOT NULL AND a.FirstName IS NOT NULL THEN N'' '' ELSE N'''' END + ISNULL(a.FirstName, N''''))) AS nvarchar(101)) AS AuthorName,
    p.[Description] AS PublisherName
FROM dbo.tbl_Book b
LEFT JOIN dbo.tbl_Author a ON a.AuthorID = b.AuthorID
LEFT JOIN dbo.tbl_Category c ON c.CategoryID = b.CategoryID
LEFT JOIN dbo.tbl_Publisher p ON p.PublisherID = b.PublisherID
');

IF OBJECT_ID(N'[dbo].[vw_Admin_BorrowStats]', N'V') IS NOT NULL
    DROP VIEW [dbo].[vw_Admin_BorrowStats];
EXEC(N'
CREATE VIEW [dbo].[vw_Admin_BorrowStats]
AS
SELECT
    CAST((SELECT COUNT(*) FROM dbo.tbl_Borrow_Detail) AS int) AS TotalLifetimeBorrows,
    CAST((SELECT COUNT(*) FROM dbo.tbl_Borrow_Detail WHERE [Status] = N''Borrowing'') AS int) AS CurrentlyBorrowed,
    CAST((SELECT COUNT(*) FROM dbo.tbl_Borrow_Detail WHERE [Status] = N''Due'') AS int) AS TotalOverdue,
    CAST((SELECT COUNT(*) FROM dbo.tbl_Borrow_Detail WHERE [Status] = N''Returned'') AS int) AS TotalReturned,
    CAST((SELECT COUNT(*) FROM dbo.tbl_Borrow_Detail WHERE [Status] = N''Lost'') AS int) AS TotalLost
');

IF OBJECT_ID(N'[dbo].[vw_Admin_Financials]', N'V') IS NOT NULL
    DROP VIEW [dbo].[vw_Admin_Financials];
EXEC(N'
CREATE VIEW [dbo].[vw_Admin_Financials]
AS
SELECT
    CAST(ISNULL((SELECT SUM(Amount) FROM dbo.tbl_Fine WHERE [Status] = N''Paid''), 0) AS bigint) AS TotalRevenueCollected,
    CAST(ISNULL((SELECT SUM(Amount) FROM dbo.tbl_Fine WHERE [Status] = N''Unpaid''), 0) AS bigint) AS PendingRevenue
');

IF OBJECT_ID(N'[dbo].[tbl_Book_Inventory]', N'V') IS NOT NULL
    DROP VIEW [dbo].[tbl_Book_Inventory];
EXEC(N'
CREATE VIEW [dbo].[tbl_Book_Inventory]
AS
SELECT BookID, BookTitle, Quantity, [Value]
FROM dbo.tbl_Book
');

IF OBJECT_ID(N'[dbo].[sp_LogIn]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_LogIn];
EXEC(N'
CREATE PROCEDURE [dbo].[sp_LogIn]
    @AccountType NVARCHAR(100),
    @Username NVARCHAR(100),
    @Password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF @AccountType = N''Admin''
    BEGIN
        SELECT TOP 1
            a.AdminID AS AccountID,
            a.FullName,
            CAST(N''Admin'' AS nvarchar(5)) AS RoleType
        FROM dbo.tbl_Admin_Account a
        WHERE a.Username = @Username
          AND a.[Password] = @Password
          AND ISNULL(a.IsActive, 1) = 1;
        RETURN;
    END;

    IF @AccountType = N''Thủ thư'' OR @AccountType = N''Librarian''
    BEGIN
        SELECT TOP 1
            l.LibrarianID AS AccountID,
            l.FullName,
            CAST(N''Librarian'' AS nvarchar(10)) AS RoleType
        FROM dbo.tbl_Librarian_Account l
        WHERE l.Username = @Username
          AND l.[Password] = @Password
          AND ISNULL(l.IsActive, 1) = 1;
        RETURN;
    END;

    SELECT TOP 1
        r.ReaderID AS AccountID,
        CAST(LTRIM(RTRIM(r.LastName + N'' '' + r.FirstName)) AS nvarchar(100)) AS FullName,
        CAST(N''Reader'' AS nvarchar(10)) AS RoleType
    FROM dbo.tbl_Reader_Account r
    WHERE CAST(r.ReaderID AS nvarchar(100)) = @Username
      AND r.[Password] = @Password
      AND ISNULL(r.[Status], N''Hoạt động'') <> N''Khóa'';
END
');

IF OBJECT_ID(N'[dbo].[sp_Admin_GetActivityLogs]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Admin_GetActivityLogs];
EXEC(N'
CREATE PROCEDURE [dbo].[sp_Admin_GetActivityLogs]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        LogID,
        CONVERT(varchar(19), [Timestamp], 120) AS [Time],
        ActorRole AS [Role],
        ActorID AS [ID],
        LogMessage AS [Nội dung hoạt động]
    FROM dbo.tbl_ActivityLog
    ORDER BY LogID DESC;
END
');

IF OBJECT_ID(N'[dbo].[sp_Admin_AddNewLibrarian]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Admin_AddNewLibrarian];
EXEC(N'
CREATE PROCEDURE [dbo].[sp_Admin_AddNewLibrarian]
    @Username varchar(50),
    @Password varchar(255),
    @FullName nvarchar(100),
    @Branch nvarchar(100),
    @AdminID int
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.tbl_Librarian_Account WHERE Username = @Username)
    BEGIN
        SELECT CAST(N''error'' AS nvarchar(5)) AS [status], CAST(N''Username existed'' AS nvarchar(25)) AS [message];
        RETURN;
    END;

    INSERT INTO dbo.tbl_Librarian_Account (Username, [Password], FullName, Branch, IsActive)
    VALUES (@Username, @Password, @FullName, @Branch, 1);

    INSERT INTO dbo.tbl_ActivityLog (ActorRole, ActorID, LogMessage)
    VALUES (''Admin'', ISNULL(@AdminID, 0), N''Created librarian: '' + @Username);

    SELECT CAST(N''success'' AS nvarchar(5)) AS [status], CAST(N''Librarian created'' AS nvarchar(25)) AS [message];
END
');

IF OBJECT_ID(N'[dbo].[sp_AddNewBook]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_AddNewBook];
EXEC(N'
CREATE PROCEDURE [dbo].[sp_AddNewBook]
    @Language nvarchar(50) = NULL,
    @BookTitle nvarchar(200),
    @AuthorID varchar(10) = NULL,
    @PublisherID varchar(10) = NULL,
    @CategoryID int = NULL,
    @PublishedDate date = NULL,
    @Quantity int = NULL,
    @Value bigint = NULL,
    @LibrarianID int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @BookID varchar(15) =
        ''B'' + RIGHT(REPLACE(CONVERT(varchar(36), NEWID()), ''-'', ''''), 14);

    INSERT INTO dbo.tbl_Book (BookID, [Language], BookTitle, AuthorID, PublisherID, CategoryID, PublishedDate, Quantity, [Value])
    VALUES (@BookID, @Language, @BookTitle, @AuthorID, @PublisherID, @CategoryID, @PublishedDate, ISNULL(@Quantity, 0), ISNULL(@Value, 0));

    INSERT INTO dbo.tbl_ActivityLog (ActorRole, ActorID, LogMessage)
    VALUES (''Librarian'', ISNULL(@LibrarianID, 0), N''Added book: '' + @BookTitle);
END
');

IF OBJECT_ID(N'[dbo].[sp_CreateReaderAccount]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_CreateReaderAccount];
EXEC(N'
CREATE PROCEDURE [dbo].[sp_CreateReaderAccount]
    @FirstName nvarchar(50),
    @LastName nvarchar(50),
    @DOB date = NULL,
    @Organization nvarchar(100) = NULL,
    @Status nvarchar(50) = NULL,
    @Role nvarchar(50),
    @LibrarianID int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.tbl_Reader_Account (FirstName, LastName, DOB, Organization, [Status], [Role])
    VALUES (@FirstName, @LastName, @DOB, @Organization, ISNULL(@Status, N''Hoạt động''), @Role);

    INSERT INTO dbo.tbl_ActivityLog (ActorRole, ActorID, LogMessage)
    VALUES (''Librarian'', ISNULL(@LibrarianID, 0), N''Created reader: '' + @LastName + N'' '' + @FirstName);
END
');

IF OBJECT_ID(N'[dbo].[sp_Reader_CreatePendingOrder]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Reader_CreatePendingOrder];
EXEC(N'
CREATE PROCEDURE [dbo].[sp_Reader_CreatePendingOrder]
    @ReaderID int,
    @DaysToBorrow int,
    @BookList nvarchar(max),
    @Notes nvarchar(max) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.tbl_Reader_Account WHERE ReaderID = @ReaderID)
    BEGIN
        SELECT CAST(N''error'' AS nvarchar(5)) AS [status], CAST(N''Reader not found'' AS nvarchar(64)) AS [message];
        RETURN;
    END;

    DECLARE @OrderID int;

    INSERT INTO dbo.tbl_Borrow_Order (ReaderID, DateBorrow, DateReturn, Notes, OrderStatus)
    VALUES (@ReaderID, CAST(GETDATE() AS date), DATEADD(day, ISNULL(@DaysToBorrow, 0), CAST(GETDATE() AS date)), @Notes, N''Pending'');

    SET @OrderID = SCOPE_IDENTITY();

    INSERT INTO dbo.tbl_Borrow_Detail (OrderID, BookID, [Status])
    SELECT
        @OrderID,
        LTRIM(RTRIM([value])),
        N''Pending''
    FROM STRING_SPLIT(ISNULL(@BookList, N''''), '','')
    WHERE LTRIM(RTRIM([value])) <> N'''';

    SELECT CAST(N''success'' AS nvarchar(5)) AS [status], CAST(N''Pending order created'' AS nvarchar(64)) AS [message];
END
');

IF OBJECT_ID(N'[dbo].[sp_Librarian_ApproveOrder]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Librarian_ApproveOrder];
EXEC(N'
CREATE PROCEDURE [dbo].[sp_Librarian_ApproveOrder]
    @OrderID int,
    @LibrarianID int
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.tbl_Borrow_Order WHERE OrderID = @OrderID)
    BEGIN
        SELECT CAST(N''error'' AS nvarchar(5)) AS [status], CAST(N''Order not found'' AS nvarchar(57)) AS [message];
        RETURN;
    END;

    UPDATE dbo.tbl_Borrow_Order
    SET OrderStatus = N''Borrowing''
    WHERE OrderID = @OrderID;

    UPDATE dbo.tbl_Borrow_Detail
    SET [Status] = N''Borrowing''
    WHERE OrderID = @OrderID;

    INSERT INTO dbo.tbl_ActivityLog (ActorRole, ActorID, LogMessage)
    VALUES (''Librarian'', ISNULL(@LibrarianID, 0), N''Approved order #'' + CAST(@OrderID AS nvarchar(20)));

    SELECT CAST(N''success'' AS nvarchar(5)) AS [status], CAST(N''Order approved'' AS nvarchar(57)) AS [message];
END
');

IF OBJECT_ID(N'[dbo].[sp_Accounting_PayFine]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Accounting_PayFine];
EXEC(N'
CREATE PROCEDURE [dbo].[sp_Accounting_PayFine]
    @FineID int,
    @LibrarianID int
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.tbl_Fine WHERE FineID = @FineID)
    BEGIN
        SELECT CAST(N''error'' AS nvarchar(7)) AS [status], CAST(N''Fine not found'' AS nvarchar(63)) AS [message];
        RETURN;
    END;

    UPDATE dbo.tbl_Fine
    SET [Status] = N''Paid''
    WHERE FineID = @FineID;

    INSERT INTO dbo.tbl_ActivityLog (ActorRole, ActorID, LogMessage)
    VALUES (''Librarian'', ISNULL(@LibrarianID, 0), N''Collected fine #'' + CAST(@FineID AS nvarchar(20)));

    SELECT CAST(N''success'' AS nvarchar(7)) AS [status], CAST(N''Fine collected successfully'' AS nvarchar(63)) AS [message];
END
');
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[sp_Accounting_PayFine]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Accounting_PayFine];
IF OBJECT_ID(N'[dbo].[sp_Librarian_ApproveOrder]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Librarian_ApproveOrder];
IF OBJECT_ID(N'[dbo].[sp_Reader_CreatePendingOrder]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Reader_CreatePendingOrder];
IF OBJECT_ID(N'[dbo].[sp_CreateReaderAccount]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_CreateReaderAccount];
IF OBJECT_ID(N'[dbo].[sp_AddNewBook]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_AddNewBook];
IF OBJECT_ID(N'[dbo].[sp_Admin_AddNewLibrarian]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Admin_AddNewLibrarian];
IF OBJECT_ID(N'[dbo].[sp_Admin_GetActivityLogs]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Admin_GetActivityLogs];
IF OBJECT_ID(N'[dbo].[sp_LogIn]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_LogIn];
IF OBJECT_ID(N'[dbo].[tbl_Book_Inventory]', N'V') IS NOT NULL
    DROP VIEW [dbo].[tbl_Book_Inventory];
IF OBJECT_ID(N'[dbo].[vw_Admin_Financials]', N'V') IS NOT NULL
    DROP VIEW [dbo].[vw_Admin_Financials];
IF OBJECT_ID(N'[dbo].[vw_Admin_BorrowStats]', N'V') IS NOT NULL
    DROP VIEW [dbo].[vw_Admin_BorrowStats];
IF OBJECT_ID(N'[dbo].[vw_Book_Public]', N'V') IS NOT NULL
    DROP VIEW [dbo].[vw_Book_Public];
");

            migrationBuilder.DropTable(
                name: "tbl_ActivityLog");

            migrationBuilder.DropTable(
                name: "tbl_Admin_Account");

            migrationBuilder.DropTable(
                name: "tbl_Attendance");

            migrationBuilder.DropTable(
                name: "tbl_Fine");

            migrationBuilder.DropTable(
                name: "tbl_Librarian_Account");

            migrationBuilder.DropTable(
                name: "tbl_Borrow_Detail");

            migrationBuilder.DropTable(
                name: "tbl_Book");

            migrationBuilder.DropTable(
                name: "tbl_Borrow_Order");

            migrationBuilder.DropTable(
                name: "tbl_Author");

            migrationBuilder.DropTable(
                name: "tbl_Category");

            migrationBuilder.DropTable(
                name: "tbl_Publisher");

            migrationBuilder.DropTable(
                name: "tbl_Reader_Account");

            migrationBuilder.DropSequence(
                name: "seq_Author");

            migrationBuilder.DropSequence(
                name: "seq_Publisher");
        }
    }
}
