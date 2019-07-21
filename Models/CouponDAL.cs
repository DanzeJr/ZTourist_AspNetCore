using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ZTourist.Models.ViewModels;

namespace ZTourist.Models
{
    public class CouponDAL
    {
        private readonly string connectionStr;

        public CouponDAL(IConfiguration configuration)
        {
            this.connectionStr = configuration["Data:ZTouristDB:ConnectionString"];
        }
               
        #region CouponDAL

        public async Task<bool> AddCouponCodeAsync(CouponCode coupon)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spAddCouponCode", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Code", coupon.Code);
                    cmd.Parameters.AddWithValue("@OffPercent", coupon.OffPercent);
                    cmd.Parameters.AddWithValue("@StartDate", coupon.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", coupon.EndDate);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    result = await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<bool> UpdateCouponCodeAsync(CouponCode coupon)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateCouponCode", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Code", coupon.Code);
                    cmd.Parameters.AddWithValue("@OffPercent", coupon.OffPercent);
                    cmd.Parameters.AddWithValue("@StartDate", coupon.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", coupon.EndDate);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    result = await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<CouponCode> FindCouponByCodeAsync(string code, DateTime? date = null)
        {
            CouponCode result = null;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spFindCouponByCode", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (date == null)
                        date = DateTime.Now;
                    cmd.Parameters.AddWithValue("@Code", code);
                    cmd.Parameters.AddWithValue("@Date", date);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (await sdr.ReadAsync())
                        {
                            result = new CouponCode { Code = code.ToUpper() };
                            result.OffPercent = sdr.GetInt32(sdr.GetOrdinal("OffPercent"));
                            result.StartDate = sdr.GetDateTime(sdr.GetOrdinal("StartDate"));
                            result.EndDate = sdr.GetDateTime(sdr.GetOrdinal("EndDate"));
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<bool> DeleteCouponByCodeAsync(string code)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteCouponByCode", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Code", code);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    result = await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        #endregion
    }
}
