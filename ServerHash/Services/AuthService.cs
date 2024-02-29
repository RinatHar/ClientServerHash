using Microsoft.AspNetCore.Mvc;
using ServerHash.Dto;
using System.Data;
using ServerHash.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ServerHash.Services
{
    public class AuthService
    {
        private readonly MyDbContext _context;
        private readonly AesEncryptionService _aesService;

        public AuthService(MyDbContext context, AesEncryptionService aesService)
        {
            _context = context;
            _aesService = aesService;
        }

        public async Task<List<string>> GetUserRights(string login, string password)
        {

            var user = await FindUserByCredentials(login, password);
            if (user != null)
            {
                return await GetUserRightsById(user.Id);
            }
            else
            {
                return [];
            }
        }

        public async Task<User> FindUserByCredentials(string login, string password)
        {
            return await _context.Users
                .Where(user => user.Login == login && user.Password == password)
                .FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetUserRightsById(int id)
        {
            return await _context.UsersRights
                .Where(ur => ur.UserId == id)
                .Select(ur => ur.Right.Name)
                .ToListAsync();
        }

        public bool UserHasWriteAccess(HttpContext httpContext)
        {
            // Проверяем наличие прав в Items
            if (httpContext.Items.TryGetValue("UserRights", out var userRights) && userRights is JsonResult userRightsJson)
            {
                if (userRightsJson.Value is string[] rightsArray)
                {
                    return rightsArray.Contains("WRITE");
                }
            }
            return false;
        }

        public async Task<IActionResult> LoginUser(object userRights)
        {
            // Приводим объект к нужному типу (JsonResult)
            if (userRights is not JsonResult userRightsJson)
            {
                throw new Exception("Ошибка при чтении прав.");
            }

            // Преобразуем данные в строку
            var userRightsString = JsonConvert.SerializeObject(userRightsJson.Value);

            // Шифруем права пользователя
            var encryptedUserRights = _aesService.Encrypt(userRightsString);

            // Возвращаем зашифрованные данные как ContentResult
            return new ContentResult
            {
                Content = encryptedUserRights,
                ContentType = "text/plain",
                StatusCode = 200
            };
        }

        public async Task RegisterUser(UserDto user)
        {
            // Шифруем данные пользователя
            var decryptedLogin = _aesService.Decrypt(user.Login);
            var decryptedPassword = _aesService.Decrypt(user.Password);

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == decryptedLogin);
            if (existingUser != null)
            {
                throw new Exception("Пользователь с таким логином уже существует.");
            }

            // Создаем нового пользователя
            var newUser = new User
            {
                Login = decryptedLogin,
                Password = decryptedPassword
            };

            // Добавляем пользователя в базу данных
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Добавляем право на чтение для нового пользователя
            await AddUserReadRight(newUser.Id);
        }

        public async Task AddUserReadRight(int userId)
        {
            // Создаем новую запись в таблице users_rights
            var newUserRight = new UserRight
            {
                UserId = userId,
                RightId = 1 // Предполагается, что ID 1 соответствует праву на чтение
            };

            // Добавляем запись в базу данных
            _context.UsersRights.Add(newUserRight);
            await _context.SaveChangesAsync();
        }
    }
}
