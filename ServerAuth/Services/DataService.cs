using Microsoft.EntityFrameworkCore;
using ServerHash.Models;

namespace ServerHash.Services
{
    public class DataService
    {
        private readonly MyDbContext _context;

        public DataService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Data>> GetAllData()
        {
            return await _context.Data.ToListAsync();
        }

        public async Task AddData(Data data)
        {
            _context.Data.Add(data);
            await _context.SaveChangesAsync();
        }
    }
}
