using System;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.Interface.NameConstraint {
    public interface ILanguageTyped {
        /// <summary>
        /// 
        /// </summary>
        LanguageType Languagetype { get; set; }
        Enum LanguageTypeField { get; }
    }
}