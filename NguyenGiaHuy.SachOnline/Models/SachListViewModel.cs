using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguyenGiaHuy.SachOnline.Models
{
    public class SachListViewModel
    {
        public IEnumerable<SACH> Saches { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }

}