//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NguyenGiaHuy.SachOnline.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CHITIETDATHANG
    {
        public int ChiTietDatHangID { get; set; }
        public Nullable<int> DonDatHangID { get; set; }
        public Nullable<int> SachID { get; set; }
        public Nullable<int> SoLuong { get; set; }
        public Nullable<double> GiaTien { get; set; }
    
        public virtual DONDATHANG DONDATHANG { get; set; }
        public virtual SACH SACH { get; set; }
    }
}
