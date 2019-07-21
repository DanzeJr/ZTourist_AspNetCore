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
    public class DestinationDAL
    {
        private readonly string connectionStr;

        public DestinationDAL(IConfiguration configuration)
        {
            this.connectionStr = configuration["Data:ZTouristDB:ConnectionString"];
        }
        
        #region DestinationDAL

        public async Task<bool> IsExistedDestinationIdAsync(string id)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spIsExistedDestinationId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = true;
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

        public async Task<bool> IsAvailableDestinationAsync(string id)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spIsAvailableDestination", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = true;
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

        public async Task<Destination> FindDestinationByIdAsync(string id, bool? isActive = null)
        {
            Destination result = null;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spFindDestinationById", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (isActive == true)
                    {
                        cmd.Parameters.AddWithValue("@IsActive", true);
                        cmd.Parameters.AddWithValue("@IsActiveCheck", true);
                    }
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (await sdr.ReadAsync())
                        {
                            result = new Destination { Id = id };
                            result.Name = sdr.GetString(sdr.GetOrdinal("Name"));
                            result.Image = sdr.GetString(sdr.GetOrdinal("Image"));
                            result.Description = sdr.GetString(sdr.GetOrdinal("Description"));
                            result.Country = sdr.GetString(sdr.GetOrdinal("Country"));
                            result.IsActive = sdr.GetBoolean(sdr.GetOrdinal("IsActive"));
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

        public async Task<Dictionary<string, string>> GetDestinationsIdNameAsync(bool? isActive = true)
        {
            Dictionary<string, string> result = null;
            string id;
            string name;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetDestinationsIdName", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (isActive == null)
                    {
                        cmd.Parameters.AddWithValue("@IsActive", true);
                        cmd.Parameters.AddWithValue("@IsActiveCheck", false);
                    }
                    else if (isActive == false)
                    {
                        cmd.Parameters.AddWithValue("@IsActive", false);
                        cmd.Parameters.AddWithValue("@IsActiveCheck", false);
                    }
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = new Dictionary<string, string>();
                            while (await sdr.ReadAsync())
                            {
                                id = sdr.GetString(sdr.GetOrdinal("Id"));
                                name = sdr.GetString(sdr.GetOrdinal("Name"));
                                result.Add(id, name);
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

        public async Task<int> GetTotalDestinationsAsync(bool? isActive)
        {
            int result = 0;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetTotalDestinations", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (isActive == null)
                    {
                        cmd.Parameters.AddWithValue("@IsActive", true);
                        cmd.Parameters.AddWithValue("@IsActiveCheck", false);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        cmd.Parameters.AddWithValue("@IsActiveCheck", isActive);
                    }
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

        public async Task<IEnumerable<Destination>> GetAllDestinationsAsync(bool? isActive, int skip, int fetch)
        {
            List<Destination> result = null;
            Destination destination;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetAllDestinations", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (isActive == null)
                    {
                        cmd.Parameters.AddWithValue("@IsActive", true);
                        cmd.Parameters.AddWithValue("@IsActiveCheck", false);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        cmd.Parameters.AddWithValue("@IsActiveCheck", isActive);
                    }
                    cmd.Parameters.AddWithValue("@Skip", skip);
                    cmd.Parameters.AddWithValue("@Fetch", fetch);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Destination>();
                            while (await sdr.ReadAsync())
                            {
                                destination = new Destination();
                                destination.Id = sdr.GetString(sdr.GetOrdinal("Id"));
                                destination.Name = sdr.GetString(sdr.GetOrdinal("Name"));
                                destination.Image = sdr.GetString(sdr.GetOrdinal("Image"));
                                destination.Country = sdr.GetString(sdr.GetOrdinal("Country"));
                                result.Add(destination);
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

        public async Task<bool> AddDestinationAsync(Destination destination)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spAddDestination", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", destination.Id);
                    cmd.Parameters.AddWithValue("@Name", destination.Name);
                    cmd.Parameters.AddWithValue("@Image", destination.Image);
                    cmd.Parameters.AddWithValue("@Description", destination.Description);
                    cmd.Parameters.AddWithValue("@Country", destination.Country);
                    cmd.Parameters.AddWithValue("@IsActive", destination.IsActive);
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

        public async Task<bool> UpdateDestinationAsync(Destination destination)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateDestination", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", destination.Id);
                    cmd.Parameters.AddWithValue("@Name", destination.Name);
                    cmd.Parameters.AddWithValue("@Image", destination.Image);
                    cmd.Parameters.AddWithValue("@Description", destination.Description);
                    cmd.Parameters.AddWithValue("@Country", destination.Country);
                    cmd.Parameters.AddWithValue("@IsActive", destination.IsActive);
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

        public async Task<bool> DeleteDestinationByIdAsync(string id)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteDestinationById", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
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

        public async Task<IEnumerable<string>> GetToursByDestinationIdAsync(string id)
        {
            List<string> result = null;
            string tourId;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetToursByDestinationId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<string>();
                            while (await sdr.ReadAsync())
                            {
                                tourId = sdr.GetString(sdr.GetOrdinal("TourId"));
                                result.Add(tourId);
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

        public async Task<IEnumerable<string>> FindFutureToursByDestinationIdAsync(string id)
        {
            List<string> result = null;
            string tourId;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spFindFutureToursByDestinationId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<string>();
                            while (await sdr.ReadAsync())
                            {
                                tourId = sdr.GetString(sdr.GetOrdinal("TourId"));
                                result.Add(tourId);
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

        #endregion
    }
}
