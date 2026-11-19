using System;
using System.Collections.Generic;
using Bai_6_4_GarageManagement.DAL;
using Bai_6_4_GarageManagement.DTO;

namespace Bai_6_4_GarageManagement.BUS
{
    public class PhuTungBUS
    {
        private readonly PhuTungDAL _dal = new PhuTungDAL();

        public List<PhuTungDTO> LayTatCa() => _dal.LayTatCa();

        public void Them(PhuTungDTO pt)
        {
            Validate(pt);
            if (!_dal.Them(pt))
                throw new Exception("Thêm phụ tùng thất bại.");
        }

        public void Sua(PhuTungDTO pt)
        {
            if (pt.MaPhuTung <= 0) throw new Exception("Vui lòng chọn phụ tùng cần sửa.");
            Validate(pt);
            if (!_dal.Sua(pt))
                throw new Exception("Cập nhật phụ tùng thất bại.");
        }

        public void Xoa(int maPT)
        {
            if (maPT <= 0) throw new Exception("Vui lòng chọn phụ tùng cần xóa.");
            if (!_dal.Xoa(maPT))
                throw new Exception("Xóa phụ tùng thất bại.");
        }

        private void Validate(PhuTungDTO pt)
        {
            if (string.IsNullOrWhiteSpace(pt.TenPhuTung))
                throw new Exception("Tên phụ tùng không được để trống.");
            if (pt.DonGia < 0)
                throw new Exception("Đơn giá phụ tùng không được âm.");
            if (pt.SoLuongTon < 0)
                throw new Exception("Số lượng tồn kho phải >= 0.");
        }
    }
}