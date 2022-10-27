//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoUongOnline.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SanPham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SanPham()
        {
            this.ChiTietDonHang = new HashSet<ChiTietDonHang>();
            this.CongThuc = new HashSet<CongThuc>();
        }
    
        public int IdSP { get; set; }
        public string TenSP { get; set; }
        public string HinhAnh { get; set; }
        public string MoTa { get; set; }
        public double GiaBan { get; set; }
        public bool TinhTrang { get; set; }
        public string IdLoaiSP { get; set; }
        public Nullable<int> IdKM { get; set; }
        public Nullable<int> SoLuongCon { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietDonHang> ChiTietDonHang { get; set; }
        public virtual KhuyenMai KhuyenMai { get; set; }
        public virtual LoaiSanPham LoaiSanPham { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CongThuc> CongThuc { get; set; }
    }
}