using System;
using System.Collections.Generic;
using CommonUtils.Code;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;

namespace Project_B.CodeServerSide.BrokerProvider.Interfaces {
    public interface IQueryableWrapper : ICloneable {
        WebRequestHelper RequestHelper { get; }
        string LoadPage(string url, List<string> postData, string contentType);
        void SetProxy(string proxy);
        void SetCookies(string domain, string[] cookies);
        void ProcessConfig(BrokerConfiguration currentConfiguration);
    }
}