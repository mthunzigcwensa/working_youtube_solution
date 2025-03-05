using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube.Domain.Entities
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(500)]
        public string userComment { get; set; }
        public DateTime postedBy { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int VideoId { get; set; }

        [ForeignKey("VideoId")]
        public virtual Video VideoData { get; set; }

        [NotMapped]
        public string TimeAgo
        {
            get
            {
                if (postedBy == null)
                    return "Unknown time"; // Handle null cases for postedBy if applicable

                var timeSpan = DateTime.UtcNow - postedBy;

                if (timeSpan.TotalSeconds < 60)
                    return $"{(int)timeSpan.TotalSeconds} second{(timeSpan.TotalSeconds >= 2 ? "s" : "")} ago";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes >= 2 ? "s" : "")} ago";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours >= 2 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 7)
                    return $"{(int)timeSpan.TotalDays} day{(timeSpan.TotalDays >= 2 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 30)
                    return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) >= 2 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 365)
                    return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) >= 2 ? "s" : "")} ago";
                return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) >= 2 ? "s" : "")} ago";
            }
        }





    }
}
