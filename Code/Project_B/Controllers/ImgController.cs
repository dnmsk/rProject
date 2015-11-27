using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using FreeImageAPI;
using MainLogic.WebFiles;
using Project_B.CodeServerSide.Enums;

namespace Project_B.Controllers {
    public class ImgController : ApplicationControllerBase {
        private const string IMAGE_STORE_PATH_CONFIG = "ImageStorePath";
        private const int IMAGE_HASH_LENGTH = 4;
        private const int SubFolderCnt = 100;
        private const int MAX_FILE_LENGTH = 1 * 1024 * 1024;
        private static readonly string _imageStorePath;

        static ImgController() {
            _imageStorePath = SiteConfiguration.GetConfigurationProperty(IMAGE_STORE_PATH_CONFIG);
        }

        [HttpGet]
        public ActionResult Index(short id, string hash) {
            var path = CombinePathFromID(id) + hash.Substring(IMAGE_HASH_LENGTH);
            if (!id.ToString().GetMD5(IMAGE_HASH_LENGTH).Equals(hash.Substring(0, IMAGE_HASH_LENGTH), StringComparison.InvariantCultureIgnoreCase)
                || !System.IO.File.Exists(path)) {
                return new EmptyResult();
            }
            /* Thu, 20 Mar 2014 09:21:47 GMT */
            Response.AddHeader("Expires", DateTime.UtcNow.AddYears(1).ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", CultureInfo.InvariantCulture));
            return File(path, GetMimeTypeByFileName(path));
        }

        public static string GetImgPath(short imageID, PhotoFormat photoFormat) {
            var path = CombinePathFromID(imageID);
            switch (photoFormat) {
                case PhotoFormat.Png:
                    return path + ".png";
                case PhotoFormat.Jpeg:
                default:
                    return path + ".jpg";
            }
        }

        public static List<short> UploadImageFromRequest(HttpRequestBase baseRequest, int fileCountLimit = 1) {
            var rejectedFileNames = new List<string>(baseRequest.Files.Count);
            var files = new Dictionary<string, Tuple<PhotoFormat, byte[]>>();

            for (var i = 0; i < baseRequest.Files.Count; i++) {
                var file = baseRequest.Files[i];
                var mimeTypeSplitted = file.ContentType.ToLower().Split('/');
                if (file != null && file.ContentLength != 0) {
                    if (mimeTypeSplitted[0] != "image" || file.ContentLength > MAX_FILE_LENGTH) {
                        rejectedFileNames.Add(file.FileName);
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
            foreach (var file in files) {
                if (uploadImages.Count >= fileCountLimit) {
                    break;
                }
                uploadImages.Add(UploadImage(file.Item1, file.Item2));
            }
            return uploadImages;
        }
        private static short UploadImage(PhotoFormat imageFormat, byte[] photoToUpload) {
            var photoID = default(short);
            Bitmap img;
            using (var steam = new MemoryStream(photoToUpload)) {
                img = new Bitmap(Image.FromStream(steam));
            }
            var ratio = (float)img.Height / img.Width;
            SaveOnDisk(img, GetImgPath(photoID, imageFormat), imageFormat, FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYSUPERB | FREE_IMAGE_SAVE_FLAGS.JPEG_PROGRESSIVE);
            return photoID;
        }
        
        private static void SaveOnDisk(Bitmap bmp, string path, PhotoFormat photoFormat, FREE_IMAGE_SAVE_FLAGS saveFlags) {
            FREE_IMAGE_FORMAT format;
            switch (photoFormat) {
                case PhotoFormat.Png:
                    format = FREE_IMAGE_FORMAT.FIF_PNG;
                    saveFlags = FREE_IMAGE_SAVE_FLAGS.PNG_Z_BEST_COMPRESSION;
                    break;
                case PhotoFormat.Jpeg:
                default:
                    format = FREE_IMAGE_FORMAT.FIF_JPEG;
                    break;
            }
            if (System.IO.File.Exists(path)) {
                System.IO.File.Delete(path);
            }
            FreeImage.SaveBitmap(bmp, path, format, saveFlags);
        }

        private static string CombinePathFromID(short id) {
            return Path.Combine(_imageStorePath, (id % SubFolderCnt).ToString(), id.ToString());
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
    }
}