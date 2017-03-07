using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dot_net_st_pete_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace dot_net_st_pete_api.Repository
{
    // todo: mix of async and sync code in repositories - update to async
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
        }

        public User GetUser(string email)
        {
            var filter = Builders<User>.Filter.Eq("Email", email);
            return _context.Users.Find(filter)
                                 .FirstOrDefault();
        }
    }
}