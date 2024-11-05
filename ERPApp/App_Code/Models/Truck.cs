namespace App_Code.Models
{
    public class Truck
    {
        public int TruckId { get; set; }
        public string TruckCode { get; set; }
        public string TruckName { get; set; }
        public TruckStatus Status { get; set; }  // Using the enum
        public string Description { get; set; }
    }
}