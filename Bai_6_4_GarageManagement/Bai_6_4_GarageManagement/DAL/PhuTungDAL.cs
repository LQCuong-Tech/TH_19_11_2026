using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bai_6_4_GarageManagement.DTO;

namespace Bai_6_4_GarageManagement.DAL
{
    public class PhuTungDAL
    {
        public List<PhuTungDTO> LayTatCa()
        {
            var list = new List<PhuTungDTO>();
            string sql = "SELECT MaPhuTung, TenPhuTung, DonGia, SoLuongTon FROM PhuTung ORDER BY TenPhuTung";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new PhuTungDTO
                        {
                            MaPhuTung = Convert.ToInt32(rd["MaPhuTung"]),
                            TenPhuTung = rd["TenPhuTung"].ToString(),
                            DonGia = Convert.ToDecimal(rd["DonGia"]),
                            SoLuongTon = Convert.ToInt32(rd["SoLuongTon"])
                        });
                    }
                }
            }
            return list;
        }

        public bool Them(PhuTungDTO pt)
        {
            string sql = "INSERT INTO PhuTung (TenPhuTung, DonGia, SoLuongTon) VALUES (@TenPhuTung, @DonGia, @SoLuongTon)";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TenPhuTung", pt.TenPhuTung);
                cmd.Parameters.AddWithValue("@DonGia", pt.DonGia);
                cmd.Parameters.AddWithValue("@SoLuongTon", pt.SoLuongTon);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Sua(PhuTungDTO pt)
        {
            string sql = "UPDATE PhuTung SET TenPhuTung = @TenPhuTung, DonGia = @DonGia, SoLuongTon = @SoLuongTon WHERE MaPhuTung = @MaPhuTung";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaPhuTung", pt.MaPhuTung);
                cmd.Parameters.AddWithValue("@TenPhuTung", pt.TenPhuTung);
                cmd.Parameters.AddWithValue("@DonGia", pt.DonGia);
                cmd.Parameters.AddWithValue("@SoLuongTon", pt.SoLuongTon);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Xoa(int maPhuTung)
        {
            string sql = "DELETE FROM PhuTung WHERE MaPhuTung = @MaPhuTung";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaPhuTung", maPhuTung);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}