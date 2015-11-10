using System;
using System.Threading.Tasks;
using CommonUtils.ExtendedTypes;
using MainLogic;
using Microsoft.AspNet.Identity;
using Project_B.Models;

namespace Project_B.CodeClientSide {
    public class UserStoreDao<TUser> : 
                                        IUserStore<TUser, int>, 
                                        IUserLockoutStore<TUser, int>, 
                                        IUserEmailStore<TUser, int>, 
                                        IUserPasswordStore<TUser, int>,
                                        IUserTwoFactorStore<TUser, int> 
                                            where TUser : class, IApplicationUser, new() {
        public Task CreateAsync(TUser user) {
            return Task.FromResult(MainLogicProvider.Instance.AccountProvider.RegisterWithEmail(user.GuestId, user.UserName, user.Password));
        }

        public Task DeleteAsync(TUser user) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            //throw new NotImplementedException();
        }

        public Task<TUser> FindByIdAsync(int userId) {
            var description = MainLogicProvider.Instance.AccountProvider.GetAccountDescription(userId);
            var tUser = description == null ? null : new TUser {
                UserName = description.Email,
                GuestId = description.GuestId,
                Id = description.AccountId
            };
            return Task.FromResult(tUser);
        }

        public Task<TUser> FindByNameAsync(string userName) {
            var description = MainLogicProvider.Instance.AccountProvider.GetAccountDescription(userName);
            var tUser = description == null ? null : new TUser {
                UserName = description.Email,
                GuestId = description.GuestId,
                Id = description.AccountId
            };
            return Task.FromResult(tUser);
        }

        public Task UpdateAsync(TUser user) {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash) {
            user.Password = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user) {
            return Task.FromResult(user.Password);
        }

        public Task<bool> HasPasswordAsync(TUser user) {
            return Task.FromResult(!user.Password.IsNullOrEmpty());
        }

        public Task SetEmailAsync(TUser user, string email) {
            user.UserName = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(TUser user) {
            return Task.FromResult(user.UserName);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user) {
            return Task.FromResult(true);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed) {
            return Task.FromResult(0);
        }

        public Task<TUser> FindByEmailAsync(string email) {
            var description = MainLogicProvider.Instance.AccountProvider.GetAccountDescription(email);
            var tUser = description == null ? null : new TUser {
                UserName = description.Email,
                GuestId = description.GuestId
            };
            return Task.FromResult(tUser);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user) {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd) {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user) {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(TUser user) {
            throw new NotImplementedException();
        }

        public Task<int> GetAccessFailedCountAsync(TUser user) {
            return Task.FromResult(0);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user) {
            return Task.FromResult(false);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled) {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled) {
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user) {
            return Task.FromResult(false);
        }
    }
}