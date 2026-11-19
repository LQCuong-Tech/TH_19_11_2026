using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bai_6_5_KaraokeManagement.DTO;

namespace Bai_6_5_KaraokeManagement.DAL
{
    public class PhongHatDAL
    {
        public List<PhongHatDTO> LayTatCa(string trangThai = null)
        {
            var list = new List<PhongHatDTO>();
            string sql = @"SELECT MaPhong, TenPhong, LoaiPhong, GiaTheoGio, TrangThai
                           FROM PhongHat
                           WHERE (@TrangThai IS NULL OR @TrangThai = N'Tất cả' OR TrangThai = @TrangThai)
                           ORDER BY MaPhong ASC";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TrangThai", string.IsNullOrEmpty(trangThai) ? (object)DBNull.Value : trangThai);
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new PhongHatDTO
                        {
                            MaPhong = Convert.ToInt32(rd["MaPhong"]),
                            TenPhong = rd["TenPhong"].ToString(),
                            LoaiPhong = rd["LoaiPhong"].ToString(),
                            GiaTheoGio = Convert.ToDecimal(rd["GiaTheoGio"]),
                            TrangThai = rd["TrangThai"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public bool Them(PhongHatDTO p)
        {
            string sql = "INSERT INTO PhongHat (TenPhong, LoaiPhong, GiaTheoGio, TrangThai) VALUES (@TenPhong, @LoaiPhong, @GiaTheoGio, N'Trống')";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TenPhong", p.TenPhong);
                cmd.Parameters.AddWithValue("@LoaiPhong", p.LoaiPhong);
                cmd.Parameters.AddWithValue("@GiaTheoGio", p.GiaTheoGio);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Sua(PhongHatDTO p)
        {
            string sql = "UPDATE PhongHat SET TenPhong = @TenPhong, LoaiPhong = @LoaiPhong, GiaTheoGio = @GiaTheoGio WHERE MaPhong = @MaPhong";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaPhong", p.MaPhong);
                cmd.Parameters.AddWithValue("@TenPhong", p.TenPhong);
                cmd.Parameters.AddWithValue("@LoaiPhong", p.LoaiPhong);
                cmd.Parameters.AddWithValue("@GiaTheoGio", p.GiaTheoGio);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Xoa(int maPhong)
        {
            string sql = "DELETE FROM PhongHat WHERE MaPhong = @MaPhong AND TrangThai = N'Trống'";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}