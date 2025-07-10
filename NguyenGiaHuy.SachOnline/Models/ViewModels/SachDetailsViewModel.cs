using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenGiaHuy.SachOnline.Models.ViewModels
{
    public class SachDetailsViewModel
    {
        public SACH Sach { get; set; }
        public string TenChuDe { get; set; }
        public string TenNXB { get; set; }
    }
}
