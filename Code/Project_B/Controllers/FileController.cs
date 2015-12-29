using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic.WebFiles;
using MainLogic.WebFiles.Policy;
using MainLogic.WebFiles.UserPolicy.Enum;
using Project_B.CodeClientSide;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;

namespace Project_B.Controllers {
    public class FileController : ApplicationControllerBase {
        private const string _fileStorePathConfig = "FileStorePath";
        private const int _fileHashLength = 4;
        private const int _subFolderCnt = 100;
        private const int _maxFileLength = 1 * 1024 * 1024;
        private static readonly string _imageStorePath;
        private readonly static TimeSpan _cacheDays = TimeSpan.FromDays(365);
        protected override bool EnableStoreRequestData => false;

        static FileController() {
            _imageStorePath = SiteConfiguration.GetConfigurationProperty(_fileStorePathConfig) ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileStore");
        }

        [HttpGet]
        [ActionProfile(ProjectBActions.PageFileIndex)]
        public ActionResult Index(short id, string type, string hash) {
            var path = Path.Combine(CombinePathFromID(id), id  + "." + type);
            if (!id.ToString().GetMD5(_fileHashLength).Equals(hash, StringComparison.InvariantCultureIgnoreCase)
                || !System.IO.File.Exists(path)) {
                return new EmptyResult();
            }
            MarkResponseAsCached(Response);
            if (!GetBaseModel().GetUserPolicyState<bool>(UserPolicyGlobal.IsStatisticsDisabled) && ProductionPolicy.IsProduction()) {
                SlothMovePlodding.Instance.AddAction(() => { ProjectProvider.Instance.WebFileProvider.AccessToFileCounter(id); });
            }
            return File(path, GetMimeTypeByFileName(path));
        }

        public static string GetFileWebPath(short imageID, FileFormat fileFormat) {
            return string.Format("/file/{0}/{1}/{2}", imageID, GetTypeImageExtension(fileFormat), imageID.ToString().GetMD5(_fileHashLength));
        }

        public static void MarkResponseAsCached(HttpResponseBase responseBase) {
            /* Thu, 20 Mar 2014 09:21:47 GMT */
            responseBase.AddHeader("Expires", DateTime.UtcNow.Add(_cacheDays).ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", CultureInfo.InvariantCulture));
            responseBase.Cache.SetMaxAge(_cacheDays);
        }
        
        public static List<short> UploadFileFromRequest(HttpRequestBase baseRequest, int fileCountLimit = 1) {
            var files = GetFilesFromRequest(baseRequest);
            return UploadBinaryFiles(files.Values, fileCountLimit);
        }

        public static short UpdateFileFromRequest(short fileID, HttpRequestBase baseRequest) {
            var file = GetFilesFromRequest(baseRequest).Values.First();
            return UploadImage(fileID, file.Item1, file.Item2);
        }

        private static Dictionary<string, Tuple<FileFormat, byte[]>> GetFilesFromRequest(HttpRequestBase baseRequest) {
            var files = new Dictionary<string, Tuple<FileFormat, byte[]>>();
            for (var i = 0; i < baseRequest.Files.Count; i++) {
                var file = baseRequest.Files[i];
                if (file != null && file.ContentLength != 0) {
                    var mimeTypeSplitted = file.ContentType.ToLower().Split('/');
                    if (file.ContentLength > _maxFileLength) {
                        continue;
                    }
                    var fileContent = new byte[file.ContentLength];
                    file.InputStream.Read(fileContent, 0, file.ContentLength);
                    FileFormat imageFormat;
                    switch (mimeTypeSplitted[1]) {
                        case "png":
                            imageFormat = FileFormat.Png;
                            break;
                        case "jpeg":
                        case "jpg":
                            imageFormat = FileFormat.Jpeg;
                            break;
                        default:
                            continue;
                    }
                    files[file.FileName] = new Tuple<FileFormat, byte[]>(imageFormat, fileContent);
                }
            }
            return files;
        }

        private static List<short> UploadBinaryFiles(IEnumerable<Tuple<FileFormat, byte[]>> files, int fileCountLimit) {
            var uploadImages = new List<short>();
            foreach (var file in files.TakeWhile(file => uploadImages.Count < fileCountLimit)) {
                uploadImages.Add(UploadImage(default(int), file.Item1, file.Item2));
            }
            return uploadImages;
        }
        private static short UploadImage(short fileID, FileFormat fileFormat, byte[] photoToUpload) {
            fileID.IfNotSet(() => {
                fileID = ProjectProvider.Instance.WebFileProvider.GetNextFileID(fileFormat);
            });
            var directory = CombinePathFromID(fileID);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            var path = Path.Combine(directory, fileID + "." + GetTypeImageExtension(fileFormat));
            if (System.IO.File.Exists(path)) {
                System.IO.File.Delete(path);
            }
            using (var f = System.IO.File.Create(path)) {
                f.Write(photoToUpload, 0, photoToUpload.Length);
            }
            return fileID;
        }
        
        private static string CombinePathFromID(short id) {
            return Path.Combine(_imageStorePath, (id % _subFolderCnt).ToString());
        }

        private static string GetMimeTypeByFileName(string fileName) {
            switch ((Path.GetExtension(fileName) ?? string.Empty).ToLower()) {
                case ".png":
                    return "image/png";
                case ".jpeg":
                case ".jpg":
                    return "image/jpeg";
                default:
                    return null;
            }
        }

        private static string GetTypeImageExtension(FileFormat fileFormat) {
            string type;
            switch (fileFormat) {
                case FileFormat.Png:
                    type = "png";
                    break;
                case FileFormat.Jpeg:
                    type = "jpg";
                    break;
                default:
                    return null;
            }
            return type;
        }
    }
}