using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguyenGiaHuy.SachOnline.Models
{
    public class ChuDeSachViewModel
    {
        public CHUDE ChuDe { get; set; }
        public List<SACH> Saches { get; set; }
    }
}