﻿using System;
using System.Linq;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider {
    public class DbBrokerProvider : BrokerBase {
        public DbBrokerProvider(BrokerType brokerType) : base(null) {
            BrokerType = brokerType;
        }

        public override BrokerType BrokerType { get; }

        public override BrokerData LoadResult(DateTime date, SportType sportType, LanguageType language) {
            var rawCiDs = RawCompetitionItem.DataSource
                .WhereEquals(RawCompetitionItem.Fields.Brokerid, (short) BrokerType)
                .WhereEquals(RawCompetitionItem.Fields.Languagetype, (short) language)
                .Where(RawCompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, date)
                .Where(RawCompetitionItem.Fields.Dateeventutc, Oper.Less, date.AddDays(1));
            if (sportType != SportType.Unknown && sportType != SportType.All) {
                rawCiDs = rawCiDs
                    .Where(new DbFnSimpleOp(RawCompetitionItem.Fields.Sporttype, FnMathOper.Add, (short) sportType), Oper.Greater, default(int));
            }
            var rawCompetitionItems = rawCiDs
                .AsList();
            var rawCompetitorsMap = RawCompetitor.DataSource
                .WhereIn(RawCompetitor.Fields.ID, rawCompetitionItems.Select(rci => rci.Competitoruniqueid1).Union(rawCompetitionItems.Select(rci => rci.Competitoruniqueid2)).Distinct())
                .AsMapByIds(RawCompetitor.Fields.ID, RawCompetitor.Fields.Name);
            var rawCompetitionSpecifyMap = RawCompetitionSpecify.DataSource
                .WhereIn(RawCompetitionSpecify.Fields.ID, rawCompetitionItems.Select(rci => rci.RawcompetitionspecifyID).Distinct())
                .AsMapByIds(RawCompetitionSpecify.Fields.ID, RawCompetitionSpecify.Fields.Name);
            var rawResultMap = RawCompetitionResult.DataSource
                .WhereIn(RawCompetitionResult.Fields.RawcompetitionitemID, rawCompetitionItems.Select(rci => rci.ID))
                .AsMapByIds(RawCompetitionResult.Fields.RawcompetitionitemID, RawCompetitionResult.Fields.Rawresultstring);

            var result = new BrokerData(BrokerType, language, rawCompetitionItems
                .GroupBy(rci => rci.RawcompetitionspecifyID)
                .Select(ge => {
                    var competitionParsed = new CompetitionParsed(FormatCompetitionName(rawCompetitionSpecifyMap[ge.Key].Name), ge.First().SportType);
                    competitionParsed.Matches
                        .AddRange(ge.Select(rci => new MatchParsed {
                            CompetitorNameFullOne = rawCompetitorsMap[rci.Rawcompetitorid1].Name,
                            CompetitorNameFullTwo = rawCompetitorsMap[rci.Rawcompetitorid2].Name,
                            DateUtc = rci.Dateeventutc,
                            Result = rawResultMap.TryGetValueOrDefault(rci.ID)?.Rawresult
                        })
                        .ToList()
                    );
                    return competitionParsed;
                })
                .ToList()
            );
            return result;
        }

        public override BrokerData LoadLive(SportType sportType, LanguageType language) {
            throw new NotImplementedException();
        }

        public override BrokerData LoadRegular(SportType sportType, LanguageType language) {
            throw new NotImplementedException();
        }
    }
}