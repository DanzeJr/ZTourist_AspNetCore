using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ZTourist.Models.ViewModels;

namespace ZTourist.Models
{
    public class TourDAL
    {
        private readonly string connectionStr;
        private readonly ILogger logger;

        public TourDAL(IConfiguration configuration, ILogger logger)
        {
            this.connectionStr = configuration["Data:ZTouristDB:ConnectionString"];
            this.logger = logger;
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

        public async Task<bool> IsAvailableTourAsync(string id)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spIsAvailableTour", cnn);
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
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (await sdr.ReadAsync())
                        {
                            result = new Tour { Id = id };
                            result.Name = sdr.GetString(sdr.GetOrdinal("Name"));
                            result.Description = sdr.GetString(sdr.GetOrdinal("Description"));
                            result.Image = sdr.GetString(sdr.GetOrdinal("Image"));
                            result.FromDate = sdr.GetDateTime(sdr.GetOrdinal("FromDate"));
                            result.ToDate = sdr.GetDateTime(sdr.GetOrdinal("ToDate"));
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
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (await sdr.ReadAsync())
                        {
                            result = new Tour { Id = id };
                            result.Name = sdr.GetString(sdr.GetOrdinal("Name"));
                            result.Description = sdr.GetString(sdr.GetOrdinal("Description"));
                            result.Image = sdr.GetString(sdr.GetOrdinal("Image"));
                            result.FromDate = sdr.GetDateTime(sdr.GetOrdinal("FromDate"));
                            result.ToDate = sdr.GetDateTime(sdr.GetOrdinal("ToDate"));
                            result.AdultFare = sdr.GetDecimal(sdr.GetOrdinal("AdultFare"));
                            result.KidFare = sdr.GetDecimal(sdr.GetOrdinal("KidFare"));
                            result.MaxGuest = sdr.GetInt32(sdr.GetOrdinal("MaxGuest"));
                            result.Transport = sdr.GetString(sdr.GetOrdinal("Transport"));
                            result.Duration = sdr.GetInt32(sdr.GetOrdinal("Duration"));
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

        public async Task<IEnumerable<string>> FindToursByUserIdAsync(string id)
        {
            List<string> result = null;
            string tourId;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spFindToursByUserId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", id);
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

        public async Task<IEnumerable<string>> FindFutureToursByUserIdAsync(string id)
        {
            List<string> result = null;
            string tourId;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spFindFutureToursByUserId", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", id);
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
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (await sdr.ReadAsync())
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
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                            if (await sdr.ReadAsync())
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
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (await sdr.ReadAsync())
                        {
                            if (!await sdr.IsDBNullAsync(sdr.GetOrdinal("TakenSlot")))
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
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Tour>();
                            while (await sdr.ReadAsync())
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
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Tour>();
                            while (await sdr.ReadAsync())
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
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Destination>();
                            while (await sdr.ReadAsync())
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
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<AppUser>();
                            while (await sdr.ReadAsync())
                            {
                                user = new AppUser();
                                user.Id = sdr.GetString(sdr.GetOrdinal("UserId"));
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
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Tour>();
                            while (await sdr.ReadAsync())
                            {
                                tour = new Tour();
                                tour.Id = sdr.GetString(sdr.GetOrdinal("Id"));
                                tour.Name = sdr.GetString(sdr.GetOrdinal("Name"));
                                tour.Description = sdr.GetString(sdr.GetOrdinal("Description"));
                                tour.FromDate = sdr.GetDateTime(sdr.GetOrdinal("FromDate"));
                                tour.ToDate = sdr.GetDateTime(sdr.GetOrdinal("ToDate"));
                                tour.Transport = sdr.GetString(sdr.GetOrdinal("Transport"));
                                tour.AdultFare = sdr.GetDecimal(sdr.GetOrdinal("AdultFare"));
                                tour.KidFare = sdr.GetDecimal(sdr.GetOrdinal("KidFare"));
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

        public async Task<int> GetTotalSearchToursAsync(TourSearchViewModel search)
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
                        cmd.Parameters.AddWithValue("@Id", $"%{search.Id}%");
                    if (!string.IsNullOrEmpty(search.Name))
                        cmd.Parameters.AddWithValue("@Name", $"%{search.Name}%");
                    if (!string.IsNullOrEmpty(search.Destination))
                        cmd.Parameters.AddWithValue("@DestinationId", search.Destination);
                    if (search.FromDate != null)
                        cmd.Parameters.AddWithValue("@FromDate", search.FromDate);
                    if (search.Duration > 0 && search.Duration < 8)
                        cmd.Parameters.AddWithValue("@Duration", search.Duration * 24);
                    if (search.MinPrice != null)
                        cmd.Parameters.AddWithValue("@MinPrice", search.MinPrice);
                    if (search.MaxPrice != null)
                        cmd.Parameters.AddWithValue("@MaxPrice", search.MaxPrice);
                    if (search.IsActive == null)
                    {
                        cmd.Parameters.AddWithValue("@IsActive", true);
                        cmd.Parameters.AddWithValue("@IsActiveCheck", false);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@IsActive", search.IsActive);
                        cmd.Parameters.AddWithValue("@IsActiveCheck", search.IsActive);
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

        public async Task<IEnumerable<Tour>> SearchToursAsync(TourSearchViewModel search)
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
                        cmd.Parameters.AddWithValue("@Id", $"%{search.Id}%");
                    if (!string.IsNullOrEmpty(search.Name))
                        cmd.Parameters.AddWithValue("@Name", $"%{search.Name}%");
                    if (!string.IsNullOrEmpty(search.Destination))
                        cmd.Parameters.AddWithValue("@DestinationId", search.Destination);
                    if (search.FromDate != null)
                        cmd.Parameters.AddWithValue("@FromDate", search.FromDate);
                    if (search.Duration > 0 && search.Duration < 8)
                        cmd.Parameters.AddWithValue("@Duration", search.Duration * 24);
                    if (search.MinPrice != null)
                        cmd.Parameters.AddWithValue("@MinPrice", search.MinPrice);
                    if (search.MaxPrice != null)
                        cmd.Parameters.AddWithValue("@MaxPrice", search.MaxPrice);
                    if (search.IsActive == null)
                    {
                        cmd.Parameters.AddWithValue("@IsActive", true);
                        cmd.Parameters.AddWithValue("@IsActiveCheck", false);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@IsActive", search.IsActive);
                        cmd.Parameters.AddWithValue("@IsActiveCheck", search.IsActive);
                    }

                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (sdr.HasRows)
                        {
                            result = new List<Tour>();
                            while (await sdr.ReadAsync())
                            {
                                tour = new Tour();
                                tour.Id = sdr.GetString(sdr.GetOrdinal("Id"));
                                tour.Name = sdr.GetString(sdr.GetOrdinal("Name"));
                                tour.Description = sdr.GetString(sdr.GetOrdinal("Description"));
                                tour.FromDate = sdr.GetDateTime(sdr.GetOrdinal("FromDate"));
                                tour.ToDate = sdr.GetDateTime(sdr.GetOrdinal("ToDate"));
                                tour.Transport = sdr.GetString(sdr.GetOrdinal("Transport"));
                                tour.AdultFare = sdr.GetDecimal(sdr.GetOrdinal("AdultFare"));
                                tour.KidFare = sdr.GetDecimal(sdr.GetOrdinal("KidFare"));
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

        public async Task<bool> AddTourAsync(Tour tour)
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
                            cmd.CommandText = "spAddTour";
                            try
                            {
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
                                result = await cmd.ExecuteNonQueryAsync() > 0;

                                if (result) // if add tour successfully, then add details
                                {
                                    result = false;
                                    cmd.CommandText = "spAddTourGuide";
                                    foreach (AppUser guide in tour.Guides)
                                    {
                                        result = false;
                                        cmd.Parameters.Clear();
                                        cmd.Parameters.AddWithValue("@TourId", tour.Id);
                                        cmd.Parameters.AddWithValue("@UserId", guide.Id);
                                        cmd.Parameters.AddWithValue("@AssignDate", DateTime.Now);
                                        result = await cmd.ExecuteNonQueryAsync() > 0;
                                        if (!result)
                                        {
                                            transaction.Rollback(); // if add tour guide failed then roll back transaction
                                            break; // stop for to return
                                        }
                                    }
                                    if (result) // if add tour guides successfully
                                    {
                                        result = false;
                                        int i = 0;
                                        cmd.CommandText = "spAddTourDestination";
                                        foreach (Destination destination in tour.Destinations)
                                        {
                                            result = false;
                                            cmd.Parameters.Clear();
                                            cmd.Parameters.AddWithValue("@TourId", tour.Id);
                                            cmd.Parameters.AddWithValue("@DestinationId", destination.Id);
                                            cmd.Parameters.AddWithValue("@IndexNumber", i);
                                            i++;
                                            result = await cmd.ExecuteNonQueryAsync() > 0;
                                            if (!result)
                                            {
                                                transaction.Rollback(); // if add tour destination failed then roll back transaction
                                                break; // stop for to return
                                            }
                                        }
                                    }

                                }
                                transaction.Commit(); // commit transaction whether update tour is successful or not
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

        public async Task<bool> UpdateTourAsync(Tour tour)
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
                            cmd.CommandText = "spUpdateTour";
                            try
                            {
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
                                result = await cmd.ExecuteNonQueryAsync() > 0;

                                if (result) // if update tour successfully, then update details
                                {
                                    result = false;
                                    cmd.CommandText = "spDeleteTourGuidesByTourId";
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.AddWithValue("@Id", tour.Id);
                                    result = await cmd.ExecuteNonQueryAsync() > 0;
                                    if (!result)
                                    {
                                        transaction.Rollback(); // if delete tour destination failed then roll back transaction
                                    }
                                    else
                                    {
                                        result = false;
                                        cmd.CommandText = "spAddTourGuide";
                                        foreach (AppUser guide in tour.Guides)
                                        {
                                            result = false;
                                            cmd.Parameters.Clear();
                                            cmd.Parameters.AddWithValue("@TourId", tour.Id);
                                            cmd.Parameters.AddWithValue("@UserId", guide.Id);
                                            cmd.Parameters.AddWithValue("@AssignDate", DateTime.Now);
                                            result = await cmd.ExecuteNonQueryAsync() > 0;
                                            if (!result)
                                            {
                                                transaction.Rollback(); // if update tour guide failed then roll back transaction
                                                break; // stop for to return
                                            }
                                        }
                                        if (result) // if update tour guides successfully
                                        {
                                            cmd.CommandText = "spDeleteTourDestinationsByTourId";
                                            cmd.Parameters.Clear();
                                            cmd.Parameters.AddWithValue("@Id", tour.Id);
                                            result = await cmd.ExecuteNonQueryAsync() > 0;
                                            if (!result)
                                            {
                                                transaction.Rollback(); // if delete tour destination failed then roll back transaction
                                            }
                                            else
                                            {
                                                result = false;
                                                int i = 0;
                                                cmd.CommandText = "spAddTourDestination";
                                                foreach (Destination destination in tour.Destinations)
                                                {
                                                    result = false;
                                                    cmd.Parameters.Clear();
                                                    cmd.Parameters.AddWithValue("@TourId", tour.Id);
                                                    cmd.Parameters.AddWithValue("@DestinationId", destination.Id);
                                                    cmd.Parameters.AddWithValue("@IndexNumber", i);
                                                    i++;
                                                    result = await cmd.ExecuteNonQueryAsync() > 0;
                                                    if (!result)
                                                    {
                                                        transaction.Rollback(); // if update tour destination failed then roll back transaction
                                                        break; // stop for to return
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                                transaction.Commit(); // commit transaction whether update tour is successful or not
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

        public async Task<bool> UpdateStatusTourByIdAsync(string id, bool isActive)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateStatusTourById", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
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

        public async Task<bool> DeleteTourByIdAsync(string id)
        {
            bool result = false;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionStr))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteTourById", cnn);
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

        #endregion

    }
}
