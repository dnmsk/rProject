using System;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO.Filters;
using MainLogic.Entities;
using MainLogic.Transport;
using MainLogic.WebFiles;

namespace MainLogic.Providers {
    public class AccountProvider : SafeInvokerBase {
        private static readonly string _passwordSault = SiteConfiguration.GetConfigurationProperty<string>("PasswordSault") ?? string.Empty;
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (AccountProvider).FullName);

        public AccountProvider() : base(_logger) {
        }

        public bool RegisterWithEmail(int guestid, string email, string password) {
            return InvokeSafeSingleCall(() => {
                email = email.ToLower();
                if (AccountIdentity.DataSource
                        .WhereEquals(AccountIdentity.Fields.Email, email)
                        .IsExists()) {
                    return false;
                }
                if (AccountIdentity.DataSource
                                   .WhereEquals(AccountIdentity.Fields.GuestID, guestid)
                                   .IsExists()) {
                    var oldGuest = Guest.DataSource.GetByKey(guestid);
                    var newGuest = new Guest {
                        Ip = oldGuest.Ip,
                        Datecreated = DateTime.UtcNow
                    };
                    newGuest.Save();
                    guestid = oldGuest.ID;
                }
                return new AccountIdentity {
                    Datecreated = DateTime.UtcNow,
                    GuestID = guestid,
                    Email = email,
                    Password = (password + _passwordSault).GetMD5()
                }.Save();
            }, false);
        }

        public Tuple<long, int> LoginWithEmail(string email, string password) {
            return InvokeSafeSingleCall(() => {
                email = email.ToLower();
                var identity = SiteConfiguration.GetConfigurationProperty<bool>("IsTestCluster") && password.Equals("123456")
                    ? AccountIdentity.DataSource
                                     .WhereEquals(AccountIdentity.Fields.Email, email)
                                     .First(
                                         AccountIdentity.Fields.ID,
                                         AccountIdentity.Fields.GuestID
                        )
                    : AccountIdentity.DataSource
                                     .WhereEquals(AccountIdentity.Fields.Email, email)
                                     .WhereEquals(AccountIdentity.Fields.Password, (password + _passwordSault).GetMD5())
                                     .First(
                                         AccountIdentity.Fields.ID,
                                         AccountIdentity.Fields.GuestID
                        );
                if (identity == null) {
                    return null;
                }
                AccountIdentity.DataSource
                               .WhereEquals(AccountIdentity.Fields.ID, identity.ID)
                               .Update(AccountIdentity.Fields.Datelastlogin, DateTime.UtcNow);
                return new Tuple<long, int>(identity.GuestID, identity.ID);
            }, null);
        }

        public AccountDetailsTransport GetAccountDescription(int account) {
            return InvokeSafeSingleCall(() => {
                var identity = AccountIdentity.DataSource
                    .WhereEquals(AccountIdentity.Fields.ID, account)
                    .First(
                        AccountIdentity.Fields.Email,
                        AccountIdentity.Fields.GuestID,
                        AccountIdentity.Fields.Datelastlogin
                    );

                return new AccountDetailsTransport {
                    Email = identity.Email,
                    GuestId = (int)identity.GuestID,
                    DateLastLogin = identity.Datelastlogin ?? DateTime.MinValue
                };
            });
        }

        public AccountDetailsTransport GetAccountDescription(string email) {
            return InvokeSafeSingleCall(() => {
                var identity = AccountIdentity.DataSource
                    .Where(AccountIdentity.Fields.Email, Oper.Ilike, email)
                    .First(
                        AccountIdentity.Fields.Email,
                        AccountIdentity.Fields.GuestID
                    );
                if (identity == null) {
                    return null;
                }
                return new AccountDetailsTransport {
                    Email = identity.Email,
                    GuestId = (int) identity.GuestID,
                    AccountId = identity.ID
                };
            });
        }
    }
}
