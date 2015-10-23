using System.IO;
using System.Web.Hosting;

namespace MainLogic.WebFiles {
    public class WebVirtualFile : VirtualFile {
        private readonly IVirtualFile _virtualFile;
        public WebVirtualFile(IVirtualFile virtualFile) : base(virtualFile.VirtualPath) {
            _virtualFile = virtualFile;
        }
        public override Stream Open() {
            var ms = new MemoryStream();
            var tw = new StreamWriter(ms);
            tw.Write(_virtualFile.GetContent());
            tw.Flush();
            ms.Position = 0;
            return ms;
        }
    }
}
