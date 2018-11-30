using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data 
{
    public class DatingRepository : IDatingRepository 
    {
        private readonly DataContext _context;
        public DatingRepository (DataContext context) 
        {
            _context = context;
        }
        public void Add<T> (T entity) where T : class 
        {
            _context.Add(entity);
        }

        public void Delete<T> (T entity) where T : class 
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<User> GetUser (int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        // Alternative 1: Simple Version of get users (without paging)
        // public async Task<IEnumerable<User>> GetUsers () 
        // {
        //     var users = await _context.Users.Include(p => p.Photos).ToListAsync();
        //     return users;
        // }        
        // Alternative 2: Return paged result by passing the UserParams 
         public async Task<PagedList<User>> GetUsers (UserParams userParams) 
        {
            // Prepare the list of users, but not execute to List method
            var users = _context.Users.Include(p => p.Photos)
                .OrderByDescending(u => u.LastActive)
                .AsQueryable();

            // Applying filter params
            users = users.Where(u => u.Id != userParams.UserId);

            users = users.Where(u => u.Gender == userParams.Gender);

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created": 
                        users = users.OrderByDescending(u => u.Created);
                        break;
                        default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            // Get users for the particular page (e.g. page 2 - users 11 to 20 for a page size of 10).
            // Note: ToListAsync is executed within the CreateAsyc generic method
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<bool> SaveAll () 
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}