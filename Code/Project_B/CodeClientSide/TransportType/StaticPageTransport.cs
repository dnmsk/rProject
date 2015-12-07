using System;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.TransportType {
    public class StaticPageTransport {
        public int ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public LanguageType Languagetype { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; }
        public bool IsTop { get; set; }
        public bool IsPublished { get; set; }
        public DateTime LastModifyDateUtc { get; set; }

        public StaticPageTransport() {
            LastModifyDateUtc = DateTime.MinValue;
        }
    }
}