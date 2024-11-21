using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
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


        [NotMapped]
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.UtcNow - AddByDate;
                if (timeSpan.TotalMinutes < 1)
                    return "Just now";
                if (timeSpan.TotalMinutes < 60)
                    return $"{timeSpan.Minutes} minutes ago";
                if (timeSpan.TotalHours < 24)
                    return $"{timeSpan.Hours} hours ago";
                if (timeSpan.TotalDays < 7)
                    return $"{timeSpan.Days} days ago";
                if (timeSpan.TotalDays < 30)
                    return $"{timeSpan.Days / 7} weeks ago";
                if (timeSpan.TotalDays < 365)
                    return $"{timeSpan.Days / 30} months ago";
                return $"{timeSpan.Days / 365} years ago";
            }
        }
    }

}
