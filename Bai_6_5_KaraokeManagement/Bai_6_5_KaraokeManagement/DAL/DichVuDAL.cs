using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bai_6_5_KaraokeManagement.DTO;

namespace Bai_6_5_KaraokeManagement.DAL
{
    public class DichVuDAL
    {
        public List<DichVuDTO> LayTatCa()
        {
            var list = new List<DichVuDTO>();
            string sql = "SELECT MaDV, TenDV, DonGia, TonKho FROM DichVu ORDER BY TenDV ASC";

            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new DichVuDTO
                        {
                            MaDV = Convert.ToInt32(rd["MaDV"]),
                            TenDV = rd["TenDV"].ToString(),
                            DonGia = Convert.ToDecimal(rd["DonGia"]),
                            TonKho = Convert.ToInt32(rd["TonKho"])
                        });
                    }
                }
            }
            return list;
        }

        public bool Them(DichVuDTO dv)
        {
            string sql = "INSERT INTO DichVu (TenDV, DonGia, TonKho) VALUES (@TenDV, @DonGia, @TonKho)";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TenDV", dv.TenDV);
                cmd.Parameters.AddWithValue("@DonGia", dv.DonGia);
                cmd.Parameters.AddWithValue("@TonKho", dv.TonKho);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Sua(DichVuDTO dv)
        {
            string sql = "UPDATE DichVu SET TenDV = @TenDV, DonGia = @DonGia, TonKho = @TonKho WHERE MaDV = @MaDV";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaDV", dv.MaDV);
                cmd.Parameters.AddWithValue("@TenDV", dv.TenDV);
                cmd.Parameters.AddWithValue("@DonGia", dv.DonGia);
                cmd.Parameters.AddWithValue("@TonKho", dv.TonKho);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Xoa(int maDV)
        {
            string sql = "DELETE FROM DichVu WHERE MaDV = @MaDV";
            using (SqlConnection conn = DbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaDV", maDV);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}