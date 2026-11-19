using System;
using System.Collections.Generic;
using Bai_6_5_KaraokeManagement.DAL;
using Bai_6_5_KaraokeManagement.DTO;

namespace Bai_6_5_KaraokeManagement.BUS
{
    public class DichVuBUS
    {
        private readonly DichVuDAL _dal = new DichVuDAL();

        public List<DichVuDTO> LayTatCa() => _dal.LayTatCa();

        public void Them(DichVuDTO dv)
        {
            Validate(dv);
            if (!_dal.Them(dv)) throw new Exception("Thêm dịch vụ thất bại.");
        }

        public void Sua(DichVuDTO dv)
        {
            if (dv.MaDV <= 0) throw new Exception("Vui lòng chọn dịch vụ cần sửa.");
            Validate(dv);
            if (!_dal.Sua(dv)) throw new Exception("Cập nhật dịch vụ thất bại.");
        }

        public void Xoa(int maDV)
        {
            if (maDV <= 0) throw new Exception("Vui lòng chọn dịch vụ cần xóa.");
            if (!_dal.Xoa(maDV)) throw new Exception("Xóa dịch vụ thất bại.");
        }

        private void Validate(DichVuDTO dv)
        {
            if (string.IsNullOrWhiteSpace(dv.TenDV))
                throw new Exception("Tên dịch vụ không được để trống.");
            if (dv.DonGia < 0)
                throw new Exception("Đơn giá không được âm.");
            if (dv.TonKho < 0)
                throw new Exception("Tồn kho không được âm.");
        }
    }
}