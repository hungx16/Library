using _3NF_Library.Models.Entities;

namespace _3NF_Library.Web.Models
{
    public class AdminDashboardVM
    {
        // Chứa dữ liệu thống kê mượn sách
        public vw_Admin_BorrowStat? BorrowStats { get; set; }

        // Chứa dữ liệu tài chính
        public vw_Admin_Financial? FinancialStats { get; set; }
    }
}