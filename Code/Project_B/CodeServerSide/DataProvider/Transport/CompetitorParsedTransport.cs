﻿using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.Transport {
    public class CompetitorParsedTransport {
        public int ID { get; set; }
        public LanguageType LanguageType { get; set; }
        public SportType SportType { get; set; }
        public GenderType GenderType { get; set; }
    }
}