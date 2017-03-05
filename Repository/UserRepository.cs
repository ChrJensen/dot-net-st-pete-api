using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dot_net_st_pete_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace dot_net_st_pete_api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context = null;

        public UserRepository(IOptions<MongoSettings> settings)
        {
            _context = new UserContext(settings);
        }

        public User AddUser(User user)
        {
            _context.Users.InsertOne(user);
            return user;
            // await _context.Users.InsertOneAsync(user);
        }

        public User GetUser(string email)
        {
            var filter = Builders<User>.Filter.Eq("Email", email);
            return _context.Users.Find(filter)
                                 .FirstOrDefault();

            // var res = Query<User>.EQ(u => u.Email, email);
            // return _context.Users.Find(res).FirstOrDefault;
        }

        // public async Task<User> GetUser(string email)
        // {
        //     var filter = Builders<User>.Filter.Eq("Email", email);
        //     return await _context.Users
        //                          .Find(filter)
        //                          .FirstOrDefaultAsync();
        // }
    }
}