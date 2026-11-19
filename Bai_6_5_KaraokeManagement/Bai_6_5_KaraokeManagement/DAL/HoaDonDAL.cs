using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Bai_6_5_KaraokeManagement.DTO;

namespace Bai_6_5_KaraokeManagement.DAL
{
    public class HoaDonDAL
    {
        // Lấy hóa đơn chưa thanh toán (TrangThai = 0) của một phòng đang hát
        public HoaDonDTO LayHoaDonChuaThanhToanTheoPhong(int maPhong)
        {
            string sql = @"SELECT TOP 1 hd.MaHD, hd.MaPhong, p.TenPhong, hd.GioVao, hd.GioRa,
                                        hd.TongTienPhong, hd.TongTienDichVu, hd.TrangThai
                           FROM HoaDon hd
                           JOIN PhongHat p ON hd.MaPhong = p.MaPhong
                           WHERE hd.MaPhong = @MaPhong AND hd.TrangThai = 0
                           ORDER BY hd.MaHD DESC";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        return new HoaDonDTO
                        {
                            MaHD = Convert.ToInt32(rd["MaHD"]),
                            MaPhong = Convert.ToInt32(rd["MaPhong"]),
                            TenPhong = rd["TenPhong"].ToString(),
                            GioVao = Convert.ToDateTime(rd["GioVao"]),
                            GioRa = rd["GioRa"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rd["GioRa"]),
                            TongTienPhong = Convert.ToDecimal(rd["TongTienPhong"]),
                            TongTienDichVu = Convert.ToDecimal(rd["TongTienDichVu"]),
                            TrangThai = Convert.ToBoolean(rd["TrangThai"])
                        };
                    }
                }
            }
            return null;
        }

        // Yêu cầu 1: Mở phòng -> Tạo HoaDon mới (GioVao = GetDate()) + đổi TrangThai phòng sang "Đang hát"
        public int MoPhong(int maPhong)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    // Kiểm tra trạng thái phòng hiện tại
                    string sqlCheck = "SELECT TrangThai FROM PhongHat WITH (UPDLOCK) WHERE MaPhong = @MaPhong";
                    using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn, trans))
                    {
                        cmdCheck.Parameters.AddWithValue("@MaPhong", maPhong);
                        string trangThai = cmdCheck.ExecuteScalar()?.ToString();
                        if (trangThai != "Trống")
                            throw new Exception("Phòng này hiện không ở trạng thái Trống!");
                    }

                    // 1. Tạo HoaDon mới ghi nhận GioVao = GETDATE()
                    string sqlInsertHD = @"INSERT INTO HoaDon (MaPhong, GioVao, TongTienPhong, TongTienDichVu, TrangThai)
                                           VALUES (@MaPhong, GETDATE(), 0, 0, 0);
                                           SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    int maHDMoi;
                    using (SqlCommand cmdHD = new SqlCommand(sqlInsertHD, conn, trans))
                    {
                        cmdHD.Parameters.AddWithValue("@MaPhong", maPhong);
                        maHDMoi = Convert.ToInt32(cmdHD.ExecuteScalar());
                    }

                    // 2. Đổi trạng thái PhongHat sang "Đang hát"
                    string sqlUpdatePhong = "UPDATE PhongHat SET TrangThai = N'Đang hát' WHERE MaPhong = @MaPhong";
                    using (SqlCommand cmdPhong = new SqlCommand(sqlUpdatePhong, conn, trans))
                    {
                        cmdPhong.Parameters.AddWithValue("@MaPhong", maPhong);
                        cmdPhong.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return maHDMoi;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        // Yêu cầu 2 (Disconnected Model): Đọc danh sách dịch vụ đã đặt bằng SqlDataAdapter + DataTable
        public DataTable LayDataTableChiTietDichVu(int maHD)
        {
            string sql = @"SELECT ct.MaHD, ct.MaDV, dv.TenDV, ct.SoLuong, ct.DonGia,
                                  (ct.SoLuong * ct.DonGia) AS ThanhTien
                           FROM ChiTietDichVu ct
                           JOIN DichVu dv ON ct.MaDV = dv.MaDV
                           WHERE ct.MaHD = @MaHD";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conn))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@MaHD", maHD);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        // Yêu cầu 2 (Disconnected Model): Đồng bộ thay đổi từ DataTable xuống CSDL qua SqlDataAdapter.Update()
        public void DongBoDataTableChiTietDichVu(DataTable dt)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlDataAdapter adapter = new SqlDataAdapter())
            {
                adapter.InsertCommand = new SqlCommand(
                    "INSERT INTO ChiTietDichVu (MaHD, MaDV, SoLuong, DonGia) VALUES (@MaHD, @MaDV, @SoLuong, @DonGia)", conn);
                adapter.InsertCommand.Parameters.Add("@MaHD", SqlDbType.Int, 4, "MaHD");
                adapter.InsertCommand.Parameters.Add("@MaDV", SqlDbType.Int, 4, "MaDV");
                adapter.InsertCommand.Parameters.Add("@SoLuong", SqlDbType.Int, 4, "SoLuong");
                adapter.InsertCommand.Parameters.Add("@DonGia", SqlDbType.Decimal, 18, "DonGia");

                adapter.UpdateCommand = new SqlCommand(
                    "UPDATE ChiTietDichVu SET SoLuong = @SoLuong, DonGia = @DonGia WHERE MaHD = @MaHD AND MaDV = @MaDV", conn);
                adapter.UpdateCommand.Parameters.Add("@SoLuong", SqlDbType.Int, 4, "SoLuong");
                adapter.UpdateCommand.Parameters.Add("@DonGia", SqlDbType.Decimal, 18, "DonGia");
                adapter.UpdateCommand.Parameters.Add("@MaHD", SqlDbType.Int, 4, "MaHD").SourceVersion = DataRowVersion.Original;
                adapter.UpdateCommand.Parameters.Add("@MaDV", SqlDbType.Int, 4, "MaDV").SourceVersion = DataRowVersion.Original;

                adapter.DeleteCommand = new SqlCommand(
                    "DELETE FROM ChiTietDichVu WHERE MaHD = @MaHD AND MaDV = @MaDV", conn);
                adapter.DeleteCommand.Parameters.Add("@MaHD", SqlDbType.Int, 4, "MaHD").SourceVersion = DataRowVersion.Original;
                adapter.DeleteCommand.Parameters.Add("@MaDV", SqlDbType.Int, 4, "MaDV").SourceVersion = DataRowVersion.Original;

                adapter.Update(dt);
            }
        }

        // Yêu cầu 3: Thanh toán hóa đơn bằng SqlTransaction
        public void ThanhToanHoaDonTransaction(int maHD, int maPhong, DateTime gioRa, decimal tongTienPhong, decimal tongTienDichVu)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // Đọc danh sách dịch vụ đã đặt của hóa đơn để trừ kho
                    var dsTruKho = new List<(int MaDV, string TenDV, int SoLuongDat, int TonHienTai)>();
                    string sqlGetCT = @"SELECT ct.MaDV, dv.TenDV, ct.SoLuong, dv.TonKho
                                        FROM ChiTietDichVu ct
                                        JOIN DichVu dv WITH (UPDLOCK) ON ct.MaDV = dv.MaDV
                                        WHERE ct.MaHD = @MaHD";

                    using (SqlCommand cmdCT = new SqlCommand(sqlGetCT, conn, trans))
                    {
                        cmdCT.Parameters.AddWithValue("@MaHD", maHD);
                        using (SqlDataReader rd = cmdCT.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                dsTruKho.Add((
                                    Convert.ToInt32(rd["MaDV"]),
                                    rd["TenDV"].ToString(),
                                    Convert.ToInt32(rd["SoLuong"]),
                                    Convert.ToInt32(rd["TonKho"])
                                ));
                            }
                        }
                    }

                    // 1. Kiểm tra & Trừ TonKho tương ứng trong bảng DichVu
                    foreach (var item in dsTruKho)
                    {
                        if (item.TonHienTai < item.SoLuongDat)
                        {
                            throw new Exception($"Mặt hàng '{item.TenDV}' chỉ còn {item.TonHienTai} trong kho, không đủ trừ {item.SoLuongDat}!");
                        }

                        string sqlTruKho = "UPDATE DichVu SET TonKho = TonKho - @SoLuong WHERE MaDV = @MaDV";
                        using (SqlCommand cmdKho = new SqlCommand(sqlTruKho, conn, trans))
                        {
                            cmdKho.Parameters.AddWithValue("@SoLuong", item.SoLuongDat);
                            cmdKho.Parameters.AddWithValue("@MaDV", item.MaDV);
                            cmdKho.ExecuteNonQuery();
                        }
                    }

                    // 2. Cập nhật GioRa, TongTienPhong, TongTienDichVu, TrangThai = 1 cho HoaDon
                    string sqlUpdHD = @"UPDATE HoaDon
                                        SET GioRa = @GioRa,
                                            TongTienPhong = @TongTienPhong,
                                            TongTienDichVu = @TongTienDichVu,
                                            TrangThai = 1
                                        WHERE MaHD = @MaHD";
                    using (SqlCommand cmdHD = new SqlCommand(sqlUpdHD, conn, trans))
                    {
                        cmdHD.Parameters.AddWithValue("@GioRa", gioRa);
                        cmdHD.Parameters.AddWithValue("@TongTienPhong", tongTienPhong);
                        cmdHD.Parameters.AddWithValue("@TongTienDichVu", tongTienDichVu);
                        cmdHD.Parameters.AddWithValue("@MaHD", maHD);
                        cmdHD.ExecuteNonQuery();
                    }

                    // 3. Cập nhật trạng thái PhongHat về "Trống"
                    string sqlUpdPhong = "UPDATE PhongHat SET TrangThai = N'Trống' WHERE MaPhong = @MaPhong";
                    using (SqlCommand cmdPhong = new SqlCommand(sqlUpdPhong, conn, trans))
                    {
                        cmdPhong.Parameters.AddWithValue("@MaPhong", maPhong);
                        cmdPhong.ExecuteNonQuery();
                    }

                    // 4. Hoàn tất giao dịch
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }
}