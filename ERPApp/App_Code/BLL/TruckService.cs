using App_Code.DAL;
using App_Code.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace App_Code.BLL
{
    public class TruckService
    {
        private readonly TruckRepository _truckRepository;

        public TruckService()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["erpDB"].ConnectionString;
            _truckRepository = new TruckRepository(connectionString);
        }

        public IEnumerable<Truck> GetAllTrucks() 
        {
            return _truckRepository.GetAllTrucks();
        }

        public Truck GetTruckById(int truckId)
        {
            return _truckRepository.GetTruckById(truckId);
        }
        public bool IsTruckCodeUnique(string truckCode)
        {
            return _truckRepository.IsTruckCodeUnique(truckCode);
        }

        public void CreateTruck(Truck truck)
        {
            _truckRepository.CreateTruck(truck);
        }


        public bool ChangeTruckStatus(int truckId, TruckStatus newStatus, bool validateOnly = false)
        {
            var trucks = GetAllTrucks();
            var truck = trucks.FirstOrDefault(t => t.TruckId == truckId);
            if (truck == null) return false;

            if (newStatus == TruckStatus.OutOfService)
            {
                if (!validateOnly)
                {
                    truck.Status = TruckStatus.OutOfService;
                    _truckRepository.SaveTruckStatusToDatabase(truckId, newStatus);
                }
                return true;
            }

            bool isValidStatusChange = false;

            switch (truck.Status)
            {
                case TruckStatus.OutOfService:
                    isValidStatusChange = (newStatus == TruckStatus.Loading);
                    break;
                case TruckStatus.Loading:
                    isValidStatusChange = (newStatus == TruckStatus.ToJob);
                    break;
                case TruckStatus.ToJob:
                    isValidStatusChange = (newStatus == TruckStatus.AtJob);
                    break;
                case TruckStatus.AtJob:
                    isValidStatusChange = (newStatus == TruckStatus.Returning);
                    break;
                case TruckStatus.Returning:
                    isValidStatusChange = (newStatus == TruckStatus.Loading);
                    break;
                default:
                    return false;
            }

            if (isValidStatusChange)
            {
                if (!validateOnly)
                {
                    truck.Status = newStatus;
                    _truckRepository.SaveTruckStatusToDatabase(truckId, newStatus);
                }
                return true;
            }

            return false;
        }


        public void UpdateTruck(Truck truck)
        {
            _truckRepository.UpdateTruck(truck);
        }

        public bool DeleteTruck(int truckId)
        {
            try
            {
                _truckRepository.DeleteTruck(truckId);
                return true;
            }
            catch
            {
                return false; 
            }
        }

    }
}