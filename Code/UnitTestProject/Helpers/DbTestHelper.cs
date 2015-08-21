using CommonUtils;
using IDEV.Hydra.DAO;
using MainLogic.Entities;

namespace UnitTestProject.Helpers {
    class DbTestHelper {

        private const string DeleteTpl = "delete from {0};";
        static readonly string[] _tblNames = {
            EntityDescriptor.GetDescriptor(typeof(UtmGuestReferrer)).TableName,
            EntityDescriptor.GetDescriptor(typeof(GuestReferrer)).TableName,
            EntityDescriptor.GetDescriptor(typeof(GuestActionLog)).TableName,
            EntityDescriptor.GetDescriptor(typeof(GuestTechInfo)).TableName,
            EntityDescriptor.GetDescriptor(typeof(GuestExistsBrowser)).TableName,
            EntityDescriptor.GetDescriptor(typeof(UtmSubdomainRule)).TableName,
            EntityDescriptor.GetDescriptor(typeof(Guest)).TableName,
        };

        /// <summary>
        /// Удаление сущностей из базы.
        /// </summary>
        public void Clear() {
            var cleanDbString = string.Empty;
            foreach (var tblName in _tblNames) {
                cleanDbString += string.Format(DeleteTpl, tblName);
            }
            DatabaseActions
                .RunQuery(cleanDbString, null, DatabaseActions.GetAdapter(TargetDB.MASTER), false, true, null, null, null, null, 100000);
        }
    }
}
