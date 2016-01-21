using System;
using Project_B.CodeServerSide.Data.Result;

namespace Project_B.CodeClientSide.TransportType.ModerateTransport {
    public class RawCompetitionItemTransport : RawEntityWithLink {
        public FullResult RawResult { get; set; }
        public FullResult Result { get; set; }

        public RawEntityWithLink Competitior1 { get; set; }
        public RawEntityWithLink Competitior2 { get; set; }
    }
}