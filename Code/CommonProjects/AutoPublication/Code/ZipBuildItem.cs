using System;

namespace AutoPublication.Code {
    public struct ZipBuildItem {
        public string FilePath { get; private set; }

        public ZipBuildItem(string filePath) {
            FilePath = filePath;
        }
    }
}