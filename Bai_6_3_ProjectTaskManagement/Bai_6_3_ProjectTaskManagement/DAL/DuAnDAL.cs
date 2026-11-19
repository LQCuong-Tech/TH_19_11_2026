using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bai_6_3_ProjectTaskManagement.DTO;

namespace Bai_6_3_ProjectTaskManagement.DAL
{
    public class DuAnDAL
    {
        public List<DuAnDTO> LayTatCa()
        {
            var list = new List<DuAnDTO>();
            string sql = "SELECT MaDuAn, TenDuAn, NgayBatDau, NgayKetThuc, TrangThai FROM DuAn ORDER BY MaDuAn DESC";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new DuAnDTO
                        {
                            MaDuAn = Convert.ToInt32(rd["MaDuAn"]),
                            TenDuAn = rd["TenDuAn"].ToString(),
                            NgayBatDau = Convert.ToDateTime(rd["NgayBatDau"]),
                            NgayKetThuc = Convert.ToDateTime(rd["NgayKetThuc"]),
                            TrangThai = rd["TrangThai"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public bool Them(DuAnDTO da)
        {
            string sql = @"INSERT INTO DuAn (TenDuAn, NgayBatDau, NgayKetThuc, TrangThai)
                           VALUES (@TenDuAn, @NgayBatDau, @NgayKetThuc, @TrangThai)";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TenDuAn", da.TenDuAn);
                cmd.Parameters.AddWithValue("@NgayBatDau", da.NgayBatDau);
                cmd.Parameters.AddWithValue("@NgayKetThuc", da.NgayKetThuc);
                cmd.Parameters.AddWithValue("@TrangThai", da.TrangThai);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Sua(DuAnDTO da)
        {
            string sql = @"UPDATE DuAn
                           SET TenDuAn = @TenDuAn, NgayBatDau = @NgayBatDau,
                               NgayKetThuc = @NgayKetThuc, TrangThai = @TrangThai
                           WHERE MaDuAn = @MaDuAn";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaDuAn", da.MaDuAn);
                cmd.Parameters.AddWithValue("@TenDuAn", da.TenDuAn);
                cmd.Parameters.AddWithValue("@NgayBatDau", da.NgayBatDau);
                cmd.Parameters.AddWithValue("@NgayKetThuc", da.NgayKetThuc);
                cmd.Parameters.AddWithValue("@TrangThai", da.TrangThai);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Xoa(int maDuAn)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // Xóa phân công của các task thuộc dự án
                    string sqlDelPC = @"DELETE FROM PhanCong WHERE MaTask IN
                                        (SELECT MaTask FROM CongViec WHERE MaDuAn = @MaDuAn)";
                    using (SqlCommand cmd1 = new SqlCommand(sqlDelPC, conn, tran))
                    {
                        cmd1.Parameters.AddWithValue("@MaDuAn", maDuAn);
                        cmd1.ExecuteNonQuery();
                    }

                    // Xóa các task thuộc dự án
                    string sqlDelCV = "DELETE FROM CongViec WHERE MaDuAn = @MaDuAn";
                    using (SqlCommand cmd2 = new SqlCommand(sqlDelCV, conn, tran))
                    {
                        cmd2.Parameters.AddWithValue("@MaDuAn", maDuAn);
                        cmd2.ExecuteNonQuery();
                    }

                    // Xóa dự án
                    string sqlDelDA = "DELETE FROM DuAn WHERE MaDuAn = @MaDuAn";
                    using (SqlCommand cmd3 = new SqlCommand(sqlDelDA, conn, tran))
                    {
                        cmd3.Parameters.AddWithValue("@MaDuAn", maDuAn);
                        int rows = cmd3.ExecuteNonQuery();
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