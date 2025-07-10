using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguyenGiaHuy.SachOnline.Models
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }     // Tổng số sách
        public int ItemsPerPage { get; set; }   // Sách mỗi trang
        public int CurrentPage { get; set; }    // Trang hiện tại
        public Func<int, string> UrlPage { get; set; }
        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
        }
    }
}
