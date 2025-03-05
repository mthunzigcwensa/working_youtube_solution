using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube.Domain.Entities
{
    public class Video
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Url]
        [MaxLength(255)]
        public string VideoUrl { get; set; }

        
        [Url]
        [MaxLength(255)]
        public string ImageUrl { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }


        public int ChannelDataId { get; set; }


        public ChannelData ChannelData { get; set; }
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime AddByDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(255)]
        public int viewCount { get; set; }


        [NotMapped]
        public string TimeAgo
        {
            get
            {
                if (AddByDate == null)
                    return "Unknown time"; // Handle null cases for postedBy if applicable

                var timeSpan = DateTime.UtcNow - AddByDate;

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
