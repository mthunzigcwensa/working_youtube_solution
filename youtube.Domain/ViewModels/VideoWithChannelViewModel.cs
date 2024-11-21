using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube.Domain.ViewModels
{
    public class VideoWithChannelViewModel
    {
        public int VideoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; }
        public string ChannelProfilePictureUrl { get; set; }
        public string ChannelHandle { get; set; }
    }

}
