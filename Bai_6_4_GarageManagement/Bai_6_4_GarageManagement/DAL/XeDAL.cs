using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bai_6_4_GarageManagement.DTO;

namespace Bai_6_4_GarageManagement.DAL
{
    public class XeDAL
    {
        public List<XeDTO> LayTatCa()
        {
            var list = new List<XeDTO>();
            string sql = @"SELECT x.BienSo, x.MaKH, kh.TenKH, kh.DienThoai, kh.DiaChi, x.HieuXe, x.NamSanXuat
                           FROM Xe x
                           JOIN KhachHang kh ON x.MaKH = kh.MaKH
                           ORDER BY x.BienSo";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new XeDTO
                        {
                            BienSo = rd["BienSo"].ToString(),
                            MaKH = Convert.ToInt32(rd["MaKH"]),
                            TenKH = rd["TenKH"].ToString(),
                            DienThoai = rd["DienThoai"].ToString(),
                            DiaChi = rd["DiaChi"]?.ToString(),
                            HieuXe = rd["HieuXe"].ToString(),
                            NamSanXuat = Convert.ToInt32(rd["NamSanXuat"])
                        });
                    }
                }
            }
            return list;
        }

        // Yêu cầu 1: Tra cứu biển số xe bằng SqlParameter
        public XeDTO TraCuuTheoBienSo(string bienSo)
        {
            string sql = @"SELECT x.BienSo, x.MaKH, kh.TenKH, kh.DienThoai, kh.DiaChi, x.HieuXe, x.NamSanXuat
                           FROM Xe x
                           JOIN KhachHang kh ON x.MaKH = kh.MaKH
                           WHERE x.BienSo = @BienSo";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@BienSo", bienSo.Trim().ToUpper());
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        return new XeDTO
                        {
                            BienSo = rd["BienSo"].ToString(),
                            MaKH = Convert.ToInt32(rd["MaKH"]),
                            TenKH = rd["TenKH"].ToString(),
                            DienThoai = rd["DienThoai"].ToString(),
                            DiaChi = rd["DiaChi"]?.ToString(),
                            HieuXe = rd["HieuXe"].ToString(),
                            NamSanXuat = Convert.ToInt32(rd["NamSanXuat"])
                        };
                    }
                }
            }
            return null;
        }

        // Tiếp nhận xe mới (nếu chủ xe chưa có thì tạo mới chủ xe rồi gắn vào Xe)
        public bool TiepNhanXeMoi(XeDTO xe)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    int maKH = xe.MaKH;

                    if (maKH <= 0)
                    {
                        // Kiểm tra SĐT đã tồn tại chưa
                        string sqlCheckKH = "SELECT MaKH FROM KhachHang WHERE DienThoai = @DienThoai";
                        using (SqlCommand cmdCheck = new SqlCommand(sqlCheckKH, conn, trans))
                        {
                            cmdCheck.Parameters.AddWithValue("@DienThoai", xe.DienThoai);
                            object res = cmdCheck.ExecuteScalar();
                            if (res != null && res != DBNull.Value)
                            {
                                maKH = Convert.ToInt32(res);
                                string sqlUpdKH = "UPDATE KhachHang SET TenKH = @TenKH, DiaChi = @DiaChi WHERE MaKH = @MaKH";
                                using (SqlCommand cmdUpd = new SqlCommand(sqlUpdKH, conn, trans))
                                {
                                    cmdUpd.Parameters.AddWithValue("@TenKH", xe.TenKH);
                                    cmdUpd.Parameters.AddWithValue("@DiaChi", (object)xe.DiaChi ?? DBNull.Value);
                                    cmdUpd.Parameters.AddWithValue("@MaKH", maKH);
                                    cmdUpd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                string sqlInsKH = @"INSERT INTO KhachHang (TenKH, DienThoai, DiaChi)
                                                    VALUES (@TenKH, @DienThoai, @DiaChi);
                                                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
                                using (SqlCommand cmdIns = new SqlCommand(sqlInsKH, conn, trans))
                                {
                                    cmdIns.Parameters.AddWithValue("@TenKH", xe.TenKH);
                                    cmdIns.Parameters.AddWithValue("@DienThoai", xe.DienThoai);
                                    cmdIns.Parameters.AddWithValue("@DiaChi", (object)xe.DiaChi ?? DBNull.Value);
                                    maKH = Convert.ToInt32(cmdIns.ExecuteScalar());
                                }
                            }
                        }
                    }

                    string sqlInsXe = @"INSERT INTO Xe (BienSo, MaKH, HieuXe, NamSanXuat)
                                        VALUES (@BienSo, @MaKH, @HieuXe, @NamSanXuat)";
                    using (SqlCommand cmdXe = new SqlCommand(sqlInsXe, conn, trans))
                    {
                        cmdXe.Parameters.AddWithValue("@BienSo", xe.BienSo.Trim().ToUpper());
                        cmdXe.Parameters.AddWithValue("@MaKH", maKH);
                        cmdXe.Parameters.AddWithValue("@HieuXe", xe.HieuXe);
                        cmdXe.Parameters.AddWithValue("@NamSanXuat", xe.NamSanXuat);
                        cmdXe.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public bool CapNhatXeVaChuXe(XeDTO xe)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    string sqlKH = "UPDATE KhachHang SET TenKH = @TenKH, DienThoai = @DienThoai, DiaChi = @DiaChi WHERE MaKH = @MaKH";
                    using (SqlCommand cmd1 = new SqlCommand(sqlKH, conn, trans))
                    {
                        cmd1.Parameters.AddWithValue("@MaKH", xe.MaKH);
                        cmd1.Parameters.AddWithValue("@TenKH", xe.TenKH);
                        cmd1.Parameters.AddWithValue("@DienThoai", xe.DienThoai);
                        cmd1.Parameters.AddWithValue("@DiaChi", (object)xe.DiaChi ?? DBNull.Value);
                        cmd1.ExecuteNonQuery();
                    }

                    string sqlXe = "UPDATE Xe SET HieuXe = @HieuXe, NamSanXuat = @NamSanXuat WHERE BienSo = @BienSo";
                    using (SqlCommand cmd2 = new SqlCommand(sqlXe, conn, trans))
                    {
                        cmd2.Parameters.AddWithValue("@BienSo", xe.BienSo.Trim().ToUpper());
                        cmd2.Parameters.AddWithValue("@HieuXe", xe.HieuXe);
                        cmd2.Parameters.AddWithValue("@NamSanXuat", xe.NamSanXuat);
                        cmd2.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public bool XoaXe(string bienSo)
        {
            string sql = "DELETE FROM Xe WHERE BienSo = @BienSo";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@BienSo", bienSo);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}