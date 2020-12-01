using FromLocalsToLocals.Database;
using FromLocalsToLocals.Utilities;
using Geocoding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FromLocalsToLocals.Models
{
    public class Vendor : IEquatable<Vendor>, IComparable<Vendor>
    {
        [NotMapped] public Location Location { get; set; }

        public int[] ReviewsCount = {0, 0, 0, 0, 0};

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required] public string UserID { get; set; }
          
        [Required]
        [Display(Name ="Title")]
        public string Title { get; set; }

        [Display(Name = "About")]
        public string About { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Date Created")]
        public string DateCreated { get; set; }

        [Required] public double Latitude { get; set; }

        [Required] public double Longitude { get; set; }


        [Column("VendorType")]
        [DisplayName("Vendor Type")]
        public string VendorTypeDb
        {
            get => VendorType.ToString();
            private set { VendorType = value.ParseEnum<VendorType>(); }
        }

        [NotMapped]
        public VendorType VendorType { get; set; }
        [NotMapped]
        public double Average { get; set; }

        public byte[] Image { get; set; }

        [JsonIgnore] 
        [IgnoreDataMember]
        [ForeignKey("UserID")]
        public virtual AppUser User { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Review> Reviews { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Post> Posts { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Follower> Followers { get; set; }


        #region IComparable


        public int CompareTo(Vendor other)
        {
            if (other == null) return 1;

            return Average.CompareTo(other.Average);
        }

        public static bool operator >(Vendor operand1, Vendor operand2)
        {
            return operand1.CompareTo(operand2) == 1;
        }

        public static bool operator <(Vendor operand1, Vendor operand2)
        {
            return operand1.CompareTo(operand2) == -1;
        }

        public static bool operator >=(Vendor operand1, Vendor operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        public static bool operator <=(Vendor operand1, Vendor operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }

        #endregion

        #region IEquatable

        public bool Equals([AllowNull] Vendor other)
        {
            if (other == null)
            {
                return false;
            }

            return (Title == other.Title);
        }

        #endregion

        public void UpdateReviewsCount(AppDbContext context)
        {
            var reviews = context.Reviews.Where(x => x.VendorID == ID).ToList();

            for (var i = 0; i < 5; i++)
            {
                ReviewsCount[i] = 0;
            }

            reviews.ForEach(x => ReviewsCount[x.Stars - 1]++);

            Average = CountAverage();
        }

        public double CountAverage()
        {
            var sum = 0;
            for (var i = 0; i < 5; i++)
            {
                sum += ReviewsCount[i] * (i+1);
            }

            if (ReviewsCount.Sum() == 0)
            {
                return 0;
            }

            return sum / (double)ReviewsCount.Sum();
        }
    }
}
