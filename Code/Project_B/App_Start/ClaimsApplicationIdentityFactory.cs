using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Project_B.Models;

namespace Project_B {
    public class ClaimsApplicationIdentityFactory<TUser> : ClaimsIdentityFactory<TUser, int> where TUser : class, IApplicationUser {

        /// <summary>
        /// Claim type used for the user id
        /// 
        /// </summary>
        public string GuestIdClaimType { get; set; }

        public ClaimsApplicationIdentityFactory() {
            GuestIdClaimType = "GuestIDClaimType";
        } 

        public override Task<ClaimsIdentity> CreateAsync(UserManager<TUser, int> manager, TUser user, string authenticationType) {
            var res = base.CreateAsync(manager, user, authenticationType);
            res.Result.AddClaim(new Claim(GuestIdClaimType, user.GuestId.ToString()));
            return res;
        }
    }
}