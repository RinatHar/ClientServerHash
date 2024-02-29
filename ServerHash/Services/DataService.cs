using Microsoft.EntityFrameworkCore;
using ServerHash.Dto;
using ServerHash.Models;

namespace ServerHash.Services
{
    public class DataService
    {
        private readonly MyDbContext _context;
        private readonly AesEncryptionService _aesService;

        public DataService(MyDbContext context, AesEncryptionService aesService)
        {
            _context = context;
            _aesService = aesService;
        }

        public async Task<List<DataDto>> GetAllData()
        {
            var data = await _context.Data.ToListAsync();
            var dataDtos = new List<DataDto>();

            foreach (var item in data)
            {
                var dataDto = new DataDto
                {
                    Value = _aesService.Encrypt(item.Value)
                };
                dataDtos.Add(dataDto);
            }

            return dataDtos;
        }

        public async Task AddData(DataDto dataDto)
        {
            var data = new Data
            {
                Value = _aesService.Decrypt(dataDto.Value)
            };

            _context.Data.Add(data);
            await _context.SaveChangesAsync();
        }
    }
}