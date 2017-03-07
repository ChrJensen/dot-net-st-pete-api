using System.Threading.Tasks;
using dot_net_st_pete_api.Models;

namespace dot_net_st_pete_api.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUser(string email);
        Task<User> AddUser(User User);
    }
}