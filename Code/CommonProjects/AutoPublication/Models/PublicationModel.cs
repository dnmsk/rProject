using System.Collections.Generic;
using AutoPublication.Code;

namespace AutoPublication.Models {
    public class PublicationModel {
        public List<BuildPublishItem> BuildPublishItems { get; set; }
        public List<ZipBuildItem> ZipBuildItems { get; set; }
    }
}