using System;
using System.Collections.Generic;
using System.Web;

namespace MainLogic.WebFiles {
    public abstract class FileControllerBase : ApplicationControllerBase {
        private const int _maxFileLength = 10 * 1024 * 1024;
        protected static Dictionary<string, Tuple<FileFormat, byte[]>> GetFilesFromRequest(HttpRequestBase baseRequest) {
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
                        case "css":
                        case "csv":
                        case "text":
                        case "txt":
                            imageFormat = FileFormat.Text;
                            break;
                        default:
                            continue;
                    }
                    files[file.FileName] = new Tuple<FileFormat, byte[]>(imageFormat, fileContent);
                }
            }
            return files;
        }
    }
}
