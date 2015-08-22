namespace MainLogic.Transport {
    public struct GuestTechInfoTransport {

        public string BrowserType { get; set; }

        public decimal Version { get; set; }

        public string Os { get; set; }

        public bool IsMobile { get; set; }

        public string UserAgent { get; set; }
    }
}
