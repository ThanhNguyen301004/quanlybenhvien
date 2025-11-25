namespace QUANLYBENHVIEN
{
    using System;
    using System.Collections.Generic;
    
    public partial class BACSI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BACSI()
        {
            this.LICHHENs = new HashSet<LICHHEN>();
        }
    
        public int MaBacSi { get; set; }
        public string HoTen { get; set; }
        public string ChuyenKhoa { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public byte[] AnhDaiDien { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LICHHEN> LICHHENs { get; set; }
    }
}
