﻿using System;

namespace Project_B.CodeClientSide.TransportType {
    public class SiteMapItem {
        public string Location { get; set; }
        public DateTime LastMod { get; set; }
        public float Priority { get; set; }
        public SiteMapChangeFreq ChangeFreq { get; set; }
    }
}