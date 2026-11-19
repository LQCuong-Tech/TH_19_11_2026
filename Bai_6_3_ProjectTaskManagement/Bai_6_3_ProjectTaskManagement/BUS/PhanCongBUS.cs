using System;
using System.Collections.Generic;
using Bai_6_3_ProjectTaskManagement.DAL;
using Bai_6_3_ProjectTaskManagement.DTO;

namespace Bai_6_3_ProjectTaskManagement.BUS
{
    public class PhanCongBUS
    {
        private readonly PhanCongDAL _dal = new PhanCongDAL();

        public List<int> LayMaThanhVienTheoTask(int maTask) => _dal.LayMaThanhVienTheoTask(maTask);

        public void PhanCongNhieuNguoi(int maTask, List<int> dsMaThanhVien)
        {
            if (maTask <= 0) throw new Exception("Vui lòng chọn công việc cần phân công.");
            _dal.LuuPhanCongNhieuThanhVien(maTask, dsMaThanhVien ?? new List<int>());
        }

        public List<ThongKeDTO> ThongKeTyLeHoanThanh(int maDuAn)
        {
            if (maDuAn <= 0) return new List<ThongKeDTO>();
            return _dal.ThongKeTyLeHoanThanhTheoDuAn(maDuAn);
        }

        public double TinhTyLeHoanThanhCaNhan(int maDuAn, int maThanhVien)
        {
            if (maDuAn <= 0 || maThanhVien <= 0) return 0;
            return _dal.TinhTyLeHoanThanhExecuteScalar(maDuAn, maThanhVien);
        }
    }
}