using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using MainLogic.WebFiles;
using Project_B.CodeServerSide.Entity;

namespace Project_B.CodeServerSide.DataProvider {
    public class WebFileProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (WebFileProvider).FullName);

        public WebFileProvider() : base(_logger) {}

        public short GetNextFileID(FileFormat imageFormat) {
            return InvokeSafeSingleCall(() => {
                var f = new SiteFileStore {
                    Accesscount = default(int),
                    Fileformat = imageFormat
                };
                f.Save();
                return f.ID;
            }, default(short));
        }
        public void UpdateFile(short webFileID, FileFormat fileFormat) {
            InvokeSafeSingleCall(() => {
                var f = SiteFileStore.DataSource.GetByKey(webFileID);
                f.DateupDatedutc = DateTime.UtcNow;
                f.Fileformat = fileFormat;
                f.Save();
                return f.ID;
            }, default(short));
        }

        public void AccessToFileCounter(short fileID) {
            InvokeSafe(() => {
                SiteFileStore.DataSource
                    .WhereEquals(SiteFileStore.Fields.ID, fileID)
                    .Update(SiteFileStore.Fields.Accesscount, new DbFnSimpleOp(SiteFileStore.Fields.Accesscount, FnMathOper.Add, 1));
            });
        }

        public short GetTotalFilesCount() {
            return InvokeSafe(() => (short) (SiteFileStore.DataSource.Max(SiteFileStore.Fields.ID) ?? default(decimal)), default(short));
        }

        public List<Tuple<short, FileFormat>> GetFileInfos(short startID, int limit) {
            return InvokeSafe(() => {
                return SiteFileStore.DataSource
                                   .Sort(SiteFileStore.Fields.ID, SortDirection.Desc)
                                   .Where(SiteFileStore.Fields.ID, Oper.LessOrEq, startID)
                                   .AsList(0, limit, SiteFileStore.Fields.Fileformat)
                                   .Select(fs => new Tuple<short, FileFormat>(fs.ID, fs.Fileformat))
                                   .ToList();
            }, new List<Tuple<short, FileFormat>>());
        } 
    }
}