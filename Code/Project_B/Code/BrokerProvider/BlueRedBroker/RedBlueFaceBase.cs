using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
using HtmlAgilityPack;
using Project_B.Code.BrokerProvider.Configuration;
using Project_B.Code.BrokerProvider.HtmlDataExtractor;
using Project_B.Code.Data;
using Project_B.Code.DataProvider.DataHelper;
using Project_B.Code.Enums;

namespace Project_B.Code.BrokerProvider.BlueRedBroker {
    public abstract class RedBlueFaceBase : BrokerBase {
        public override BrokerType BrokerType {
            get { return BrokerType.RedBlue; }
        }

        protected RedBlueFaceBase(WebRequestHelper requestHelper) : base(requestHelper) {}

        public override List<CompetitionParsed> BuildCompetitions(string htmlContent) {
            var result = new List<CompetitionParsed>();
            var htmlBlockHelper = new HtmlBlockHelper(htmlContent);
            var dateTimeFixer = GetGmtFixer(htmlContent);
            var groupedCompetitions = htmlBlockHelper.ExtractBlock(CurrentConfiguration.XPath[SectionName.XPathToCategoryContainer]);
            foreach (var groupedCompetition in groupedCompetitions) {
                result.AddRange(ExtractMatchesBlockFromCompetitonGroup(groupedCompetition, dateTimeFixer));
            }
            return result;
        }

        protected List<CompetitionParsed> ExtractMatchesBlockFromCompetitonGroup(HtmlNode node, DateTimeToGmtFixer dateTimeFixer) {
            var result = new List<CompetitionParsed>();
            var competitonName = string.Empty;
            var type = SportType.Unknown;
            var nameBlock = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToCategoryName]);
            if (nameBlock.Count > 0) {
                competitonName = nameBlock[0].InnerText.Trim();
            }
            var listMatchBlockForCompetitions = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToListCompetitionInCategory]);
            foreach (var listMatchBlockForCompetition in listMatchBlockForCompetitions) {
                var competiton = new CompetitionParsed();
                if (!competitonName.IsNullOrWhiteSpace()) {
                    competiton.Name.Add(competitonName);
                }
                nameBlock = HtmlBlockHelper.ExtractBlock(listMatchBlockForCompetition, CurrentConfiguration.XPath[SectionName.XPathToCompetitionName]);
                if (nameBlock.Count > 0) {
                    competiton.Name.AddRange(nameBlock[0].InnerText.RemoveAllTags().Replace("&nbsp;", " ").Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));
                }
                if (type != SportType.Unknown || type == SportType.Unknown && (type = SportTypeHelper.Instance[competiton.Name]) != SportType.Unknown) { 
                    competiton.Type = type;
                }
                if (type == SportType.Unknown) {
                    continue;
                }
                var matches = ExtractMatchesFromMatchesBlock(listMatchBlockForCompetition, type, dateTimeFixer);
                if (matches.Any()) {
                    competiton.Matches = matches;
                    result.Add(competiton);
                }
            }
            return result;
        }

        private static DateTimeToGmtFixer GetGmtFixer(string htmlContent) {
            var idx = htmlContent.IndexOf("\"gmt", StringComparison.InvariantCultureIgnoreCase);
            var num = htmlContent.Substring(idx + 4);
            num = num.Substring(0, num.IndexOf("\"", StringComparison.InvariantCultureIgnoreCase));
            return new DateTimeToGmtFixer(StringParser.ToShort(num, 0));
        }

        protected DateTime ParseDateTime(string date) {
            date = date.ToLower().Replace("мая", "май");
            var defaultDateTime = DateTime.MinValue;
            foreach (var dateTimeFormat in CurrentConfiguration.StringArray[SectionName.ArrayDateTimeFormat]) {
                var dateTime = StringParser.ToDateTime(date, defaultDateTime, dateTimeFormat);
                if (!dateTime.Equals(defaultDateTime)) {
                    return dateTime;
                }
            }
            return defaultDateTime;
        }

        protected abstract List<MatchParsed> ExtractMatchesFromMatchesBlock(HtmlNode node, SportType type, DateTimeToGmtFixer dateTimeFixer);
    }
}