using System;
using System.Collections.Generic;
using Bai_6_3_ProjectTaskManagement.DAL;
using Bai_6_3_ProjectTaskManagement.DTO;

namespace Bai_6_3_ProjectTaskManagement.BUS
{
    public class DuAnBUS
    {
        private readonly DuAnDAL _dal = new DuAnDAL();

        public List<DuAnDTO> LayTatCa() => _dal.LayTatCa();

        public void Them(DuAnDTO da)
        {
            Validate(da);
            if (!_dal.Them(da))
                throw new Exception("Không thể thêm dự án mới.");
        }

        public void Sua(DuAnDTO da)
        {
            if (da.MaDuAn <= 0) throw new Exception("Vui lòng chọn dự án cần sửa.");
            Validate(da);
            if (!_dal.Sua(da))
                throw new Exception("Cập nhật dự án thất bại.");
        }

        public void Xoa(int maDuAn)
        {
            if (maDuAn <= 0) throw new Exception("Vui lòng chọn dự án cần xóa.");
            if (!_dal.Xoa(maDuAn))
                throw new Exception("Xóa dự án thất bại.");
        }

        private void Validate(DuAnDTO da)
        {
            if (string.IsNullOrWhiteSpace(da.TenDuAn))
                throw new Exception("Tên dự án không được để trống.");
            if (da.NgayKetThuc.Date < da.NgayBatDau.Date)
                throw new Exception("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");
        }
    }
}