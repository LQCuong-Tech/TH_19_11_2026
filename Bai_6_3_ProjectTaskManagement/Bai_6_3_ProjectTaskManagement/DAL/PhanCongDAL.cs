using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bai_6_3_ProjectTaskManagement.DTO;

namespace Bai_6_3_ProjectTaskManagement.DAL
{
    public class PhanCongDAL
    {
        public List<int> LayMaThanhVienTheoTask(int maTask)
        {
            var list = new List<int>();
            string sql = "SELECT MaThanhVien FROM PhanCong WHERE MaTask = @MaTask";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaTask", maTask);
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(Convert.ToInt32(rd["MaThanhVien"]));
                    }
                }
            }
            return list;
        }

        // Giao việc cho một hoặc nhiều thành viên dùng SqlTransaction
        public bool LuuPhanCongNhieuThanhVien(int maTask, List<int> dsMaThanhVien)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // Xóa phân công cũ của Task trước khi ghi danh sách mới
                    string sqlDelete = "DELETE FROM PhanCong WHERE MaTask = @MaTask";
                    using (SqlCommand cmdDel = new SqlCommand(sqlDelete, conn, tran))
                    {
                        cmdDel.Parameters.AddWithValue("@MaTask", maTask);
                        cmdDel.ExecuteNonQuery();
                    }

                    // Thêm từng thành viên được chọn
                    string sqlInsert = "INSERT INTO PhanCong (MaTask, MaThanhVien, NgayGiao) VALUES (@MaTask, @MaThanhVien, GETDATE())";
                    foreach (int maTV in dsMaThanhVien)
                    {
                        using (SqlCommand cmdIns = new SqlCommand(sqlInsert, conn, tran))
                        {
                            cmdIns.Parameters.AddWithValue("@MaTask", maTask);
                            cmdIns.Parameters.AddWithValue("@MaThanhVien", maTV);
                            cmdIns.ExecuteNonQuery();
                        }
                    }

                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        // Yêu cầu 3.2 (Cách 1 - GROUP BY): Thống kê tỷ lệ % hoàn thành của từng thành viên trong dự án
        public List<ThongKeDTO> ThongKeTyLeHoanThanhTheoDuAn(int maDuAn)
        {
            var list = new List<ThongKeDTO>();
            string sql = @"
                SELECT tv.MaThanhVien,
                       tv.HoTen,
                       tv.ChuyenMon,
                       COUNT(cv.MaTask) AS TongTask,
                       SUM(CASE WHEN cv.TrangThai = N'Done' THEN 1 ELSE 0 END) AS TaskHoanThanh
                FROM ThanhVien tv
                JOIN PhanCong pc ON tv.MaThanhVien = pc.MaThanhVien
                JOIN CongViec cv ON pc.MaTask = cv.MaTask
                WHERE cv.MaDuAn = @MaDuAn
                GROUP BY tv.MaThanhVien, tv.HoTen, tv.ChuyenMon
                ORDER BY TaskHoanThanh DESC, tv.HoTen ASC";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaDuAn", maDuAn);
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        int tong = Convert.ToInt32(rd["TongTask"]);
                        int done = Convert.ToInt32(rd["TaskHoanThanh"]);
                        double tyLe = tong > 0 ? Math.Round((double)done * 100 / tong, 2) : 0;

                        list.Add(new ThongKeDTO
                        {
                            MaThanhVien = Convert.ToInt32(rd["MaThanhVien"]),
                            HoTen = rd["HoTen"].ToString(),
                            ChuyenMon = rd["ChuyenMon"].ToString(),
                            TongTask = tong,
                            TaskHoanThanh = done,
                            TyLeHoanThanh = tyLe
                        });
                    }
                }
            }
            return list;
        }

        // Yêu cầu 3.2 (Cách 2 - ExecuteScalar): Tính trực tiếp % hoàn thành của 1 thành viên cụ thể trong dự án
        public double TinhTyLeHoanThanhExecuteScalar(int maDuAn, int maThanhVien)
        {
            string sqlTong = @"SELECT COUNT(*) FROM PhanCong pc
                               JOIN CongViec cv ON pc.MaTask = cv.MaTask
                               WHERE cv.MaDuAn = @MaDuAn AND pc.MaThanhVien = @MaTV";

            string sqlDone = @"SELECT COUNT(*) FROM PhanCong pc
                               JOIN CongViec cv ON pc.MaTask = cv.MaTask
                               WHERE cv.MaDuAn = @MaDuAn AND pc.MaThanhVien = @MaTV AND cv.TrangThai = N'Done'";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();

                int tongTask = 0;
                using (SqlCommand cmd1 = new SqlCommand(sqlTong, conn))
                {
                    cmd1.Parameters.AddWithValue("@MaDuAn", maDuAn);
                    cmd1.Parameters.AddWithValue("@MaTV", maThanhVien);
                    tongTask = Convert.ToInt32(cmd1.ExecuteScalar());
                }

                if (tongTask == 0) return 0;

                int taskDone = 0;
                using (SqlCommand cmd2 = new SqlCommand(sqlDone, conn))
                {
                    cmd2.Parameters.AddWithValue("@MaDuAn", maDuAn);
                    cmd2.Parameters.AddWithValue("@MaTV", maThanhVien);
                    taskDone = Convert.ToInt32(cmd2.ExecuteScalar());
                }

                return Math.Round((double)taskDone * 100 / tongTask, 2);
            }
        }
    }
}