using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ZTourist.Models
{
    public class OrderDAL
    {
        private readonly string connectionStr;
        private readonly ILogger logger;

        public OrderDAL(IConfiguration configuration, ILogger logger)
        {
            this.connectionStr = configuration["Data:ZTouristDB:ConnectionString"];
            this.logger = logger;
        }

        #region OrderDAL

        public async Task<bool> AddOrderAsync(Order order)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlTransaction transaction = cnn.BeginTransaction())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = cnn;
                            cmd.Transaction = transaction;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spAddOrder";
                            try
                            {
                                cmd.Parameters.AddWithValue("@Id", order.Id);
                                cmd.Parameters.AddWithValue("@UserId", order.Customer.Id);
                                cmd.Parameters.AddWithValue("@CouponCode", order.Cart.Coupon?.Code);
                                cmd.Parameters.AddWithValue("@Comment", order.Comment);
                                cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                                cmd.Parameters.AddWithValue("@Status", order.Status);
                                result = await cmd.ExecuteNonQueryAsync() > 0;

                                if (result) // if insert order customer successfully, then insert order details
                                {
                                    result = false;
                                    cmd.CommandText = "spAddOrderDetail";
                                    foreach (CartLine line in order.Cart.Lines)
                                    {
                                        result = false;
                                        cmd.Parameters.Clear();
                                        cmd.Parameters.AddWithValue("@OrderId", order.Id);
                                        cmd.Parameters.AddWithValue("@TourId", line.Tour.Id);
                                        cmd.Parameters.AddWithValue("@AdultTicket", line.AdultTicket);
                                        cmd.Parameters.AddWithValue("@KidTicket", line.KidTicket);
                                        result = await cmd.ExecuteNonQueryAsync() > 0;
                                        if (!result)
                                        {
                                            transaction.Rollback(); // if insert order detail failed then roll back transaction
                                            break; // stop for to return
                                        }
                                    }
                                }

                                transaction.Commit(); // commit transaction whether insert order is successful or not
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex.Message);
                                try
                                {
                                    transaction.Rollback(); //if an exception is throw, roll back transaction
                                }
                                catch (Exception rollbackEx)
                                {
                                    logger.LogError(rollbackEx.Message);
                                }
                            }
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

        public async Task<Order> FindOrderByIdAsync(string id)
        {
            Order result = null;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spFindOrderById", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (await sdr.ReadAsync())
                        {
                            result = new Order { Id = id.ToUpper(), Customer = new AppUser(), Cart = new Cart { Coupon = new CouponCode() } };
                            result.Customer.Id = sdr.GetString(sdr.GetOrdinal("UserId"));
                            if (!sdr.IsDBNull(sdr.GetOrdinal("CouponCode")))
                                result.Cart.Coupon.Code = sdr.GetString(sdr.GetOrdinal("CouponCode"));
                            result.Comment = sdr.GetString(sdr.GetOrdinal("Comment"));
                            result.OrderDate = sdr.GetDateTime(sdr.GetOrdinal("OrderDate"));
                            result.Status = sdr.GetString(sdr.GetOrdinal("Status"));
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

        public async Task<int> GetTotalOrdersByUserIdAsync(string userId)
        {
            int result = 0;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetTotalOrdersByUserId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (await sdr.ReadAsync())
                        {
                            result = sdr.GetInt32(sdr.GetOrdinal("Total"));
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

        public async Task<IEnumerable<Order>> FindOrdersByUserIdAsync(string userId, int skip, int fetch)
        {
            List<Order> result = null;
            Order order;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spFindOrdersByUserId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Skip", skip);
                    cmd.Parameters.AddWithValue("@Fetch", fetch);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Order>();
                            while (await sdr.ReadAsync())
                            {
                                order = new Order { Cart = new Cart { Coupon = new CouponCode() } };
                                order.Id = sdr.GetString(sdr.GetOrdinal("Id"));
                                if (!sdr.IsDBNull(sdr.GetOrdinal("CouponCode")))
                                    order.Cart.Coupon.Code = sdr.GetString(sdr.GetOrdinal("CouponCode"));
                                order.OrderDate = sdr.GetDateTime(sdr.GetOrdinal("OrderDate"));
                                order.Status = sdr.GetString(sdr.GetOrdinal("Status"));
                                result.Add(order);
                            }
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

        public async Task<int> GetTotalOrdersByStatusAsync(string status)
        {
            int result = 0;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetTotalOrdersByStatus", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Status", status);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (await sdr.ReadAsync())
                        {
                            result = sdr.GetInt32(sdr.GetOrdinal("Total"));
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

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status, int skip, int fetch)
        {
            List<Order> result = null;
            Order order;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetOrdersByStatus", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Skip", skip);
                    cmd.Parameters.AddWithValue("@Fetch", fetch);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Order>();
                            while (await sdr.ReadAsync())
                            {
                                order = new Order { Customer = new AppUser(), Cart = new Cart { Coupon = new CouponCode() } };
                                order.Id = sdr.GetString(sdr.GetOrdinal("Id"));
                                order.Customer.UserName = sdr.GetString(sdr.GetOrdinal("UserName"));
                                if (!sdr.IsDBNull(sdr.GetOrdinal("CouponCode")))
                                    order.Cart.Coupon.Code = sdr.GetString(sdr.GetOrdinal("CouponCode"));
                                order.OrderDate = sdr.GetDateTime(sdr.GetOrdinal("OrderDate"));
                                order.Status = sdr.GetString(sdr.GetOrdinal("Status"));
                                result.Add(order);
                            }
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

        public async Task<List<CartLine>> GetDetailsByOrderIdAsync(string id)
        {
            List<CartLine> result = null;
            CartLine line;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetDetailsByOrderId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<CartLine>();
                            while (await sdr.ReadAsync())
                            {
                                line = new CartLine { Tour = new Tour() };
                                line.Tour.Id = sdr.GetString(sdr.GetOrdinal("TourId"));
                                line.Tour.Name = sdr.GetString(sdr.GetOrdinal("Name"));
                                line.Tour.AdultFare = sdr.GetDecimal(sdr.GetOrdinal("AdultFare"));
                                line.Tour.KidFare = sdr.GetDecimal(sdr.GetOrdinal("KidFare"));
                                line.Tour.FromDate = sdr.GetDateTime(sdr.GetOrdinal("FromDate"));
                                line.AdultTicket = sdr.GetInt32(sdr.GetOrdinal("AdultTicket"));
                                line.KidTicket = sdr.GetInt32(sdr.GetOrdinal("KidTicket"));
                                result.Add(line);
                            }
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

        public async Task<bool> UpdateOrderStatusById(string id, string status)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateOrderStatusById", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Status", status);
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
