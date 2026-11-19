using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bai_6_3_ProjectTaskManagement.DTO;

namespace Bai_6_3_ProjectTaskManagement.DAL
{
    public class ThanhVienDAL
    {
        public List<ThanhVienDTO> LayTatCa()
        {
            var list = new List<ThanhVienDTO>();
            string sql = "SELECT MaThanhVien, HoTen, Email, ChuyenMon FROM ThanhVien ORDER BY HoTen";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new ThanhVienDTO
                        {
                            MaThanhVien = Convert.ToInt32(rd["MaThanhVien"]),
                            HoTen = rd["HoTen"].ToString(),
                            Email = rd["Email"]?.ToString(),
                            ChuyenMon = rd["ChuyenMon"]?.ToString()
                        });
                    }
                }
            }
            return list;
        }

        public bool Them(ThanhVienDTO tv)
        {
            string sql = "INSERT INTO ThanhVien (HoTen, Email, ChuyenMon) VALUES (@HoTen, @Email, @ChuyenMon)";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@HoTen", tv.HoTen);
                cmd.Parameters.AddWithValue("@Email", tv.Email);
                cmd.Parameters.AddWithValue("@ChuyenMon", tv.ChuyenMon);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Sua(ThanhVienDTO tv)
        {
            string sql = @"UPDATE ThanhVien SET HoTen = @HoTen, Email = @Email, ChuyenMon = @ChuyenMon
                           WHERE MaThanhVien = @MaThanhVien";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaThanhVien", tv.MaThanhVien);
                cmd.Parameters.AddWithValue("@HoTen", tv.HoTen);
                cmd.Parameters.AddWithValue("@Email", tv.Email);
                cmd.Parameters.AddWithValue("@ChuyenMon", tv.ChuyenMon);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Xoa(int maThanhVien)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    using (SqlCommand cmd1 = new SqlCommand("DELETE FROM PhanCong WHERE MaThanhVien = @MaTV", conn, tran))
                    {
                        cmd1.Parameters.AddWithValue("@MaTV", maThanhVien);
                        cmd1.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd2 = new SqlCommand("DELETE FROM ThanhVien WHERE MaThanhVien = @MaTV", conn, tran))
                    {
                        cmd2.Parameters.AddWithValue("@MaTV", maThanhVien);
                        int rows = cmd2.ExecuteNonQuery();
                        tran.Commit();
                        return rows > 0;
                    }
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }
}