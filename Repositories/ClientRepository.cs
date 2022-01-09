using System.Threading.Tasks;
using NewspaperAdvertisementManagementSystem.Contexts;
using NewspaperAdvertisementManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace NewspaperAdvertisementManagementSystem.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ClientRepository(ApplicationDbContext context, IWebHostEnvironment _hostEnvironment)
        {
            this._hostEnvironment = _hostEnvironment;
            this._context = context;
        }

        public async Task<int> AddClient(Client client)
        {
            client.ProfileImageName = await SaveImage(client.ProfileImageFile);
            var result = await _context.Clients.AddAsync(client);

            await _context.SaveChangesAsync();
            return result.Entity.ClientId;
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);

            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images//Clients", imageName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return imageName;

        }

        public async Task<Client> GetClientById(int ClientId)
        {

            Client client = await _context.Clients.FirstOrDefaultAsync(client => client.ClientId == ClientId);

            return client;

        }

        public async Task<Client> UpdateClient(Client client)
        {
            var result = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == client.ClientId);
            if (result != null)
            {
                result.FirstName = client.FirstName;
                result.LastName = client.LastName;
                result.Email = client.Email;
                result.Address = client.Address;
                deleteImage(result.ProfileImageName);
                result.ProfileImageName = await SaveImage(client.ProfileImageFile);

                await _context.SaveChangesAsync();
                return result;
            }
            return null;

        }

        private void deleteImage(string imageName)
        {
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images//Clients", imageName);

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }

        }
    }
}