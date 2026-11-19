using System;
using System.Collections.Generic;
using Bai_6_3_ProjectTaskManagement.DAL;
using Bai_6_3_ProjectTaskManagement.DTO;

namespace Bai_6_3_ProjectTaskManagement.BUS
{
    public class ThanhVienBUS
    {
        private readonly ThanhVienDAL _dal = new ThanhVienDAL();

        public List<ThanhVienDTO> LayTatCa() => _dal.LayTatCa();

        public void Them(ThanhVienDTO tv)
        {
            Validate(tv);
            if (!_dal.Them(tv))
                throw new Exception("Thêm thành viên thất bại.");
        }

        public void Sua(ThanhVienDTO tv)
        {
            if (tv.MaThanhVien <= 0) throw new Exception("Vui lòng chọn thành viên cần sửa.");
            Validate(tv);
            if (!_dal.Sua(tv))
                throw new Exception("Cập nhật thành viên thất bại.");
        }

        public void Xoa(int maThanhVien)
        {
            if (maThanhVien <= 0) throw new Exception("Vui lòng chọn thành viên cần xóa.");
            if (!_dal.Xoa(maThanhVien))
                throw new Exception("Xóa thành viên thất bại.");
        }

        private void Validate(ThanhVienDTO tv)
        {
            if (string.IsNullOrWhiteSpace(tv.HoTen))
                throw new Exception("Họ tên thành viên không được để trống.");
            if (string.IsNullOrWhiteSpace(tv.Email) || !tv.Email.Contains("@"))
                throw new Exception("Email không hợp lệ.");
        }
    }
}