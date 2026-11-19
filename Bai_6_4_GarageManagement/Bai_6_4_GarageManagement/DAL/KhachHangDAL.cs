using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bai_6_4_GarageManagement.DTO;

namespace Bai_6_4_GarageManagement.DAL
{
    public class KhachHangDAL
    {
        public List<KhachHangDTO> LayTatCa()
        {
            var list = new List<KhachHangDTO>();
            string sql = "SELECT MaKH, TenKH, DienThoai, DiaChi FROM KhachHang ORDER BY MaKH DESC";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new KhachHangDTO
                        {
                            MaKH = Convert.ToInt32(rd["MaKH"]),
                            TenKH = rd["TenKH"].ToString(),
                            DienThoai = rd["DienThoai"].ToString(),
                            DiaChi = rd["DiaChi"]?.ToString()
                        });
                    }
                }
            }
            return list;
        }

        public KhachHangDTO TimTheoDienThoai(string dienThoai)
        {
            string sql = "SELECT MaKH, TenKH, DienThoai, DiaChi FROM KhachHang WHERE DienThoai = @DienThoai";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@DienThoai", dienThoai);
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        return new KhachHangDTO
                        {
                            MaKH = Convert.ToInt32(rd["MaKH"]),
                            TenKH = rd["TenKH"].ToString(),
                            DienThoai = rd["DienThoai"].ToString(),
                            DiaChi = rd["DiaChi"]?.ToString()
                        };
                    }
                }
            }
            return null;
        }

        public int ThemVaLayMa(KhachHangDTO kh)
        {
            string sql = @"INSERT INTO KhachHang (TenKH, DienThoai, DiaChi)
                           VALUES (@TenKH, @DienThoai, @DiaChi);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TenKH", kh.TenKH);
                cmd.Parameters.AddWithValue("@DienThoai", kh.DienThoai);
                cmd.Parameters.AddWithValue("@DiaChi", (object)kh.DiaChi ?? DBNull.Value);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Sua(KhachHangDTO kh)
        {
            string sql = "UPDATE KhachHang SET TenKH = @TenKH, DienThoai = @DienThoai, DiaChi = @DiaChi WHERE MaKH = @MaKH";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaKH", kh.MaKH);
                cmd.Parameters.AddWithValue("@TenKH", kh.TenKH);
                cmd.Parameters.AddWithValue("@DienThoai", kh.DienThoai);
                cmd.Parameters.AddWithValue("@DiaChi", (object)kh.DiaChi ?? DBNull.Value);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}