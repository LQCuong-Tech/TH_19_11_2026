using System;
using System.Data;
using Bai_6_5_KaraokeManagement.DAL;
using Bai_6_5_KaraokeManagement.DTO;

namespace Bai_6_5_KaraokeManagement.BUS
{
    public class HoaDonBUS
    {
        private readonly HoaDonDAL _dal = new HoaDonDAL();

        public HoaDonDTO LayHoaDonDangHoatDong(int maPhong)
        {
            return _dal.LayHoaDonChuaThanhToanTheoPhong(maPhong);
        }

        public int MoPhong(int maPhong)
        {
            if (maPhong <= 0) throw new Exception("Vui lòng chọn phòng cần mở.");
            return _dal.MoPhong(maPhong);
        }

        public DataTable LayDataTableChiTietDichVu(int maHD)
        {
            return _dal.LayDataTableChiTietDichVu(maHD);
        }

        // Thêm dịch vụ vào DataTable ngắt kết nối và đồng bộ xuống CSDL
        public void ThemDichVuVaoDataTable(DataTable dtChiTiet, int maHD, DichVuDTO dv, int soLuong)
        {
            if (maHD <= 0) throw new Exception("Phòng chưa mở hóa đơn.");
            if (dv == null) throw new Exception("Vui lòng chọn dịch vụ / đồ uống.");
            if (soLuong <= 0) throw new Exception("Số lượng gọi phải lớn hơn 0.");

            DataRow existingRow = null;
            foreach (DataRow row in dtChiTiet.Rows)
            {
                if (row.RowState != DataRowState.Deleted && Convert.ToInt32(row["MaDV"]) == dv.MaDV)
                {
                    existingRow = row;
                    break;
                }
            }

            if (existingRow != null)
            {
                int slMoi = Convert.ToInt32(existingRow["SoLuong"]) + soLuong;
                existingRow["SoLuong"] = slMoi;
                existingRow["ThanhTien"] = slMoi * dv.DonGia;
            }
            else
            {
                DataRow newRow = dtChiTiet.NewRow();
                newRow["MaHD"] = maHD;
                newRow["MaDV"] = dv.MaDV;
                newRow["TenDV"] = dv.TenDV;
                newRow["SoLuong"] = soLuong;
                newRow["DonGia"] = dv.DonGia;
                newRow["ThanhTien"] = soLuong * dv.DonGia;
                dtChiTiet.Rows.Add(newRow);
            }

            _dal.DongBoDataTableChiTietDichVu(dtChiTiet);
        }

        public void XoaDongDichVuTrongDataTable(DataTable dtChiTiet, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dtChiTiet.Rows.Count) return;
            dtChiTiet.Rows[rowIndex].Delete();
            _dal.DongBoDataTableChiTietDichVu(dtChiTiet);
        }

        public decimal TinhTongTienDichVu(DataTable dtChiTiet)
        {
            if (dtChiTiet == null) return 0;
            decimal tong = 0;
            foreach (DataRow row in dtChiTiet.Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                    tong += Convert.ToDecimal(row["ThanhTien"]);
            }
            return tong;
        }

        public decimal TinhTienPhong(DateTime gioVao, DateTime gioRa, decimal giaTheoGio)
        {
            double soGio = (gioRa - gioVao).TotalHours;
            if (soGio < 0) soGio = 0;
            return Math.Round((decimal)soGio * giaTheoGio, 0);
        }

        public void ThanhToan(int maHD, int maPhong, DateTime gioVao, DateTime gioRa, decimal giaTheoGio, DataTable dtChiTiet)
        {
            if (maHD <= 0) throw new Exception("Không tìm thấy hóa đơn cần thanh toán.");
            decimal tongTienPhong = TinhTienPhong(gioVao, gioRa, giaTheoGio);
            decimal tongTienDichVu = TinhTongTienDichVu(dtChiTiet);

            _dal.ThanhToanHoaDonTransaction(maHD, maPhong, gioRa, tongTienPhong, tongTienDichVu);
        }
    }
}