using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitBotBackToTheFuture
{
    class JsonParse
    {
        private static string location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
        private static string configJsonFileDirectory = location + "key.txt";

        public string bitmexKey { get; private set; }
        public string bitmexSecret { get; private set; }
        public string bitmexKeyWeb { get; private set; }
        public string bitmexSecretWeb { get; private set; }
        public string timeGraph { get; private set; }
        public string statusShort { get; set; }
        public string statusLong { get; set; }
        public string pair { get; private set; }
        public int qtdyContacts { get; private set; }
        public int interval { get; private set;}
        public int intervalOrder { get; private set; }
        public int intervalCapture { get; private set; }
        public int intervalCancelOrder { get; private set; }
        public int positionContracts { get; set; }
        public double profit { get; private set;}
        public int limiteOrder { get; private set; }
        public double fee { get; private set; }
        public double stoploss { get; private set; }
        public double stopgain { get; private set; }
        public string bitmexDomain { get; private set; }
        public bool roeAutomatic { get; private set; }
        public string webserverConfig { get; private set; }
        public string webserver { get; private set; }
        public List<JToken> indicatorsEntry { get; private set; }
        public List<JToken> indicatorsEntryCross { get; private set; }
        public List<JToken> indicatorsEntryDecision { get; private set; }


        public JsonParse() {
            parseJsonToObject();
        }

        private JContainer readJsonFile() {
            String jsonConfigInString = System.IO.File.ReadAllText(configJsonFileDirectory);
            return (JContainer)JsonConvert.DeserializeObject(jsonConfigInString, (typeof(JContainer))); ;
        }

        void parseJsonToObject(){
            JContainer configJsonInArrayList = readJsonFile();

            bitmexKey = configJsonInArrayList["key"].ToString();
            bitmexSecret = configJsonInArrayList["secret"].ToString();
            bitmexKeyWeb = configJsonInArrayList["webserverKey"].ToString();
            bitmexSecretWeb = configJsonInArrayList["webserverSecret"].ToString();
            bitmexDomain = configJsonInArrayList["domain"].ToString();
            statusShort = configJsonInArrayList["short"].ToString();
            statusLong = configJsonInArrayList["long"].ToString();
            pair = configJsonInArrayList["pair"].ToString();
            timeGraph = configJsonInArrayList["timeGraph"].ToString();
            qtdyContacts = int.Parse(configJsonInArrayList["contract"].ToString());
            interval = int.Parse(configJsonInArrayList["interval"].ToString());
            intervalCancelOrder = int.Parse(configJsonInArrayList["intervalCancelOrder"].ToString());
            intervalOrder = int.Parse(configJsonInArrayList["intervalOrder"].ToString());
            intervalCapture = int.Parse(configJsonInArrayList["webserverIntervalCapture"].ToString());
            profit = double.Parse(configJsonInArrayList["profit"].ToString());
            fee = double.Parse(configJsonInArrayList["fee"].ToString());
            stoploss = double.Parse(configJsonInArrayList["stoploss"].ToString());
            stopgain = double.Parse(configJsonInArrayList["stopgain"].ToString());
            roeAutomatic = configJsonInArrayList["roe"].ToString() == "automatic";
            limiteOrder = int.Parse(configJsonInArrayList["limiteOrder"].ToString());
            webserverConfig = configJsonInArrayList["webserverConfig"].ToString();
            webserver = configJsonInArrayList["webserver"].ToString();
            indicatorsEntry = configJsonInArrayList["indicatorsEntry"].ToList();
            indicatorsEntryCross = configJsonInArrayList["indicatorsEntryCross"].ToList();
            indicatorsEntryDecision = configJsonInArrayList["indicatorsEntryDecision"].ToList();
        }

        internal string getValue(String nameList, String nameIndicator, String nameParameter)
        {
            JContainer configJsonInArrayList = readJsonFile();
            foreach (var item in configJsonInArrayList[nameList])
                if (item["name"].ToString().Trim().ToUpper() == nameIndicator.ToUpper().Trim())
                    return item[nameParameter].ToString().Trim();
            return null;
        }

    }
}
