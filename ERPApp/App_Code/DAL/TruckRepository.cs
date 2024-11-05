using App_Code.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace App_Code.DAL
{
    public class TruckRepository
    {
        private readonly string _connectionString;
        public TruckRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Retrieves all truck records from the database.
        /// This method executes a SQL  to select all entries from the "Trucks" table,
        /// and reads the results into a list of <see cref="Truck"/> objects. 
        /// Each <see cref="Truck"/> is populated with properties including TruckId, TruckCode, TruckName, Status, and Description. 
        /// The Status property is parsed from a string to the <see cref="TruckStatus"/> enumeration.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Truck}"/> containing all truck records from the database.</returns>
        public IEnumerable<Truck> GetAllTrucks()
        {
            var trucks = new List<Truck>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("usp_GetAllTrucks", connection))
                {
                    command.CommandType = CommandType.StoredProcedure; 

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            trucks.Add(new Truck
                            {
                                TruckId = reader.GetInt32(reader.GetOrdinal("TruckId")),
                                TruckCode = reader.GetString(reader.GetOrdinal("TruckCode")),
                                TruckName = reader.GetString(reader.GetOrdinal("TruckName")),
                                Status = (TruckStatus)Enum.Parse(typeof(TruckStatus), reader.GetString(reader.GetOrdinal("Status"))),
                                Description = reader.GetString(reader.GetOrdinal("Description"))
                            });
                        }
                    }
                }
            }
            return trucks;
        }

        /// <summary>
        /// Retrieves a Truck object from the database based on the specified truck ID.
        /// This method executes a SQL to fetch truck details, including the truck code, name, status, and description.
        /// If a truck with the specified ID is found, a Truck object is created and returned; otherwise, null is returned.
        /// </summary>
        /// <param name="truckId">The ID of the truck to retrieve.</param>
        /// <returns>A Truck object populated with the truck's details, or null if no truck is found with the specified ID.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during database access.</exception>
        public Truck GetTruckById(int truckId)
        {
            Truck truck = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetTruckById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TruckId", truckId);

                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                truck = new Truck
                                {
                                    TruckId = (int)reader["TruckId"],
                                    TruckCode = (string)reader["TruckCode"],
                                    TruckName = (string)reader["TruckName"],
                                    Status = (TruckStatus)Enum.Parse(typeof(TruckStatus), (string)reader["Status"]),
                                    Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null
                                };
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("An error occurred while fetching the truck data.", ex);
                    }
                }
            }

            return truck;
        }

        /// <summary>
        /// Inserts a new truck record into the Trucks table in the database.
        /// This method takes a <see cref="Truck"/> object as a parameter
        /// The properties include TruckCode, TruckName, Status, and Description.
        /// If the Description is null, it is stored as DBNull in the database.
        /// </summary>
        /// <param name="truck">The <see cref="Truck"/> object containing the truck details to be inserted.</param>
        public void CreateTruck(Truck truck)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("usp_CreateTruck", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TruckCode", truck.TruckCode);
                    command.Parameters.AddWithValue("@TruckName", truck.TruckName);
                    command.Parameters.AddWithValue("@Status", truck.Status);
                    command.Parameters.AddWithValue("@Description", (object)truck.Description ?? DBNull.Value);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Message.Contains("A truck with this code already exists."))
                        {
                            throw new Exception("A truck with this code already exists.");
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the details of an existing truck in the database.
        /// </summary>
        /// <param name="truck">The <see cref="Truck"/> object containing the updated information for the truck.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="truck"/> parameter is null.</exception>
        /// <exception cref="Exception">Thrown when the update operation fails or if an SQL error occurs during execution.</exception>
        public void UpdateTruck(Truck truck)
        {
            if (truck == null)
            {
                throw new ArgumentNullException(nameof(truck), "Truck object cannot be null.");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("UpdateTruck", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TruckId", truck.TruckId);
                    command.Parameters.AddWithValue("@TruckCode", truck.TruckCode);
                    command.Parameters.AddWithValue("@TruckName", truck.TruckName);
                    command.Parameters.AddWithValue("@Status", truck.Status);
                    command.Parameters.AddWithValue("@Description", (object)truck.Description ?? DBNull.Value);

                    try
                    {
                        connection.Open();

                        var rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("Update failed. No rows were affected.");
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("An error occurred while updating the truck record.", ex);
                    }
                }
            }
        }


        /// <summary>
        /// Checks if the provided truck code is unique in the Trucks table.
        /// </summary>
        /// <param name="truckCode">The truck code to be checked for uniqueness.</param>
        /// <returns>Returns true if the truck code is unique (not found in the database), otherwise false.</returns>
        public bool IsTruckCodeUnique(string truckCode)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string procName = "usp_IsTruckCodeUnique";
                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TruckCode", truckCode);

                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    conn.Close();

                    return count == 0;
                }
            }
        }


        /// <summary>
        /// Deletes a truck record from the database based on the provided truck ID.
        /// </summary>
        /// <param name="truckId">The unique identifier of the truck to be deleted.</param>
        /// <returns>True if the truck record was successfully deleted; otherwise, false.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while attempting to delete the truck record.</exception>
        public bool DeleteTruck(int truckId)
        {
            string storedProcedure = "usp_DeleteTruck"; 

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure; 
                    command.Parameters.AddWithValue("@TruckId", truckId);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        // Return true if one or more rows were affected (deleted)
                        return rowsAffected > 0;
                    }
                    catch (SqlException ex)
                    {
                        // Log or handle the exception as needed
                        throw new Exception("An error occurred while deleting the truck record.", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the status of a truck in the database.
        /// </summary>
        /// <param name="truckId">The unique identifier of the truck whose status is to be updated.</param>
        /// <param name="newStatus">The new status to set for the truck, represented as a TruckStatus enum.</param>
        public void SaveTruckStatusToDatabase(int truckId, TruckStatus newStatus)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("usp_UpdateTruckStatus", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Status", (int)newStatus); 
                    cmd.Parameters.AddWithValue("@TruckId", truckId);

                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}