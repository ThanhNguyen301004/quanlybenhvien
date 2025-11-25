namespace QUANLYBENHVIEN
{
    using System;
    using System.Collections.Generic;
    
    public partial class LICHHEN
    {
        public int MaLichHen { get; set; }
        public Nullable<int> MaBenhNhan { get; set; }
        public Nullable<int> MaBacSi { get; set; }
        public System.DateTime NgayHen { get; set; }
        public string MucDich { get; set; }
        public string TrangThai { get; set; }
        public string Email { get; set; }
    
        public virtual BACSI BACSI { get; set; }
        public virtual BENHNHAN BENHNHAN { get; set; }
    }
}
