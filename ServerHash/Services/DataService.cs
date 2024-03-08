using Microsoft.EntityFrameworkCore;
using ServerHash.Dto;
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

        public async Task<List<DataDto>> GetAllData()
        {
            var data = await _context.Data.ToListAsync();
            var dataDtos = new List<DataDto>();

            foreach (var item in data)
            {
                var dataDto = new DataDto
                {
                    Value = item.Value
                };
                dataDtos.Add(dataDto);
            }

            return dataDtos;
        }

        public async Task AddData(DataDto dataDto)
        {
            var data = new Data
            {
                Value = dataDto.Value
            };

            _context.Data.Add(data);
            await _context.SaveChangesAsync();
        }
    }
}