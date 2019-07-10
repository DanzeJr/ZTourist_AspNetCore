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
    public class TouristDAL
    {
        private readonly string connectionStr;

        public TouristDAL(IConfiguration configuration)
        {
            this.connectionStr = configuration["Data:ZTouristDB:ConnectionString"];
        }

        #region TourDAL

        public async Task<bool> IsExistedTourIdAsync(string id)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spIsExistedTourId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
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

        public async Task<Tour> FindTourByTourIdAsync(string id)
        {
            Tour result = null;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spFindTourByTourId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.Read())
                        {
                            result = new Tour { Id = id };
                            result.Name = sdr.GetString(sdr.GetOrdinal("Name"));
                            result.Image = sdr.GetString(sdr.GetOrdinal("Image"));
                            result.FromDate = sdr.GetDateTime(sdr.GetOrdinal("FromDate"));
                            result.AdultFare = sdr.GetDecimal(sdr.GetOrdinal("AdultFare"));
                            result.KidFare = sdr.GetDecimal(sdr.GetOrdinal("KidFare"));
                            result.MaxGuest = sdr.GetInt32(sdr.GetOrdinal("MaxGuest"));
                            result.Duration = sdr.GetInt32(sdr.GetOrdinal("Duration"));
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

        public async Task<Tour> FindTourByTourIdEmpAsync(string id)
        {
            Tour result = null;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spFindTourByTourIdEmp", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.Read())
                        {
                            result = new Tour();
                            result.Name = sdr.GetString(sdr.GetOrdinal("Name"));
                            result.Description = sdr.GetString(sdr.GetOrdinal("Description"));
                            result.Image = sdr.GetString(sdr.GetOrdinal("Image"));
                            result.FromDate = sdr.GetDateTime(sdr.GetOrdinal("FromDate"));
                            result.ToDate = sdr.GetDateTime(sdr.GetOrdinal("ToDate"));
                            result.AdultFare = sdr.GetDecimal(sdr.GetOrdinal("AdultFare"));
                            result.KidFare = sdr.GetDecimal(sdr.GetOrdinal("KidFare"));
                            result.MaxGuest = sdr.GetInt32(sdr.GetOrdinal("MaxGuest"));
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

        public async Task<Tour> GetTourInCartByTourIdAsync(string id)
        {
            Tour result = null;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spFindTourInCartByTourId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.Read())
                        {
                            result = new Tour();
                            result.Name = sdr.GetString(sdr.GetOrdinal("Name"));
                            result.Image = sdr.GetString(sdr.GetOrdinal("Image"));
                            result.AdultFare = sdr.GetDecimal(sdr.GetOrdinal("AdultFare"));
                            result.KidFare = sdr.GetDecimal(sdr.GetOrdinal("KidFare"));
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

        public async Task<int> GetMaxGuestByTourIdAsync(string id)
        {
            int result = 0;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetMaxGuestByTourId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.Read())
                        {
                            result = sdr.GetInt32(sdr.GetOrdinal("MaxGuest"));
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

        public async Task<int> GetTakenSlotByTourIdAsync(string id)
        {
            int result = 0;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetTakenSlotByTourId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.Read())
                        {
                            result = sdr.GetInt32(sdr.GetOrdinal("TakenSlot"));
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

        public async Task<IEnumerable<Tour>> GetTrendingToursAsync()
        {
            List<Tour> result = null;
            Tour tour;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetTrendingTours", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Tour>();
                            while (sdr.Read())
                            {
                                tour = new Tour();
                                tour.Id = sdr.GetString(sdr.GetOrdinal("TourId"));
                                tour.Image = sdr.GetString(sdr.GetOrdinal("Image"));
                                tour.AdultFare = sdr.GetDecimal(sdr.GetOrdinal("AdultFare"));
                                tour.Duration = sdr.GetInt32(sdr.GetOrdinal("Duration"));
                                result.Add(tour);
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

        public async Task<IEnumerable<Tour>> GetNearestToursAsync()
        {
            List<Tour> result = null;
            Tour tour;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetNearestTours", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Tour>();
                            while (sdr.Read())
                            {
                                tour = new Tour();
                                tour.Id = sdr.GetString(sdr.GetOrdinal("Id"));
                                tour.Image = sdr.GetString(sdr.GetOrdinal("Image"));
                                tour.AdultFare = sdr.GetDecimal(sdr.GetOrdinal("AdultFare"));
                                tour.Duration = sdr.GetInt32(sdr.GetOrdinal("Duration"));
                                result.Add(tour);
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

        public async Task<IEnumerable<Destination>> FindDestinationsByTourIdAsync(string id)
        {
            List<Destination> result = null;
            Destination destination;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spFindDestinationsByTourId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Destination>();
                            while (sdr.Read())
                            {
                                destination = new Destination();
                                destination.Id = sdr.GetString(sdr.GetOrdinal("DestinationId"));
                                destination.Name = sdr.GetString(sdr.GetOrdinal("Name"));
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

        public async Task<Destination> GetFinalDestinationByTourIdAsync(string id)
        {
            Destination result = null;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spFindFinalDestinationByTourId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.Read())
                        {
                            result = new Destination();
                            result.Name = sdr.GetString(sdr.GetOrdinal("Name"));
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

        public async Task<IEnumerable<AppUser>> FindGuidesByTourIdAsync(string id)
        {
            List<AppUser> result = null;
            AppUser user;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spFindGuidesByTourId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<AppUser>();
                            while (sdr.Read())
                            {
                                user = new AppUser();
                                user.UserName = sdr.GetString(sdr.GetOrdinal("UserName"));
                                user.FirstName = sdr.GetString(sdr.GetOrdinal("FirstName"));
                                user.LastName = sdr.GetString(sdr.GetOrdinal("LastName"));
                                result.Add(user);
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

        public async Task<int> GetTotalToursAsync()
        {
            int result = 0;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetTotalTours", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.Read())
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

        public async Task<IEnumerable<Tour>> GetAllToursAsync(int skip, int fetch)
        {
            List<Tour> result = null;
            Tour tour;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetAllTours", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Skip", skip);
                    cmd.Parameters.AddWithValue("@Fetch", fetch);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Tour>();
                            while (sdr.Read())
                            {
                                tour = new Tour();
                                tour.Id = sdr.GetString(sdr.GetOrdinal("Id"));
                                tour.Name = sdr.GetString(sdr.GetOrdinal("Name"));
                                tour.Description = sdr.GetString(sdr.GetOrdinal("Description"));
                                tour.FromDate = sdr.GetDateTime(sdr.GetOrdinal("FromDate"));
                                tour.ToDate = sdr.GetDateTime(sdr.GetOrdinal("ToDate"));
                                tour.Transport = sdr.GetString(sdr.GetOrdinal("Transport"));
                                tour.AdultFare = sdr.GetDecimal(sdr.GetOrdinal("AdultFare"));
                                tour.Image = sdr.GetString(sdr.GetOrdinal("Image"));
                                tour.Duration = sdr.GetInt32(sdr.GetOrdinal("Duration"));
                                result.Add(tour);
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

        public async Task<int> GetTotalSearchToursAsync(TourViewModel search)
        {
            int result = 0;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd;
                    if (string.IsNullOrEmpty(search.Destination))
                    {
                        cmd = new SqlCommand("spGetTotalSearchTours", cnn);
                    }
                    else
                    {
                        cmd = new SqlCommand("spGetTotalSearchToursIncludeDestinationId", cnn);
                    }                    
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (!string.IsNullOrEmpty(search.Id))
                        cmd.Parameters.AddWithValue("@Id", search.Id);
                    if (!string.IsNullOrEmpty(search.Name))
                        cmd.Parameters.AddWithValue("@Name", search.Name);
                    if (!string.IsNullOrEmpty(search.Destination))
                        cmd.Parameters.AddWithValue("@DestinationId", search.Destination);
                    if (search.FromDate != null)
                        cmd.Parameters.AddWithValue("@FromDate", search.FromDate);
                    if (search.Duration != null)
                        cmd.Parameters.AddWithValue("@Duration", search.Duration);
                    if (search.MinPrice != null)
                        cmd.Parameters.AddWithValue("@MinPrice", search.MinPrice);
                    if (search.MaxPrice != null)
                        cmd.Parameters.AddWithValue("@MaxPrice", search.MaxPrice);
                    if (search.IsActive != null)
                    {
                        if (search.IsActive == true)
                        {
                            cmd.Parameters.AddWithValue("@IsActive", true);
                            cmd.Parameters.AddWithValue("@IsActiveCheck", true);
                        } else
                        {
                            cmd.Parameters.AddWithValue("@IsActive", false);
                            cmd.Parameters.AddWithValue("@IsActiveCheck", false);
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@IsActive", true);
                        cmd.Parameters.AddWithValue("@IsActiveCheck", false);
                    }

                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.Read())
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

        public async Task<IEnumerable<Tour>> SearchToursAsync(TourViewModel search)
        {
            List<Tour> result = null;
            Tour tour;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {                    
                    SqlCommand cmd;
                    if (string.IsNullOrEmpty(search.Destination))
                    {
                        cmd = new SqlCommand("spSearchTours", cnn);
                    }
                    else
                    {
                        cmd = new SqlCommand("spSearchToursIncludeDestinationId", cnn);
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Skip", search.Skip);
                    cmd.Parameters.AddWithValue("@Fetch", search.Fetch);
                    if (!string.IsNullOrEmpty(search.Id))
                        cmd.Parameters.AddWithValue("@Id", search.Id);
                    if (!string.IsNullOrEmpty(search.Name))
                        cmd.Parameters.AddWithValue("@Name", search.Name);
                    if (!string.IsNullOrEmpty(search.Destination))
                        cmd.Parameters.AddWithValue("@DestinationId", search.Destination);
                    if (search.FromDate != null)
                        cmd.Parameters.AddWithValue("@FromDate", search.FromDate);
                    if (search.Duration != null)
                        cmd.Parameters.AddWithValue("@Duration", search.Duration);
                    if (search.MinPrice != null)
                        cmd.Parameters.AddWithValue("@MinPrice", search.MinPrice);
                    if (search.MaxPrice != null)
                        cmd.Parameters.AddWithValue("@MaxPrice", search.MaxPrice);
                    if (search.IsActive != null)
                    {
                        if (search.IsActive == true)
                        {
                            cmd.Parameters.AddWithValue("@IsActive", true);
                            cmd.Parameters.AddWithValue("@IsActiveCheck", true);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@IsActive", false);
                            cmd.Parameters.AddWithValue("@IsActiveCheck", false);
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@IsActive", true);
                        cmd.Parameters.AddWithValue("@IsActiveCheck", false);
                    }

                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Tour>();
                            while (sdr.Read())
                            {
                                tour = new Tour();
                                tour.Id = sdr.GetString(sdr.GetOrdinal("Id"));
                                tour.Name = sdr.GetString(sdr.GetOrdinal("Name"));
                                tour.Description = sdr.GetString(sdr.GetOrdinal("Description"));
                                tour.FromDate = sdr.GetDateTime(sdr.GetOrdinal("FromDate"));
                                tour.ToDate = sdr.GetDateTime(sdr.GetOrdinal("ToDate"));
                                tour.Transport = sdr.GetString(sdr.GetOrdinal("Transport"));
                                tour.AdultFare = sdr.GetDecimal(sdr.GetOrdinal("AdultFare"));
                                tour.Image = sdr.GetString(sdr.GetOrdinal("Image"));
                                tour.Duration = sdr.GetInt32(sdr.GetOrdinal("Duration"));
                                result.Add(tour);
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

        public async Task<bool> HasOrderByTourIdAsync(string id)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spHasOrderByTourId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
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

        public async Task<bool> AddTourDestinationAsync(string tourId, string destinationId, int index)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spAddTourDestination", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TourId", tourId);
                    cmd.Parameters.AddWithValue("@DestinationId", destinationId);
                    cmd.Parameters.AddWithValue("@IndexNumber", index);
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

        public async Task<bool> DeleteTourDestinationsByTourIdAsync(string Id)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteTourDestinationsByTourId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
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

        public async Task<IEnumerable<Destination>> GetDestinationsByTourIdAsync(string id)
        {
            List<Destination> result = null;
            Destination destination;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetDestinationsByTourId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Destination>();                            
                            while (sdr.Read())
                            {
                                destination = new Destination();
                                destination.Id = sdr.GetString(sdr.GetOrdinal("DestinationId"));
                                destination.Name = sdr.GetString(sdr.GetOrdinal("Name"));
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

        public async Task<bool> AddTourGuideAsync(string tourId, string userId)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("AddTourGuide", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TourId", tourId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
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

        public async Task<bool> DeleteTourGuidesByTourIdAsync(string id)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteTourGuidesByTourId", cnn);
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

        public async Task<IEnumerable<string>> GetGuidesByTourIdAsync(string id)
        {
            List<string> result = null;
            string guide;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetGuidesByTourId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<string>();
                            while (sdr.Read())
                            {
                                guide = sdr.GetString(sdr.GetOrdinal("UserId"));
                                result.Add(guide);
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

        public async Task<bool> AddTourAsync(Tour tour)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spAddTour", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", tour.Id);
                    cmd.Parameters.AddWithValue("@Name", tour.Name);
                    cmd.Parameters.AddWithValue("@Description", tour.Description);
                    cmd.Parameters.AddWithValue("@FromDate", tour.FromDate);
                    cmd.Parameters.AddWithValue("@ToDate", tour.ToDate);
                    cmd.Parameters.AddWithValue("@AdultFare", tour.AdultFare);
                    cmd.Parameters.AddWithValue("@KidFare", tour.KidFare);
                    cmd.Parameters.AddWithValue("@MaxGuest", tour.MaxGuest);
                    cmd.Parameters.AddWithValue("@Image", tour.Image);
                    cmd.Parameters.AddWithValue("@Transport", tour.Transport);
                    cmd.Parameters.AddWithValue("@IsActive", tour.IsActive);
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

        public async Task<bool> UpdateTourAsync(Tour tour)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateTour", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", tour.Id);
                    cmd.Parameters.AddWithValue("@Name", tour.Name);
                    cmd.Parameters.AddWithValue("@Description", tour.Description);
                    cmd.Parameters.AddWithValue("@FromDate", tour.FromDate);
                    cmd.Parameters.AddWithValue("@ToDate", tour.ToDate);
                    cmd.Parameters.AddWithValue("@AdultFare", tour.AdultFare);
                    cmd.Parameters.AddWithValue("@KidFare", tour.KidFare);
                    cmd.Parameters.AddWithValue("@Transport", tour.Transport);
                    cmd.Parameters.AddWithValue("@MaxGuest", tour.MaxGuest);
                    cmd.Parameters.AddWithValue("@Image", tour.Image);
                    cmd.Parameters.AddWithValue("@IsActive", tour.IsActive);
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
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
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

        public async Task<Dictionary<string, string>> GetDestinationsIdNameAsync()
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
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.HasRows)
                        {
                            result = new Dictionary<string, string>();
                            while (sdr.Read())
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

        public async Task<int> GetTotalDestinationsAsync()
        {
            int result = 0;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetTotalDestinations", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.Read())
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

        public async Task<IEnumerable<Destination>> GetAllDestinationsAsync()
        {
            List<Destination> result = null;
            Destination destination;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetAllDestinations", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Destination>();
                            while (sdr.Read())
                            {
                                destination = new Destination();
                                destination.Id = sdr.GetString(sdr.GetOrdinal("Id"));
                                destination.Name = sdr.GetString(sdr.GetOrdinal("Name"));
                                destination.Description = sdr.GetString(sdr.GetOrdinal("Description"));
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
                    cmd.Parameters.AddWithValue("@Description", destination.Description);
                    cmd.Parameters.AddWithValue("@Image", destination.Image);
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

        #endregion


        #region OrderDAL

        public async Task<bool> AddOrderAsync(Order order)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spAddOrder", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", order.Id);
                    cmd.Parameters.AddWithValue("@UserId", order.UserId);
                    cmd.Parameters.AddWithValue("@CouponCode", order.CouponCode);
                    cmd.Parameters.AddWithValue("@Comment", order.Comment);
                    cmd.Parameters.AddWithValue("@Status", order.Status);
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

        public async Task<bool> AddOrderDetailAsync(string orderId, string tourId, int adultTicket, int kidTicket)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spAddOrderDetail", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@TourId", tourId);
                    cmd.Parameters.AddWithValue("@AdultTicket", adultTicket);
                    cmd.Parameters.AddWithValue("@KidTicket", kidTicket);
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


        #region CouponCodeDAL

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

        public async Task<int> GetOffPercentByCodeAsync(string code)
        {
            int result = 0;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spGetOffPercentByCode", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Code", code);
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        if (sdr.Read())
                        {
                            result = sdr.GetInt32(sdr.GetOrdinal("OffPercent"));
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
