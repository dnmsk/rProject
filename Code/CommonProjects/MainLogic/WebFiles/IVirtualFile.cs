namespace MainLogic.WebFiles {
    public interface IVirtualFile {
        /// <summary>
        /// имя файла
        /// </summary>
        string VirtualPath { get; }
        /// <summary>
        /// контент файла
        /// </summary>
        /// <returns></returns>
        string GetContent();
    }
}
