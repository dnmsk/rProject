using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Project_B.Code {
    public class UserStoreDao<TUser> : IUserStore<TUser> where TUser : class, IUser<string> {
        public Task CreateAsync(TUser user) {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TUser user) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            //throw new NotImplementedException();
        }

        public Task<TUser> FindByIdAsync(string userId) {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByNameAsync(string userName) {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TUser user) {
            throw new NotImplementedException();
        }
    }
}