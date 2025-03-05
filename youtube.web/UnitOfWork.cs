using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube.Application.Common.Interfaces;
using youtube.Infrastrcture.Data;

namespace youtube.Infrastrcture.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IApplicationUserRepository User { get; private set; }
        public IChannelRepository Channel { get; private set; }

        public IVideoRepository Video { get; private set; }
        public ICommentRepository Comment { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            User = new ApplicationUserRepository(_db);
            Channel = new ChannelRepository(_db);
            Video = new VideoRepository(_db);
            Comment = new CommentRepository(_db);

        }

        public void Save()
        {
            _db.SaveChanges();
        }


    }
}
