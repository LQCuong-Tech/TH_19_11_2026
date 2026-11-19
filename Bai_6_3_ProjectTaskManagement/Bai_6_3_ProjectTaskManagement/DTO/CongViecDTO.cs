using System;

namespace Bai_6_3_ProjectTaskManagement.DTO
{
    public class CongViecDTO
    {
        public int MaTask { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; }
        public string TenTask { get; set; }
        public string MoTa { get; set; }
        public DateTime HanNop { get; set; }
        public string DoUuTien { get; set; }
        public string TrangThai { get; set; }
        public string NguoiThucHien { get; set; }

        public override string ToString()
        {
            string nguoi = string.IsNullOrEmpty(NguoiThucHien) ? "Chưa giao" : NguoiThucHien;
            return $"[#{MaTask}] {TenTask} | Ưu tiên: {DoUuTien} | Hạn: {HanNop:dd/MM} ({nguoi})";
        }
    }
}