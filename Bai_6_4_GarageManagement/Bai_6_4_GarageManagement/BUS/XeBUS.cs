using System;
using System.Collections.Generic;
using Bai_6_4_GarageManagement.DAL;
using Bai_6_4_GarageManagement.DTO;

namespace Bai_6_4_GarageManagement.BUS
{
    public class XeBUS
    {
        private readonly XeDAL _dal = new XeDAL();

        public List<XeDTO> LayTatCa() => _dal.LayTatCa();

        public XeDTO TraCuuTheoBienSo(string bienSo)
        {
            if (string.IsNullOrWhiteSpace(bienSo))
                throw new Exception("Vui lòng nhập biển số xe cần tra cứu.");
            return _dal.TraCuuTheoBienSo(bienSo);
        }

        public void TiepNhanXeMoi(XeDTO xe)
        {
            Validate(xe);
            if (_dal.TraCuuTheoBienSo(xe.BienSo) != null)
                throw new Exception($"Biển số xe '{xe.BienSo.ToUpper()}' đã tồn tại trong hệ thống.");

            if (!_dal.TiepNhanXeMoi(xe))
                throw new Exception("Tiếp nhận xe mới thất bại.");
        }

        public void CapNhat(XeDTO xe)
        {
            Validate(xe);
            if (!_dal.CapNhatXeVaChuXe(xe))
                throw new Exception("Cập nhật thông tin xe thất bại.");
        }

        public void Xoa(string bienSo)
        {
            if (string.IsNullOrWhiteSpace(bienSo))
                throw new Exception("Vui lòng chọn xe cần xóa.");
            if (!_dal.XoaXe(bienSo))
                throw new Exception("Xóa xe thất bại.");
        }

        private void Validate(XeDTO xe)
        {
            if (string.IsNullOrWhiteSpace(xe.BienSo))
                throw new Exception("Biển số xe không được để trống.");
            if (string.IsNullOrWhiteSpace(xe.TenKH))
                throw new Exception("Họ tên chủ xe không được để trống.");
            if (string.IsNullOrWhiteSpace(xe.DienThoai))
                throw new Exception("Số điện thoại chủ xe không được để trống.");
            if (string.IsNullOrWhiteSpace(xe.HieuXe))
                throw new Exception("Hiệu xe không được để trống.");
            if (xe.NamSanXuat < 1980 || xe.NamSanXuat > DateTime.Now.Year + 1)
                throw new Exception("Năm sản xuất không hợp lệ.");
        }
    }
}