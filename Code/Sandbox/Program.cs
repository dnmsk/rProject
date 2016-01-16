using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using AutoPublication.Code;
using CommonUtils.Code;
using CommonUtils.Core.Config;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbConfig;
using MainLogic.WebFiles;
using MainLogic.Wrapper;
using Project_B.CodeClientSide.Helper;
using Project_B.CodeServerSide;
using Project_B.CodeServerSide.Algorithm;
using Project_B.CodeServerSide.BrokerProvider;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.DataProvider.DataHelper.ProcessData;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Enums;
using Spywords_Project.Code;
using Spywords_Project.Code.Algorithms;

namespace Sandbox {
    class Program {
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(Program).FullName);
        private static readonly ConcurrentQueue<Action> _actionsToExecute = new ConcurrentQueue<Action>();
        static void Main(string[] args) {
            try {
                RegisterDB();
                var fileContent = File.ReadAllLines("e:\\domains.csv");
                for (var index = 0; index < fileContent.Length; index++) {
                    var s = fileContent[index];
                    if (s.IsNullOrWhiteSpace()) {
                        continue;
                    }

                    var index1 = index;
                    _actionsToExecute.Enqueue(() => CollectAndWriteToArray(s, fileContent, index1)); 
                }
                50.Steps(i => {
                    new Thread(() => {
                        Action act;
                        while (_actionsToExecute.TryDequeue(out act)) {
                            act();
                        }
                    }).Start();
                });


                while (_actionsToExecute.Any()) {
                    Thread.Sleep(1000);
                }

                File.WriteAllLines("e:\\domains_collected.csv", fileContent, Encoding.GetEncoding(1251));

                Console.WriteLine("End: " + DateTime.Now);
                while (true) {
                    Thread.Sleep(1000);
                }

                //Console.WriteLine(start.ElapsedMilliseconds);
                //Console.ReadLine();
            } catch (Exception ex) {
                Console.WriteLine(ex);
                _logger.Error(ex);
                Console.ReadLine();
            }
            Console.WriteLine("Success");
            //Console.ReadLine();
        }

        private static void CollectAndWriteToArray(string s, string[] fileContent, int index) {
            fileContent[index] = s = s.Trim();
            Tuple<HttpStatusCode, string> siteContent;
            try {
                siteContent = WebRequestHelper.GetContentWithStatus("http://" + DomainExtension.PunycodeDomain(s));
            } catch (Exception ex) {
                fileContent[index] = new[] { s, "не ответил" }.StrJoin(";");
                return;
            }
            var emails = CollectEmailPhoneFromDomain.GetEmailFromContent(siteContent.Item2);
            var phones = CollectEmailPhoneFromDomain.GetPhoneFromContent(siteContent.Item2);
            fileContent[index] = new[] { s, (phones ?? new string[0]).StrJoin(","), (emails ?? new string[0]).StrJoin(",") }.StrJoin(";");
        }

        static void RegisterDB() {
            ConfigHelper.RegisterConfigTarget("Application.xml", xml => {
                var xmlDoc = (XmlDocument)xml;
                var container = xmlDoc.GetElementsByTagName("ConnectionStrings");
                if (container.Count != 1) {
                    return;
                }
                foreach (XmlNode connectionItem in container[0].ChildNodes) {
                    DatabaseActions.AddConnectionOption(
                        connectionItem.Attributes["name"].Value,
                        new ConnectOptions(
                            connectionItem.Attributes["connectionString"].Value,
                            uint.Parse(connectionItem.Attributes["maxConnCount"].Value),
                            uint.Parse(connectionItem.Attributes["maxRetries"].Value),
                            (DbServerType)Enum.Parse(typeof(DbServerType), connectionItem.Attributes["dbServerType"].Value))
                    );
                }
            });
        }
    }
}
