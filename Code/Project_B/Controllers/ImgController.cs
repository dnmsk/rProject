using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using MainLogic.WebFiles;
using Project_B.CodeServerSide.Enums;

namespace Project_B.Controllers {
    public class ImgController : ApplicationControllerBase {
        private const string _imageStorePathConfig = "ImageStorePath";
        private const int _imageHashLength = 4;
        private const int _subFolderCnt = 100;
        private const int _maxFileLength = 1 * 1024 * 1024;
        private static readonly string _imageStorePath;
        private readonly static TimeSpan _cacheDays = TimeSpan.FromDays(365);

        static ImgController() {
            _imageStorePath = SiteConfiguration.GetConfigurationProperty(_imageStorePathConfig) ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImageStore");
        }

        [HttpGet]
        public ActionResult Index(short id, string type, string hash) {
            var path = CombinePathFromID(id) + "." + type;
            if (!id.ToString().GetMD5(_imageHashLength).Equals(hash, StringComparison.InvariantCultureIgnoreCase)
                || !System.IO.File.Exists(path)) {
                return new EmptyResult();
            }
            MarkResponseAsCached(Response);
            return File(path, GetMimeTypeByFileName(path));
        }

        public static string GetImgWebPath(short imageID, PhotoFormat photoFormat) {
            return string.Format("/img/{0}/{1}/{2}", imageID, GetTypeImageExtension(photoFormat), imageID.ToString().GetMD5(_imageHashLength));
        }

        public static void MarkResponseAsCached(HttpResponseBase responseBase) {
            /* Thu, 20 Mar 2014 09:21:47 GMT */
            responseBase.AddHeader("Expires", DateTime.UtcNow.Add(_cacheDays).ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", CultureInfo.InvariantCulture));
            responseBase.Cache.SetMaxAge(_cacheDays);
        }
        
        public static List<short> UploadImageFromRequest(HttpRequestBase baseRequest, int fileCountLimit = 1) {
            var files = new Dictionary<string, Tuple<PhotoFormat, byte[]>>();
            for (var i = 0; i < baseRequest.Files.Count; i++) {
                var file = baseRequest.Files[i];
                if (file != null && file.ContentLength != 0) {
                    var mimeTypeSplitted = file.ContentType.ToLower().Split('/');
                    if (mimeTypeSplitted[0] != "image" || file.ContentLength > _maxFileLength) {
                        continue;
                    }
                    var fileContent = new byte[file.ContentLength];
                    file.InputStream.Read(fileContent, 0, file.ContentLength);
                    PhotoFormat imageFormat;
                    switch (mimeTypeSplitted[1]) {
                        case "png":
                            imageFormat = PhotoFormat.Png;
                            break;
                        case "jpeg":
                        case "jpg":
                        default:
                            imageFormat = PhotoFormat.Jpeg;
                            break;
                    }
                    files[file.FileName] = new Tuple<PhotoFormat, byte[]>(imageFormat, fileContent);
                }
            }
            return UploadBinaryImages(files.Values, fileCountLimit);
        }

        private static List<short> UploadBinaryImages(IEnumerable<Tuple<PhotoFormat, byte[]>> files, int fileCountLimit) {
            var uploadImages = new List<short>();
            foreach (var file in files.TakeWhile(file => uploadImages.Count < fileCountLimit)) {
                uploadImages.Add(UploadImage(file.Item1, file.Item2));
            }
            return uploadImages;
        }
        private static short UploadImage(PhotoFormat imageFormat, byte[] photoToUpload) {
            var photoID = default(short);
            var path = CombinePathFromID(photoID) + "." + GetTypeImageExtension(imageFormat);
            if (System.IO.File.Exists(path)) {
                System.IO.File.Delete(path);
            }
            using (var f = System.IO.File.Create(path)) {
                f.Write(photoToUpload, 0, photoToUpload.Length);
            }
            return photoID;
        }
        
        private static string CombinePathFromID(short id) {
            return Path.Combine(_imageStorePath, (id % _subFolderCnt).ToString(), id.ToString());
        }

        private static string GetMimeTypeByFileName(string fileName) {
            switch ((Path.GetExtension(fileName) ?? string.Empty).ToLower()) {
                case ".png":
                    return "image/png";
                case ".jpeg":
                case ".jpg":
                default:
                    return "image/jpeg";
            }
        }

        private static string GetTypeImageExtension(PhotoFormat photoFormat) {
            string type;
            switch (photoFormat) {
                case PhotoFormat.Png:
                    type = "png";
                    break;
                case PhotoFormat.Jpeg:
                default:
                    type = "jpg";
                    break;
            }
            return type;
        }
    }
}