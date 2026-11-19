using System;
using System.Collections.Generic;
using System.Linq;
using Bai_6_4_GarageManagement.DAL;
using Bai_6_4_GarageManagement.DTO;

namespace Bai_6_4_GarageManagement.BUS
{
    public class PhieuSuaChuaBUS
    {
        private readonly PhieuSuaChuaDAL _dal = new PhieuSuaChuaDAL();

        public decimal TinhTongChiPhi(List<ChiTietSuaChuaDTO> dsChiTiet)
        {
            if (dsChiTiet == null || dsChiTiet.Count == 0) return 0;
            return dsChiTiet.Sum(x => (x.SoLuong * x.DonGia) + x.TienCong);
        }

        public int XacNhanLapPhieu(PhieuSuaChuaDTO phieu, List<ChiTietSuaChuaDTO> dsChiTiet)
        {
            if (string.IsNullOrWhiteSpace(phieu.BienSo))
                throw new Exception("Vui lòng chọn hoặc tra cứu biển số xe cần sửa chữa.");
            if (string.IsNullOrWhiteSpace(phieu.NoiDungSua))
                throw new Exception("Vui lòng nhập nội dung sửa chữa / bảo dưỡng.");
            if (dsChiTiet == null || dsChiTiet.Count == 0)
                throw new Exception("Phiếu sửa chữa phải có ít nhất 1 phụ tùng / hạng mục thay thế.");

            phieu.TongTien = TinhTongChiPhi(dsChiTiet);
            if (string.IsNullOrWhiteSpace(phieu.TrangThai))
                phieu.TrangThai = "Đã hoàn thành";

            return _dal.LapPhieuSuaChuaTransaction(phieu, dsChiTiet);
        }

        public List<PhieuSuaChuaDTO> LayDanhSachPhieu(string trangThai = null)
        {
            return _dal.LayDanhSachPhieu(trangThai);
        }

        public List<ChiTietSuaChuaDTO> LayChiTietPhieu(int maPhieu)
        {
            return _dal.LayChiTietPhieu(maPhieu);
        }

        public void ThanhToan(int maPhieu)
        {
            if (maPhieu <= 0) throw new Exception("Vui lòng chọn phiếu sửa chữa cần thanh toán.");
            var chiTiet = _dal.LayChiTietPhieu(maPhieu);
            decimal tongTien = TinhTongChiPhi(chiTiet);

            if (!_dal.ThanhToanPhieu(maPhieu, tongTien))
                throw new Exception("Thanh toán phiếu sửa chữa thất bại.");
        }
    }
}