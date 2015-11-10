using System;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.Interface {
    public interface IStaticPage {
        /// <summary>
        /// 
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        LanguageType Languagetype { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Keywords { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        DateTime Datecreatedutc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        DateTime? Datepublishedutc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        DateTime Datemodifiedutc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool Istop { get; set; }

        bool Save();
    }
}