using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube.Domain.Entities;

namespace youtube.Domain.ViewModels
{
    public class ChannelVM
    {
        public ChannelData ChannelData { get; set; }
        public IEnumerable<Video> UserVideos { get; set; }
    }
}
