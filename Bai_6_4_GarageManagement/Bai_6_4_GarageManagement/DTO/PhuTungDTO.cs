namespace Bai_6_4_GarageManagement.DTO
{
    public class PhuTungDTO
    {
        public int MaPhuTung { get; set; }
        public string TenPhuTung { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuongTon { get; set; }

        public override string ToString()
        {
            return $"{TenPhuTung} (Giá: {DonGia:N0}đ | Tồn: {SoLuongTon})";
        }
    }
}