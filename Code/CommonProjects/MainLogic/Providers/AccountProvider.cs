using System;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using MainLogic.Entities;
using MainLogic.Transport;

namespace MainLogic.Providers {
    public class AccountProvider : SafeInvokerBase {
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
                        .Where(AccountIdentity.Fields.Email, Oper.Eq, email)
                        .IsExists()) {
                    return false;
                }
                return new AccountIdentity {
                    Datecreated = DateTime.Now,
                    GuestID = guestid,
                    Email = email,
                    Password = password.GetMD5()
                }.Save();
            }, false);
        }

        public Tuple<long, int> LoginWithEmail(string email, string password) {
            return InvokeSafeSingleCall(() => {
                email = email.ToLower();
                var identity = AccountIdentity.DataSource
                                              .WhereEquals(AccountIdentity.Fields.Email, email)
                                              .WhereEquals(AccountIdentity.Fields.Password, password.GetMD5())
                                              .First(
                                                    AccountIdentity.Fields.ID,
                                                    AccountIdentity.Fields.GuestID
                                                );
                if (identity == null) {
                    return null;
                }
                AccountIdentity.DataSource
                               .WhereEquals(AccountIdentity.Fields.ID, identity.ID)
                               .Update(AccountIdentity.Fields.Datelastlogin, DateTime.Now);
                return new Tuple<long, int>(identity.GuestID, identity.ID);
            }, null);
        }

        public AccountDetailsTransport GetAccountDescription(int account) {
            return InvokeSafeSingleCall(() => {
                var identity = AccountIdentity.DataSource
                    .WhereEquals(AccountIdentity.Fields.ID, account)
                    .First(AccountIdentity.Fields.Email);

                return new AccountDetailsTransport {
                    Email = identity.Email
                };
            }, null);
        }
    }
}
