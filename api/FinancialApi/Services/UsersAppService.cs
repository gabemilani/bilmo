using AutoMapper;
using Financial.Api.Exceptions;
using Financial.Api.Models;
using Financial.Domain.Entities;
using Financial.Domain.Services;
using System;
using System.Threading.Tasks;

namespace Financial.Api.Services
{
    public class UsersAppService
    {
        private readonly UsersService _userService;
        private readonly WalletsService _walletsService;

        public UsersAppService(UsersService userService, WalletsService walletsService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _walletsService = walletsService ?? throw new ArgumentNullException(nameof(walletsService));
        }

        public async Task<User> AddAsync(UserInputModel inputModel)
        {        
            var user = Mapper.Map<User>(inputModel);
            await _userService.AddAsync(user);
            return user;           
        }

        public async Task<Wallet> AddUserWalletAsync(int userId, WalletInputModel inputModel)
        {
            var user = await _userService.GetAsync(userId);
            var wallet = Mapper.Map<Wallet>(inputModel);
            wallet.User = user ?? throw new NotFoundException("User not found");

            await _walletsService.AddAsync(wallet);

            return wallet;
        }

        public Task DeleteAsync(int id) => _userService.DeleteAsync(id);

        public Task<User> GetAsync(int id) => _userService.GetAsync(id);

        public User[] GetAll() => _userService.GetAll();

        public async Task<Wallet[]> GetWalletsAsync(int id)
        {
            var user = await _userService.GetAsync(id);
            if (user == null)
                throw new NotFoundException("User not found");

            return _walletsService.GetUserWalletsAsync(id);
        }

        public Task<User> ReplaceAsync(int id, UserInputModel inputModel)
        {
            var user = Mapper.Map<User>(inputModel);
            return _userService.ReplaceAsync(id, user);
        }
    }
}