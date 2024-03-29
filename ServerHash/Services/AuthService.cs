﻿using Microsoft.AspNetCore.Mvc;
using ServerHash.Dto;
using System.Data;
using ServerHash.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace ServerHash.Services
{
    public class AuthService
    {
        private readonly MyDbContext _context;

        public AuthService(MyDbContext context)
        {
            _context = context;
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
            if (httpContext.Items.TryGetValue("UserRights", out var userRightsObject))
            {
                if (userRightsObject is List<string> userRights)
                {
                    return userRights.Contains("WRITE");
                }
            }
            return false;
        }

        public async Task<IActionResult> LoginUser(object userRights)
        {
            // Приводим объект к нужному типу (JsonResult)
            if (userRights is not JsonResult userRightsJson)
            {
                throw new Exception("Error when reading the rights.");
            }

            return userRightsJson;

        }

        public async Task RegisterUser(UserDto user)
        {

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == user.Login);
            if (existingUser != null)
            {
                throw new Exception("A user with this username already exists.");
            }

            // Создаем нового пользователя
            var newUser = new User
            {
                Login = user.Login,
                Password = user.Password
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
