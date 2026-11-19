using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bai_6_4_GarageManagement.DTO;

namespace Bai_6_4_GarageManagement.DAL
{
    public class PhieuSuaChuaDAL
    {
        // Yêu cầu 2: Lập phiếu sửa chữa & Thay thế phụ tùng bằng SqlTransaction
        public int LapPhieuSuaChuaTransaction(PhieuSuaChuaDTO phieu, List<ChiTietSuaChuaDTO> dsChiTiet)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // 1. Tạo bản ghi PhieuSuaChua
                    string sqlInsertPhieu = @"
                        INSERT INTO PhieuSuaChua (BienSo, NgayNhan, NoiDungSua, TongTien, TrangThai)
                        VALUES (@BienSo, GETDATE(), @NoiDungSua, @TongTien, @TrangThai);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    int maPhieuMoi;
                    using (SqlCommand cmdPhieu = new SqlCommand(sqlInsertPhieu, conn, trans))
                    {
                        cmdPhieu.Parameters.AddWithValue("@BienSo", phieu.BienSo);
                        cmdPhieu.Parameters.AddWithValue("@NoiDungSua", phieu.NoiDungSua);
                        cmdPhieu.Parameters.AddWithValue("@TongTien", phieu.TongTien);
                        cmdPhieu.Parameters.AddWithValue("@TrangThai", phieu.TrangThai);
                        maPhieuMoi = Convert.ToInt32(cmdPhieu.ExecuteScalar());
                    }

                    // 2. Duyệt từng chi tiết: Kiểm tra tồn kho -> Thêm ChiTietSuaChua -> Trừ SoLuongTon
                    foreach (var item in dsChiTiet)
                    {
                        // Kiểm tra tồn kho hiện tại trong Transaction (WITH (UPDLOCK) tránh xung đột đồng thời)
                        string sqlCheckTon = "SELECT TenPhuTung, SoLuongTon FROM PhuTung WITH (UPDLOCK) WHERE MaPhuTung = @MaPT";
                        int tonHienTai = 0;
                        string tenPT = item.TenPhuTung;

                        using (SqlCommand cmdCheck = new SqlCommand(sqlCheckTon, conn, trans))
                        {
                            cmdCheck.Parameters.AddWithValue("@MaPT", item.MaPhuTung);
                            using (SqlDataReader rd = cmdCheck.ExecuteReader())
                            {
                                if (!rd.Read())
                                    throw new Exception($"Không tìm thấy phụ tùng mã #{item.MaPhuTung} trong kho.");

                                tenPT = rd["TenPhuTung"].ToString();
                                tonHienTai = Convert.ToInt32(rd["SoLuongTon"]);
                            }
                        }

                        // Nếu không đủ số lượng trong kho -> ném lỗi để nhảy xuống catch gọi trans.Rollback()
                        if (tonHienTai < item.SoLuong)
                        {
                            throw new Exception($"Phụ tùng '{tenPT}' chỉ còn {tonHienTai} cái trong kho, không đủ xuất {item.SoLuong} cái!");
                        }

                        // Thêm bản ghi ChiTietSuaChua
                        string sqlInsertCT = @"
                            INSERT INTO ChiTietSuaChua (MaPhieu, MaPhuTung, SoLuong, TienCong)
                            VALUES (@MaPhieu, @MaPhuTung, @SoLuong, @TienCong)";
                        using (SqlCommand cmdCT = new SqlCommand(sqlInsertCT, conn, trans))
                        {
                            cmdCT.Parameters.AddWithValue("@MaPhieu", maPhieuMoi);
                            cmdCT.Parameters.AddWithValue("@MaPhuTung", item.MaPhuTung);
                            cmdCT.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                            cmdCT.Parameters.AddWithValue("@TienCong", item.TienCong);
                            cmdCT.ExecuteNonQuery();
                        }

                        // Trừ số lượng phụ tùng trong kho (SoLuongTon -= SoLuong)
                        string sqlTruKho = "UPDATE PhuTung SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaPhuTung = @MaPhuTung";
                        using (SqlCommand cmdKho = new SqlCommand(sqlTruKho, conn, trans))
                        {
                            cmdKho.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                            cmdKho.Parameters.AddWithValue("@MaPhuTung", item.MaPhuTung);
                            cmdKho.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                    return maPhieuMoi;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public List<PhieuSuaChuaDTO> LayDanhSachPhieu(string trangThai = null)
        {
            var list = new List<PhieuSuaChuaDTO>();
            string sql = @"
                SELECT p.MaPhieu, p.BienSo, kh.TenKH, kh.DienThoai, x.HieuXe,
                       p.NgayNhan, p.NoiDungSua, p.TongTien, p.TrangThai
                FROM PhieuSuaChua p
                JOIN Xe x ON p.BienSo = x.BienSo
                JOIN KhachHang kh ON x.MaKH = kh.MaKH
                WHERE (@TrangThai IS NULL OR @TrangThai = N'Tất cả' OR p.TrangThai = @TrangThai)
                ORDER BY p.MaPhieu DESC";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TrangThai", string.IsNullOrEmpty(trangThai) ? (object)DBNull.Value : trangThai);
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new PhieuSuaChuaDTO
                        {
                            MaPhieu = Convert.ToInt32(rd["MaPhieu"]),
                            BienSo = rd["BienSo"].ToString(),
                            TenKH = rd["TenKH"].ToString(),
                            DienThoai = rd["DienThoai"].ToString(),
                            HieuXe = rd["HieuXe"].ToString(),
                            NgayNhan = Convert.ToDateTime(rd["NgayNhan"]),
                            NoiDungSua = rd["NoiDungSua"]?.ToString(),
                            TongTien = Convert.ToDecimal(rd["TongTien"]),
                            TrangThai = rd["TrangThai"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<ChiTietSuaChuaDTO> LayChiTietPhieu(int maPhieu)
        {
            var list = new List<ChiTietSuaChuaDTO>();
            string sql = @"
                SELECT ct.MaPhieu, ct.MaPhuTung, pt.TenPhuTung, pt.DonGia, ct.SoLuong, ct.TienCong
                FROM ChiTietSuaChua ct
                JOIN PhuTung pt ON ct.MaPhuTung = pt.MaPhuTung
                WHERE ct.MaPhieu = @MaPhieu";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new ChiTietSuaChuaDTO
                        {
                            MaPhieu = Convert.ToInt32(rd["MaPhieu"]),
                            MaPhuTung = Convert.ToInt32(rd["MaPhuTung"]),
                            TenPhuTung = rd["TenPhuTung"].ToString(),
                            DonGia = Convert.ToDecimal(rd["DonGia"]),
                            SoLuong = Convert.ToInt32(rd["SoLuong"]),
                            TienCong = Convert.ToDecimal(rd["TienCong"])
                        });
                    }
                }
            }
            return list;
        }

        // Yêu cầu 3: Cập nhật trạng thái thanh toán & đồng bộ lại tổng tiền chuẩn xác
        public bool ThanhToanPhieu(int maPhieu, decimal tongTien)
        {
            string sql = @"UPDATE PhieuSuaChua
                           SET TrangThai = N'Đã thanh toán', TongTien = @TongTien
                           WHERE MaPhieu = @MaPhieu";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TongTien", tongTien);
                cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}