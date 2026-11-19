using System;
using System.Collections.Generic;
using System.Linq;
using Bai_6_3_ProjectTaskManagement.DAL;
using Bai_6_3_ProjectTaskManagement.DTO;

namespace Bai_6_3_ProjectTaskManagement.BUS
{
    public class CongViecBUS
    {
        private readonly CongViecDAL _cvDal = new CongViecDAL();
        private readonly PhanCongDAL _pcDal = new PhanCongDAL();

        public static readonly string[] DanhSachTrangThai = { "To Do", "In Progress", "Review", "Done" };

        public List<CongViecDTO> LayDanhSachTask(int? maDuAn = null, string trangThai = null)
        {
            return _cvDal.LayDanhSachTask(maDuAn, trangThai);
        }

        public void TaoTaskVaPhanCong(CongViecDTO cv, List<int> dsMaThanhVien)
        {
            Validate(cv);
            int maTaskMoi = _cvDal.ThemVaLayMa(cv);
            if (maTaskMoi <= 0)
                throw new Exception("Tạo công việc thất bại.");

            if (dsMaThanhVien != null && dsMaThanhVien.Count > 0)
            {
                _pcDal.LuuPhanCongNhieuThanhVien(maTaskMoi, dsMaThanhVien);
            }
        }

        public void CapNhatTaskVaPhanCong(CongViecDTO cv, List<int> dsMaThanhVien)
        {
            if (cv.MaTask <= 0) throw new Exception("Vui lòng chọn công việc cần sửa.");
            Validate(cv);

            if (!_cvDal.Sua(cv))
                throw new Exception("Cập nhật thông tin công việc thất bại.");

            _pcDal.LuuPhanCongNhieuThanhVien(cv.MaTask, dsMaThanhVien ?? new List<int>());
        }

        public void ChuyenTrangThai(int maTask, string trangThaiMoi)
        {
            if (maTask <= 0) throw new Exception("Vui lòng chọn một công việc.");
            if (!DanhSachTrangThai.Contains(trangThaiMoi))
                throw new Exception("Trạng thái công việc không hợp lệ.");

            if (!_cvDal.CapNhatTrangThai(maTask, trangThaiMoi))
                throw new Exception("Chuyển trạng thái công việc thất bại.");
        }

        public void Xoa(int maTask)
        {
            if (maTask <= 0) throw new Exception("Vui lòng chọn công việc cần xóa.");
            if (!_cvDal.Xoa(maTask))
                throw new Exception("Xóa công việc thất bại.");
        }

        public List<CongViecDTO> LayTaskTreHan()
        {
            return _cvDal.LayTaskTreHan();
        }

        private void Validate(CongViecDTO cv)
        {
            if (cv.MaDuAn <= 0)
                throw new Exception("Vui lòng chọn dự án cho công việc.");
            if (string.IsNullOrWhiteSpace(cv.TenTask))
                throw new Exception("Tên công việc (Task) không được để trống.");
        }
    }
}