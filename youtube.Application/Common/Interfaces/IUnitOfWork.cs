using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository User { get; }      
        IChannelRepository Channel { get; }
        IVideoRepository Video { get; }
        void Save();
    }
}
