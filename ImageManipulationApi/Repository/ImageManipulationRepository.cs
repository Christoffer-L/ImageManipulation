using ImageManipulationApi.Database;
using ImageManipulationApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR;
using ImageManipulationApi.Hubs;

namespace ImageManipulationApi.Repository
{
    public interface IImageManipulationRepository : IBase<UserManipulatedImage> {}

    public class ImageManipulationRepository : IImageManipulationRepository
    {
        private readonly ApiDbContext _Db;

        public ImageManipulationRepository(ApiDbContext db)
        {
            _Db = db;
        }

        #region Sync methods
        public bool Add(UserManipulatedImage newObject)
        {
            _Db.UserManipulatedImages.Add(newObject);
            return (_Db.SaveChanges() > 0);
        }

        public UserManipulatedImage Get(string id)
        {
            return _Db.UserManipulatedImages.SingleOrDefault(image => image.Id.Equals(id));
        }
        #endregion

        #region Async methods
        public async Task<bool> AddAsync(UserManipulatedImage newObject)
        {
            await _Db.UserManipulatedImages.AddAsync(newObject);
            return (await _Db.SaveChangesAsync() > 0);
        }

        public async Task<UserManipulatedImage> GetAsync(string id)
        {
            return await _Db.UserManipulatedImages.SingleOrDefaultAsync(image => image.Id.Equals(id));
        }
        #endregion
    }
}
