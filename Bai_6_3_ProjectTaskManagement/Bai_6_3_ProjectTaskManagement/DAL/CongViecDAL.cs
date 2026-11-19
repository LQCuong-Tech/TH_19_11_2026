using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bai_6_3_ProjectTaskManagement.DTO;

namespace Bai_6_3_ProjectTaskManagement.DAL
{
    public class CongViecDAL
    {
        public List<CongViecDTO> LayDanhSachTask(int? maDuAn = null, string trangThai = null)
        {
            var list = new List<CongViecDTO>();
            string sql = @"
                SELECT cv.MaTask, cv.MaDuAn, da.TenDuAn, cv.TenTask, cv.MoTa,
                       cv.HanNop, cv.DoUuTien, cv.TrangThai,
                       ISNULL(STUFF((
                           SELECT ', ' + tv.HoTen
                           FROM PhanCong pc
                           JOIN ThanhVien tv ON pc.MaThanhVien = tv.MaThanhVien
                           WHERE pc.MaTask = cv.MaTask
                           FOR XML PATH(''), TYPE
                       ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''), N'Chưa phân công') AS NguoiThucHien
                FROM CongViec cv
                JOIN DuAn da ON cv.MaDuAn = da.MaDuAn
                WHERE (@MaDuAn IS NULL OR cv.MaDuAn = @MaDuAn)
                  AND (@TrangThai IS NULL OR @TrangThai = N'Tất cả' OR cv.TrangThai = @TrangThai)
                ORDER BY cv.HanNop ASC";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaDuAn", (object)maDuAn ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", string.IsNullOrEmpty(trangThai) ? (object)DBNull.Value : trangThai);

                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new CongViecDTO
                        {
                            MaTask = Convert.ToInt32(rd["MaTask"]),
                            MaDuAn = Convert.ToInt32(rd["MaDuAn"]),
                            TenDuAn = rd["TenDuAn"].ToString(),
                            TenTask = rd["TenTask"].ToString(),
                            MoTa = rd["MoTa"]?.ToString(),
                            HanNop = Convert.ToDateTime(rd["HanNop"]),
                            DoUuTien = rd["DoUuTien"].ToString(),
                            TrangThai = rd["TrangThai"].ToString(),
                            NguoiThucHien = rd["NguoiThucHien"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public int ThemVaLayMa(CongViecDTO cv)
        {
            string sql = @"INSERT INTO CongViec (MaDuAn, TenTask, MoTa, HanNop, DoUuTien, TrangThai)
                           VALUES (@MaDuAn, @TenTask, @MoTa, @HanNop, @DoUuTien, @TrangThai);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaDuAn", cv.MaDuAn);
                cmd.Parameters.AddWithValue("@TenTask", cv.TenTask);
                cmd.Parameters.AddWithValue("@MoTa", (object)cv.MoTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@HanNop", cv.HanNop);
                cmd.Parameters.AddWithValue("@DoUuTien", cv.DoUuTien);
                cmd.Parameters.AddWithValue("@TrangThai", cv.TrangThai);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Sua(CongViecDTO cv)
        {
            string sql = @"UPDATE CongViec
                           SET MaDuAn = @MaDuAn, TenTask = @TenTask, MoTa = @MoTa,
                               HanNop = @HanNop, DoUuTien = @DoUuTien, TrangThai = @TrangThai
                           WHERE MaTask = @MaTask";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaTask", cv.MaTask);
                cmd.Parameters.AddWithValue("@MaDuAn", cv.MaDuAn);
                cmd.Parameters.AddWithValue("@TenTask", cv.TenTask);
                cmd.Parameters.AddWithValue("@MoTa", (object)cv.MoTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@HanNop", cv.HanNop);
                cmd.Parameters.AddWithValue("@DoUuTien", cv.DoUuTien);
                cmd.Parameters.AddWithValue("@TrangThai", cv.TrangThai);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Yêu cầu 2: Chuyển trạng thái qua lệnh Parameterized SQL Update
        public bool CapNhatTrangThai(int maTask, string trangThaiMoi)
        {
            string sql = "UPDATE CongViec SET TrangThai = @TrangThai WHERE MaTask = @MaTask";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TrangThai", trangThaiMoi);
                cmd.Parameters.AddWithValue("@MaTask", maTask);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Xoa(int maTask)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    using (SqlCommand cmd1 = new SqlCommand("DELETE FROM PhanCong WHERE MaTask = @MaTask", conn, tran))
                    {
                        cmd1.Parameters.AddWithValue("@MaTask", maTask);
                        cmd1.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd2 = new SqlCommand("DELETE FROM CongViec WHERE MaTask = @MaTask", conn, tran))
                    {
                        cmd2.Parameters.AddWithValue("@MaTask", maTask);
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

        // Yêu cầu 3.1: Dùng SqlDataReader hiển thị các công việc đã quá HanNop mà TrangThai != 'Done'
        public List<CongViecDTO> LayTaskTreHan()
        {
            var list = new List<CongViecDTO>();
            string sql = @"
                SELECT cv.MaTask, cv.MaDuAn, da.TenDuAn, cv.TenTask, cv.MoTa,
                       cv.HanNop, cv.DoUuTien, cv.TrangThai,
                       ISNULL(STUFF((
                           SELECT ', ' + tv.HoTen
                           FROM PhanCong pc
                           JOIN ThanhVien tv ON pc.MaThanhVien = tv.MaThanhVien
                           WHERE pc.MaTask = cv.MaTask
                           FOR XML PATH(''), TYPE
                       ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''), N'Chưa phân công') AS NguoiThucHien
                FROM CongViec cv
                JOIN DuAn da ON cv.MaDuAn = da.MaDuAn
                WHERE cv.HanNop < GETDATE() AND cv.TrangThai <> N'Done'
                ORDER BY cv.HanNop ASC";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CongViecDTO
                        {
                            MaTask = reader.GetInt32(reader.GetOrdinal("MaTask")),
                            MaDuAn = reader.GetInt32(reader.GetOrdinal("MaDuAn")),
                            TenDuAn = reader["TenDuAn"].ToString(),
                            TenTask = reader["TenTask"].ToString(),
                            MoTa = reader["MoTa"]?.ToString(),
                            HanNop = reader.GetDateTime(reader.GetOrdinal("HanNop")),
                            DoUuTien = reader["DoUuTien"].ToString(),
                            TrangThai = reader["TrangThai"].ToString(),
                            NguoiThucHien = reader["NguoiThucHien"].ToString()
                        });
                    }
                }
            }
            return list;
        }
    }
}