using _3NF_Library.Data.Context;
using _3NF_Library.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Data;
using Microsoft.Data.SqlClient;

namespace _3NF_Library.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryDbContext _context;
        public HomeController(LibraryDbContext context) { _context = context; }

        // --- 1. TRANG CHỦ (ĐÃ CHẠY TỐT) ---
        public async Task<IActionResult> Index()
        {
            return View(await _context.vw_Book_Publics.AsNoTracking().ToListAsync());
        }

        [Authorize(Roles = "Librarian")]
        public IActionResult LibrarianDashboard() => View();

        // --- 2. QUẢN LÝ KHO SÁCH (HÀM ĐÚNG ĐÃ CHẠY ĐƯỢC CỦA BẠN) ---
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> ManageBooks(string search, int? categoryId)
        {
            // Lấy danh sách từ View (giống hệt trang Index)
            var books = _context.vw_Book_Publics.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(b => b.BookTitle.Contains(search) || b.AuthorName.Contains(search));
            }
            if (categoryId.HasValue)
            {
                books = books.Where(b => b.CategoryID == categoryId.Value);
            }

            ViewBag.CategoryStats = await _context.tbl_Categories.AsNoTracking().ToListAsync();
            ViewBag.Authors = await _context.tbl_Authors.AsNoTracking().ToListAsync();
            ViewBag.Publishers = await _context.tbl_Publishers.AsNoTracking().ToListAsync();

            // Trả về List vw_Book_Public nguyên bản để đảm bảo trang luôn load được
            return View(await books.ToListAsync());
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> UpdateBook(string bookId, string title, int qty, long val)
        {
            try
            {
                // Sử dụng bảng tbl_Book_Inventory (số ít) từ đoạn code chạy được của bạn
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE tbl_Book_Inventory SET BookTitle = @title, Quantity = @qty, Value = @val WHERE BookID = @id",
                    new SqlParameter("@title", title), new SqlParameter("@qty", qty), new SqlParameter("@val", val), new SqlParameter("@id", bookId));
                TempData["SuccessMessage"] = "Cập nhật thành công!";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("ManageBooks");
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> AddBook(string title, string lang, string authorId, string pubId, int catId, DateTime pubDate, int qty, long val)
        {
            int libId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                await _context.Database.OpenConnectionAsync();
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_AddNewBook"; cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Language", lang)); cmd.Parameters.Add(new SqlParameter("@BookTitle", title));
                    cmd.Parameters.Add(new SqlParameter("@AuthorID", authorId)); cmd.Parameters.Add(new SqlParameter("@PublisherID", pubId));
                    cmd.Parameters.Add(new SqlParameter("@CategoryID", catId)); cmd.Parameters.Add(new SqlParameter("@PublishedDate", pubDate));
                    cmd.Parameters.Add(new SqlParameter("@Quantity", qty)); cmd.Parameters.Add(new SqlParameter("@Value", val));
                    cmd.Parameters.Add(new SqlParameter("@LibrarianID", libId));
                    await cmd.ExecuteNonQueryAsync();
                }
                TempData["SuccessMessage"] = $"Đã thêm sách '{title}' thành công!";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            finally { await _context.Database.CloseConnectionAsync(); }
            return RedirectToAction("ManageBooks");
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> QuickAddCategory(string name, string shortName)
        {
            try
            {
                int newId = 0;
                // Xử lý tự động: Nếu quên nhập tên ngắn thì lấy tên dài, và đảm bảo không vượt quá 10 ký tự (theo chuẩn VARCHAR(10) của DB)
                string safeShortName = string.IsNullOrEmpty(shortName) ? name : shortName;
                if (safeShortName.Length > 10) safeShortName = safeShortName.Substring(0, 10);

                await _context.Database.OpenConnectionAsync();
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    // Đã thêm cột ShortName vào lệnh INSERT
                    cmd.CommandText = "INSERT INTO tbl_Category (CategoryName, ShortName) OUTPUT INSERTED.CategoryID VALUES (@name, @shortName)";
                    cmd.Parameters.Add(new SqlParameter("@name", name));
                    cmd.Parameters.Add(new SqlParameter("@shortName", safeShortName));

                    newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
                return Json(new { success = true, id = newId, text = name });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
            finally { await _context.Database.CloseConnectionAsync(); }
        }

        //ĐÃ CHẠY TỐT THÊM TÁC GIẢ 
        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> QuickAddAuthor(string lastName, string firstName, DateTime? dob, string specialty, string description)
        {
            try
            {
                string newId = "";
                await _context.Database.OpenConnectionAsync();
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    // Đã đổi thành tbl_Author
                    cmd.CommandText = "INSERT INTO tbl_Author (FirstName, LastName) OUTPUT INSERTED.AuthorID VALUES (@first, @last)";
                    cmd.Parameters.Add(new SqlParameter("@first", firstName));
                    cmd.Parameters.Add(new SqlParameter("@last", lastName));
                    var result = await cmd.ExecuteScalarAsync();
                    newId = result?.ToString() ?? "";
                }
                return Json(new { success = true, id = newId, text = $"{lastName} {firstName}" });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
            finally { await _context.Database.CloseConnectionAsync(); }
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> QuickAddPublisher(string name, string address)
        {
            try
            {
                string newId = "";
                await _context.Database.OpenConnectionAsync();
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    // SỬA QUAN TRỌNG: 
                    // 1. Tên NXB được Insert vào cột [Description] cho khớp với Data.sql
                    // 2. Không cần chèn PublisherID vì DB của bạn đã dùng SEQUENCE tự động sinh (DEFAULT NEXT VALUE)
                    cmd.CommandText = "INSERT INTO tbl_Publisher (Description, Address) OUTPUT INSERTED.PublisherID VALUES (@name, @address)";
                    cmd.Parameters.Add(new SqlParameter("@name", name));
                    cmd.Parameters.Add(new SqlParameter("@address", (object)address ?? DBNull.Value));

                    var result = await cmd.ExecuteScalarAsync();
                    newId = result?.ToString() ?? "";
                }
                return Json(new { success = true, id = newId, text = name });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
            finally { await _context.Database.CloseConnectionAsync(); }
        }


        // --- 3. QUẢN LÝ ĐỘC GIẢ (GIỮ NGUYÊN) ---
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> ManageReaders()
        {
            string query = @"SELECT r.ReaderID, r.FirstName, r.LastName, r.DOB, r.Organization, r.Role, r.Phone, r.Status, 
                (SELECT COUNT(*) FROM tbl_Borrow_Detail bd JOIN tbl_Borrow_Order o ON bd.OrderID = o.OrderID WHERE o.ReaderID = r.ReaderID AND bd.Status = 'Borrowing') as BorrowingCount 
                FROM tbl_Reader_Account r";
            var readers = new List<dynamic>();
            await _context.Database.OpenConnectionAsync();
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = query;
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        readers.Add(new { ID = (int)reader["ReaderID"], FirstName = reader["FirstName"].ToString(), LastName = reader["LastName"].ToString(), DOB = reader["DOB"] != DBNull.Value ? ((DateTime)reader["DOB"]).ToString("yyyy-MM-dd") : "", Organization = reader["Organization"]?.ToString() ?? "", Role = reader["Role"]?.ToString() ?? "", Phone = reader["Phone"]?.ToString() ?? "", Status = reader["Status"]?.ToString() ?? "Hoạt động", Borrowing = (int)reader["BorrowingCount"], Overdue = 0 });
                    }
                }
            }
            await _context.Database.CloseConnectionAsync();
            return View(readers);
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet] public IActionResult AddReader() => View();

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> AddReader(string ho, string ten, DateTime ngaySinh, string toChuc, string status, string chucVu)
        {
            int libId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                await _context.Database.OpenConnectionAsync();
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_CreateReaderAccount"; cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@FirstName", ten)); cmd.Parameters.Add(new SqlParameter("@LastName", ho));
                    cmd.Parameters.Add(new SqlParameter("@DOB", ngaySinh)); cmd.Parameters.Add(new SqlParameter("@Organization", toChuc));
                    cmd.Parameters.Add(new SqlParameter("@Status", status)); cmd.Parameters.Add(new SqlParameter("@Role", chucVu));
                    cmd.Parameters.Add(new SqlParameter("@LibrarianID", libId));
                    await cmd.ExecuteNonQueryAsync();
                }
                TempData["SuccessMessage"] = "Đã tạo độc giả thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex) { ViewBag.Error = ex.Message; return View(); }
            finally { await _context.Database.CloseConnectionAsync(); }
        }
        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> UpdateReader(int id, string ho, string ten, DateTime ngaySinh, string phone, string toChuc, string chucVu, string status)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE tbl_Reader_Account SET FirstName = @ten, LastName = @ho, DOB = @dob, Phone = @phone, Organization = @tochuc, Role = @role, Status = @status WHERE ReaderID = @id",
                    new SqlParameter("@ten", ten), new SqlParameter("@ho", ho), new SqlParameter("@dob", ngaySinh),
                    new SqlParameter("@phone", (object)phone ?? DBNull.Value), new SqlParameter("@tochuc", (object)toChuc ?? DBNull.Value),
                    new SqlParameter("@role", chucVu), new SqlParameter("@status", status), new SqlParameter("@id", id));
                TempData["SuccessMessage"] = "Cập nhật độc giả thành công!";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("ManageReaders");
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> LockReader(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE tbl_Reader_Account SET Status = N'Khóa' WHERE ReaderID = @id",
                    new SqlParameter("@id", id));
                TempData["SuccessMessage"] = "Đã khóa tài khoản độc giả!";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("ManageReaders");
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> UnlockReader(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE tbl_Reader_Account SET Status = N'Hoạt động' WHERE ReaderID = @id",
                    new SqlParameter("@id", id));
                TempData["SuccessMessage"] = "Đã mở khóa tài khoản thành công!";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("ManageReaders");
        }

        // --- 4. MƯỢN - TRẢ (GIỮ NGUYÊN) ---
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> BorrowReturn()
        {
            var pendingOrders = new List<dynamic>(); var borrowingOrders = new List<dynamic>();
            await _context.Database.OpenConnectionAsync();
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = @"SELECT o.OrderID, (r.LastName + ' ' + r.FirstName) as ReaderName, o.OrderStatus, o.DateReturn, (SELECT STRING_AGG(bd.BookID, ', ') FROM tbl_Borrow_Detail bd WHERE bd.OrderID = o.OrderID) as BookList FROM tbl_Borrow_Order o JOIN tbl_Reader_Account r ON o.ReaderID = r.ReaderID WHERE o.OrderStatus IN ('Pending', 'Borrowing')";
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var item = new { OrderID = (int)reader["OrderID"], ReaderName = reader["ReaderName"].ToString(), Deadline = ((DateTime)reader["DateReturn"]).ToString("dd/MM/yyyy"), BookList = reader["BookList"]?.ToString() ?? "", Status = reader["OrderStatus"].ToString() };
                        if (item.Status == "Pending") pendingOrders.Add(item); else borrowingOrders.Add(item);
                    }
                }
            }
            await _context.Database.CloseConnectionAsync();
            ViewBag.PendingOrders = pendingOrders; ViewBag.BorrowingOrders = borrowingOrders;
            ViewBag.Readers = await _context.tbl_Reader_Accounts.AsNoTracking().Where(r => r.Status == "Hoạt động").ToListAsync();
            return View();
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> CreateBorrowOrder(int readerId, DateTime returnDate, string bookIds, string notes)
        {
            int days = (int)(returnDate.Date - DateTime.Now.Date).TotalDays;
            try
            {
                await _context.Database.OpenConnectionAsync();
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_Reader_CreatePendingOrder"; cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ReaderID", readerId)); cmd.Parameters.Add(new SqlParameter("@DaysToBorrow", days));
                    cmd.Parameters.Add(new SqlParameter("@BookList", bookIds)); cmd.Parameters.Add(new SqlParameter("@Notes", notes ?? ""));
                    await cmd.ExecuteNonQueryAsync();
                }
                TempData["SuccessMessage"] = "Đã tạo đơn chờ!";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            finally { await _context.Database.CloseConnectionAsync(); }
            return RedirectToAction("BorrowReturn");
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> ApproveOrder(int orderId)
        {
            int libId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                await _context.Database.OpenConnectionAsync();
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = @"EXEC sp_Librarian_ApproveOrder @ID, @LibID; UPDATE tbl_Borrow_Detail SET Status = 'Borrowing' WHERE OrderID = @ID;";
                    cmd.Parameters.Add(new SqlParameter("@ID", orderId)); cmd.Parameters.Add(new SqlParameter("@LibID", libId));
                    await cmd.ExecuteNonQueryAsync();
                }
                TempData["SuccessMessage"] = "Đã duyệt đơn!";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            finally { await _context.Database.CloseConnectionAsync(); }
            return RedirectToAction("BorrowReturn");
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> RejectOrder(int orderId)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("BEGIN TRANSACTION; DELETE FROM tbl_Borrow_Detail WHERE OrderID = @id; DELETE FROM tbl_Borrow_Order WHERE OrderID = @id; COMMIT;", new SqlParameter("@id", orderId));
                TempData["SuccessMessage"] = "Đã xóa đơn!";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            return RedirectToAction("BorrowReturn");
        }

        //XÁC NHẬN TRẢ SÁCH (NẾU TRỄ SẼ TỰ ĐỘNG CHỐT TIỀN PHẠT) 
        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> ConfirmReturn(int orderId)
        {
            int libId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                await _context.Database.OpenConnectionAsync();
                using (var transaction = await _context.Database.GetDbConnection().BeginTransactionAsync())
                {
                    try
                    {
                        int daysLate = 0;
                        // Tính số ngày trễ
                        using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                        {
                            cmd.Transaction = transaction;
                            cmd.CommandText = "SELECT DATEDIFF(day, DateReturn, GETDATE()) FROM tbl_Borrow_Order WHERE OrderID = @id";
                            cmd.Parameters.Add(new SqlParameter("@id", orderId));
                            var result = await cmd.ExecuteScalarAsync();
                            daysLate = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                        }

                        // Nếu trễ, chốt luôn Bill phạt (Mỗi cuốn chưa mất * 4000đ * số ngày trễ)
                        if (daysLate > 0)
                        {
                            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                            {
                                cmd.Transaction = transaction;
                                cmd.CommandText = @"
                            INSERT INTO tbl_Fine (DetailID, Reason, Amount, Status)
                            SELECT DetailID, N'Trễ hạn ' + CAST(@days AS NVARCHAR) + N' ngày', @fine, 'Unpaid'
                            FROM tbl_Borrow_Detail 
                            WHERE OrderID = @id AND Status IN ('Borrowing', 'Due');";
                                cmd.Parameters.Add(new SqlParameter("@id", orderId));
                                cmd.Parameters.Add(new SqlParameter("@days", daysLate));
                                cmd.Parameters.Add(new SqlParameter("@fine", daysLate * 4000));
                                await cmd.ExecuteNonQueryAsync();
                            }
                            TempData["SuccessMessage"] = $"Khách trả trễ {daysLate} ngày!\nHệ thống đã tự động chốt hóa đơn phạt (4,000đ/ngày) vào mục Vi Phạm.";
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "Khách trả sách đúng hạn. Giao dịch hoàn tất!";
                        }

                        // Cập nhật đơn thành Returned và Lưu log
                        using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                        {
                            cmd.Transaction = transaction;
                            cmd.CommandText = @"
                        UPDATE tbl_Borrow_Order SET OrderStatus = 'Returned' WHERE OrderID = @id; 
                        UPDATE tbl_Borrow_Detail SET Status = 'Returned' WHERE OrderID = @id AND Status IN ('Borrowing', 'Due');
                        INSERT INTO tbl_ActivityLog (ActorRole, ActorID, LogMessage) VALUES ('Librarian', @libId, N'Xác nhận trả đơn #' + CAST(@id AS VARCHAR));";
                            cmd.Parameters.Add(new SqlParameter("@id", orderId));
                            cmd.Parameters.Add(new SqlParameter("@libId", libId));
                            await cmd.ExecuteNonQueryAsync();
                        }

                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        TempData["ErrorMessage"] = "Lỗi Database: " + ex.Message;
                    }
                }
            }
            catch (Exception ex) { TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message; }
            finally { await _context.Database.CloseConnectionAsync(); }

            return RedirectToAction("BorrowReturn");
        }
        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> GetBooksForOrder(int orderId)
        {
            var books = new List<dynamic>();
            await _context.Database.OpenConnectionAsync();
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                // Chỉ lấy những sách chưa trả (Borrowing hoặc Due)
                cmd.CommandText = @"SELECT d.BookID, b.BookTitle 
                            FROM tbl_Borrow_Detail d 
                            JOIN tbl_Book b ON d.BookID = b.BookID 
                            WHERE d.OrderID = @id AND d.Status IN ('Borrowing', 'Due')";
                cmd.Parameters.Add(new SqlParameter("@id", orderId));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        books.Add(new { id = reader["BookID"].ToString(), title = reader["BookTitle"].ToString() });
                    }
                }
            }
            await _context.Database.CloseConnectionAsync();
            return Json(books);
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> ProcessLostBooks(int orderId, List<string> lostBooks)
        {
            if (lostBooks == null || !lostBooks.Any())
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất 1 cuốn sách bị mất!";
                return RedirectToAction("BorrowReturn");
            }

            int libId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            long totalFine = 0;

            try
            {
                await _context.Database.OpenConnectionAsync();

                // Dùng Transaction của C# để kiểm soát an toàn
                using (var transaction = await _context.Database.GetDbConnection().BeginTransactionAsync())
                {
                    try
                    {
                        // 1. Lấy chức vụ độc giả
                        string role = "";
                        using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                        {
                            cmd.Transaction = transaction;
                            cmd.CommandText = "SELECT r.Role FROM tbl_Borrow_Order o JOIN tbl_Reader_Account r ON o.ReaderID = r.ReaderID WHERE o.OrderID = @id";
                            cmd.Parameters.Add(new SqlParameter("@id", orderId));
                            role = (await cmd.ExecuteScalarAsync())?.ToString() ?? "";
                        }

                        // 2. Tính toán tỷ lệ giảm giá 1 lần (Cho tối ưu hiệu suất)
                        double discountRate = 0;
                        string r = role.ToLower();
                        if (r.Contains("học sinh")) discountRate = 0.05;
                        else if (r.Contains("sinh viên")) discountRate = 0.10;
                        else if (r.Contains("giáo viên") || r.Contains("giảng viên")) discountRate = 0.15;
                        else if (r.Contains("giáo sư")) discountRate = 0.20;

                        // 3. Đổi trạng thái sách thành 'Lost' và tính tiền theo công thức: Giá gốc - Giảm giá
                        foreach (var bookId in lostBooks)
                        {
                            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                            {
                                cmd.Transaction = transaction;
                                cmd.CommandText = "UPDATE tbl_Borrow_Detail SET Status = 'Lost' WHERE OrderID = @oId AND BookID = @bId";
                                cmd.Parameters.Add(new SqlParameter("@oId", orderId));
                                cmd.Parameters.Add(new SqlParameter("@bId", bookId));
                                await cmd.ExecuteNonQueryAsync();
                            }

                            long bookValue = 0;
                            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                            {
                                cmd.Transaction = transaction;
                                // Lấy giá sách, nếu NULL thì tạm tính 100.000 VNĐ
                                cmd.CommandText = "SELECT ISNULL(Value, 100000) FROM tbl_Book WHERE BookID = @bId";
                                cmd.Parameters.Add(new SqlParameter("@bId", bookId));
                                var valObj = await cmd.ExecuteScalarAsync();
                                bookValue = valObj != null ? Convert.ToInt64(valObj) : 100000;
                            }

                            // Công thức chuẩn: Gốc - (Gốc * % Giảm)
                            long fineForThisBook = (long)(bookValue - (bookValue * discountRate));
                            totalFine += fineForThisBook;
                        }

                        // 4. Cắt ngắn chuỗi Lý do (Chống lỗi vỡ 255 ký tự của SQL)
                        string reason = $"Khách làm mất {lostBooks.Count} cuốn sách. (Mã: {string.Join(", ", lostBooks)})";
                        if (reason.Length > 250) reason = reason.Substring(0, 250);

                        // 5. Ghi dữ liệu vào bảng Phạt (tbl_Fine)
                        using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                        {
                            cmd.Transaction = transaction;
                            cmd.CommandText = @"
                DECLARE @dId INT = (SELECT TOP 1 DetailID FROM tbl_Borrow_Detail WHERE OrderID = @oId AND Status = 'Lost');
                INSERT INTO tbl_Fine (DetailID, Reason, Amount, Status) VALUES (@dId, @reason, @fine, 'Unpaid');
                INSERT INTO tbl_ActivityLog (ActorRole, ActorID, LogMessage) VALUES ('Librarian', @libId, N'Đã phạt báo mất sách: ' + CAST(@fine AS VARCHAR) + ' VNĐ');";
                            cmd.Parameters.Add(new SqlParameter("@oId", orderId));
                            cmd.Parameters.Add(new SqlParameter("@reason", reason));
                            cmd.Parameters.Add(new SqlParameter("@fine", totalFine));
                            cmd.Parameters.Add(new SqlParameter("@libId", libId));
                            await cmd.ExecuteNonQueryAsync();
                        }

                        await transaction.CommitAsync();
                        TempData["SuccessMessage"] = $"Xác nhận báo mất thành công!\nTổng tiền phạt: {totalFine:N0} VNĐ";
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        TempData["ErrorMessage"] = "Lỗi xử lý Database: " + ex.Message;
                    }
                }
            }
            catch (Exception ex) { TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message; }
            finally { await _context.Database.CloseConnectionAsync(); }

            return RedirectToAction("BorrowReturn");
        }



        // --- QUẢN LÝ TIỀN PHẠT (MỚI) ---
        [Authorize(Roles = "Librarian,Admin")]
        public async Task<IActionResult> ManageFines()
        {
            var violations = new List<dynamic>();
            await _context.Database.OpenConnectionAsync();

            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                // Đã sửa thành DateBorrow và DateReturn cho khớp 100% với CSDL
                cmd.CommandText = @"
            SELECT 
                o.OrderID,
                ISNULL(r.LastName, '') + ' ' + ISNULL(r.FirstName, '') AS ReaderName,
                ISNULL(r.Phone, 'Chưa cập nhật') AS Phone,
                o.DateBorrow,
                o.DateReturn,
                b.BookTitle,
                f.FineID,
                f.Amount,
                f.Reason,
                bd.Status AS DetailStatus,
                DATEDIFF(day, o.DateReturn, GETDATE()) AS DaysLate
            FROM tbl_Borrow_Detail bd
            JOIN tbl_Borrow_Order o ON bd.OrderID = o.OrderID
            JOIN tbl_Reader_Account r ON o.ReaderID = r.ReaderID
            JOIN tbl_Book b ON bd.BookID = b.BookID
            LEFT JOIN tbl_Fine f ON bd.DetailID = f.DetailID AND f.Status = 'Unpaid'
            WHERE bd.Status = 'Lost' 
               OR (bd.Status IN ('Borrowing', 'Due') AND o.DateReturn < GETDATE())
               OR (f.Status = 'Unpaid')
            ORDER BY o.DateReturn ASC";

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int fineId = reader["FineID"] != DBNull.Value ? (int)reader["FineID"] : 0;
                        long amount = 0;
                        string viPham = "";
                        bool canCollect = false;

                        if (fineId > 0)
                        {
                            // Đã có hóa đơn (Mất sách hoặc Đã trả sách nhưng trễ)
                            amount = Convert.ToInt64(reader["Amount"]);
                            string reason = reader["Reason"].ToString() ?? "";
                            viPham = reason.Contains("mất", StringComparison.OrdinalIgnoreCase) || reason.Contains("Lost") ? "Mất sách" : "Trễ hạn";
                            canCollect = true; // Đã chốt bill, cho phép thu tiền
                        }
                        else
                        {
                            // Sách chưa trả, tính nhẩm tiền phạt (4000đ/ngày) hiển thị cho Thủ thư biết
                            int daysLate = reader["DaysLate"] != DBNull.Value ? (int)reader["DaysLate"] : 0;
                            if (daysLate > 0)
                            {
                                amount = daysLate * 4000;
                                viPham = $"Đang trễ {daysLate} ngày";
                                canCollect = false; // Bắt buộc trả sách xong mới cho thu tiền
                            }
                        }

                        if (amount > 0)
                        {
                            violations.Add(new
                            {
                                OrderID = (int)reader["OrderID"],
                                ReaderName = reader["ReaderName"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                BorrowDate = Convert.ToDateTime(reader["DateBorrow"]).ToString("dd/MM/yyyy"),
                                Deadline = Convert.ToDateTime(reader["DateReturn"]).ToString("dd/MM/yyyy"),
                                BookTitle = reader["BookTitle"].ToString(),
                                Qty = 1,
                                Status = viPham,
                                FineID = fineId,
                                Amount = amount,
                                CanCollect = canCollect
                            });
                        }
                    }
                }
            }
            await _context.Database.CloseConnectionAsync();
            ViewBag.Violations = violations;
            return View();
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> CollectFine(int fineId)
        {
            int libId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                await _context.Database.OpenConnectionAsync();
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_Accounting_PayFine";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@FineID", fineId));
                    cmd.Parameters.Add(new SqlParameter("@LibrarianID", libId));

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string status = reader["status"].ToString() ?? "";
                            string msg = reader["message"].ToString() ?? "";
                            if (status == "success") TempData["SuccessMessage"] = msg;
                            else TempData["ErrorMessage"] = msg;
                        }
                    }
                }
            }
            catch (Exception ex) { TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message; }
            finally { await _context.Database.CloseConnectionAsync(); }

            return RedirectToAction("ManageFines");
        }



        // --- 5. ADMIN (GIỮ NGUYÊN) ---

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetBorrowChartData()
        {
            var labels = new List<string>();
            var borrowData = new List<int>();
            var returnData = new List<int>();
            var newData = new List<int>();

            await _context.Database.OpenConnectionAsync();
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = @"
            WITH Last7Days AS (
                SELECT CAST(GETDATE() AS DATE) AS d, 0 AS n
                UNION ALL
                SELECT DATEADD(day, -1, d), n + 1 FROM Last7Days WHERE n < 6
            )
            SELECT 
                d AS DateDate,
                (SELECT COUNT(*) FROM tbl_Borrow_Order WHERE CAST(DateBorrow AS DATE) = d) AS Borrows,
                (SELECT COUNT(*) FROM tbl_Borrow_Order WHERE OrderStatus = 'Returned' AND CAST(GETDATE() AS DATE) = d) AS Returns,
                (SELECT COUNT(*) FROM tbl_Book WHERE CAST(PublishedDate AS DATE) = d) AS NewBooks
            FROM Last7Days
            ORDER BY DateDate ASC";

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        labels.Add(Convert.ToDateTime(reader["DateDate"]).ToString("dd/MM"));
                        borrowData.Add((int)reader["Borrows"]);
                        returnData.Add((int)reader["Returns"]);
                        newData.Add((int)reader["NewBooks"]);
                    }
                }
            }
            await _context.Database.CloseConnectionAsync();
            return Json(new { labels, borrowData, returnData, newData });
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var vm = new AdminDashboardVM
            {
                BorrowStats = (await _context.vw_Admin_BorrowStats.AsNoTracking().ToListAsync()).FirstOrDefault(),
                FinancialStats = (await _context.vw_Admin_Financials.AsNoTracking().ToListAsync()).FirstOrDefault()
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivityLogs()
        {
            return View(await _context.Procedures.sp_Admin_GetActivityLogsAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet] public IActionResult AddLibrarian() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddLibrarian(string username, string password, string fullName, string branch)
        {
            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _context.Procedures.sp_Admin_AddNewLibrarianAsync(username, password, fullName, branch, adminId);
            if (result != null && result.Count > 0 && result[0].status == "success")
            {
                TempData["SuccessMessage"] = result[0].message;
                return RedirectToAction("AdminDashboard");
            }
            ViewBag.Error = result?[0].message;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    
    }




}