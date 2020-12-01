using FromLocalsToLocals.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace FromLocalsToLocals.Models
{ 
    public class Review : IEquatable<Review>
    {
        public Review()
        {

        }
        public Review(int id, int commentId, string username, string comment, int stars, string vendorTitle)
        {
            VendorID = id;
            CommentID = commentId;
            SenderUsername = username;
            Text = comment;
            Stars = stars;
            Reply = "";
            ReplySender = "";
            ReplyDate = "";

            PublisherSingleton.Instance.Send(new ReviewCreatedEventArgs(this, vendorTitle));
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required] public int VendorID { get; set; }

        [Required] public int CommentID { get; set; }

        [Required] public string SenderUsername { get; set; }

        [Required] public string Text { get; set; }

        [Required] public int Stars { get; set; }

        [Required] public string Date { get; set; }

        public string Reply { get; set; }

        public string ReplySender { get; set; }

        public string ReplyDate { get; set; }

        [ForeignKey("VendorID")]
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Vendor Vendor { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Notification> Notifications { get; set; }

        public bool Equals([AllowNull] Review other)
        {
            if (other == null)
            {
                return false;
            }

            return (VendorID == other.VendorID && SenderUsername == other.SenderUsername && Text == other.Text);
        }
    }
}
