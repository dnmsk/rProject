﻿using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.TransportType {
    public class BrokerPageTransport : StaticPageTransport {
        public BrokerType BrokerType { get; set; }

        public string Pageurl { get; set; }

        public string TargetUrl { get; set; }
        
        /// <summary>
        /// </summary>
        public string Largeiconclass { get; set; }

        /// <summary>
        /// </summary>
        public short Orderindex { get; set; }
    }
}