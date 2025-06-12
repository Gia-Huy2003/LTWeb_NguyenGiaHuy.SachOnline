using System;
using System.Linq;

namespace NguyenGiaHuy.SachOnline.Models
{
    public partial class GioHang
    {
        SachOnline1Entities1 db = new SachOnline1Entities1();

        public int iSachID { get; set; }
        public string sTenSach { get; set; }
        public string sAnhSP { get; set; }
        public double dGiaTien { get; set; }
        public int iSoLuong { get; set; }

        public double dTongTien
        {
            get { return iSoLuong * dGiaTien; }
        }

        public GioHang(int ms)
        {
            iSachID = ms;

            SACH s = db.SACHes.Single(n => n.SachID == iSachID);

            sTenSach = s.TenSach;
            sAnhSP = s.anhSP;
            dGiaTien = s.GiaBan ?? 0;  // Dùng null-coalescing operator vì GiaBan là nullable
            iSoLuong = 1;
        }
    }
}
