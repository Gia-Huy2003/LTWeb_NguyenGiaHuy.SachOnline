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

        public GioHang(int maSach)
        {
            SACH s = db.SACHes.Single(n => n.SachID == maSach);
            iSachID = maSach;
            sTenSach = s.TenSach;
            sAnhSP = s.anhSP;
            iSoLuong = 1;

            // ✅ Ưu tiên giá khuyến mãi nếu có
            if (s.GiaKhuyenMai.HasValue && s.GiaKhuyenMai < s.GiaBan)
                dGiaTien = s.GiaKhuyenMai.Value;
            else
                dGiaTien = s.GiaBan ?? 0;
        }
    }
}
