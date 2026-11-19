namespace Bai_6_5_KaraokeManagement.DTO
{
    public class DichVuDTO
    {
        public int MaDV { get; set; }
        public string TenDV { get; set; }
        public decimal DonGia { get; set; }
        public int TonKho { get; set; }

        public override string ToString()
        {
            return $"{TenDV} - {DonGia:N0}đ (Tồn: {TonKho})";
        }
    }
}