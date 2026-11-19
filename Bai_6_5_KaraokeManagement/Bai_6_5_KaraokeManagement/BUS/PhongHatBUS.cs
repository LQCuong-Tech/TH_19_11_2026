using System;
using System.Collections.Generic;
using Bai_6_5_KaraokeManagement.DAL;
using Bai_6_5_KaraokeManagement.DTO;

namespace Bai_6_5_KaraokeManagement.BUS
{
    public class PhongHatBUS
    {
        private readonly PhongHatDAL _dal = new PhongHatDAL();

        public List<PhongHatDTO> LayTatCa(string trangThai = null) => _dal.LayTatCa(trangThai);

        public void Them(PhongHatDTO p)
        {
            Validate(p);
            if (!_dal.Them(p)) throw new Exception("Thêm phòng hát thất bại.");
        }

        public void Sua(PhongHatDTO p)
        {
            if (p.MaPhong <= 0) throw new Exception("Vui lòng chọn phòng cần sửa.");
            Validate(p);
            if (!_dal.Sua(p)) throw new Exception("Cập nhật phòng hát thất bại.");
        }

        public void Xoa(int maPhong)
        {
            if (maPhong <= 0) throw new Exception("Vui lòng chọn phòng cần xóa.");
            if (!_dal.Xoa(maPhong))
                throw new Exception("Không thể xóa phòng đang hát hoặc đã có hóa đơn.");
        }

        private void Validate(PhongHatDTO p)
        {
            if (string.IsNullOrWhiteSpace(p.TenPhong))
                throw new Exception("Tên phòng không được để trống.");
            if (p.GiaTheoGio <= 0)
                throw new Exception("Giá theo giờ phải lớn hơn 0.");
        }
    }
}