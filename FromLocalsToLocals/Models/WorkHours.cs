using System;
using FromLocalsToLocals.Utilities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace FromLocalsToLocals.Models
{
    public class WorkHours : IEquatable<WorkHours>
    {
        public WorkHours()
        {

        }

        public WorkHours(int vendorID, int day, TimeSpan openTime, TimeSpan closeTime)
        {
            VendorID = vendorID;
            Day = day;
            OpenTime = openTime;
            CloseTime = closeTime;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [NotMapped]
        public bool IsWorking { get; set; }

        [Required]
        public int VendorID { get; set; }

        [Required]
        public int Day { get; set; }

        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }

        [ForeignKey("VendorID")]
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Vendor Vendor { get; set; }

        public bool Equals([AllowNull] WorkHours other)
        {
            if (other == null)
            {
                return false;
            }

            return (VendorID == other.VendorID && Day == other.Day && OpenTime == other.OpenTime && CloseTime == other.CloseTime);
        }
    }
}

