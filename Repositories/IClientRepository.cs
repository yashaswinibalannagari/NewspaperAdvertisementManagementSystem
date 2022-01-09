using System;
using System.Threading.Tasks;
using NewspaperAdvertisementManagementSystem.Models;
namespace NewspaperAdvertisementManagementSystem.Repositories
{
    public interface IClientRepository
    {
        Task<Client> GetClientById(int ClientId);

        Task<int> AddClient(Client client);

        Task<Client> UpdateClient(Client client);
    }
}