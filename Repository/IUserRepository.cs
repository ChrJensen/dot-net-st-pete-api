using System.Threading.Tasks;
using dot_net_st_pete_api.Models;

namespace dot_net_st_pete_api.Repository
{
    public interface IUserRepository
    {
        User GetUser(string email);
        User AddUser(User User);
    }
}