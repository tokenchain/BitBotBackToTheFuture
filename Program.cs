using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

class MainClass
{


    //REAL NET
    public static string version = "0.0.0.8";
    public static string location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
    public static string bitmexKey = "";
    public static string bitmexSecret = "";
    public static string bitmexKeyWeb = "";
    public static string bitmexSecretWeb = "";
    public static string timeGraph = "";
    public static string statusShort = "";
    public static string statusLong = "";
    public static string pair = "";
    public static int qtdyContacts = 0;
    public static int qtdyContactsPosition = 0; // by Lucas Sousa ;)
    public static int interval = 0;
    public static int intervalOrder = 0;
    public static int intervalCapture = 0;
    public static double profit = 0;
    public static double fee = 0;
    public static string bitmexDomain = "";
    public static BitMEX.BitMEXApi bitMEXApi = null;

    public static List<IIndicator> lstIndicatorsAll = new List<IIndicator>();
    public static List<IIndicator> lstIndicatorsEntry = new List<IIndicator>();
    public static List<IIndicator> lstIndicatorsEntryCross = new List<IIndicator>();
    public static List<IIndicator> lstIndicatorsEntryDecision = new List<IIndicator>();

    public static double[] arrayPriceClose = new double[100];
    public static double[] arrayPriceHigh = new double[100];
    public static double[] arrayPriceLow = new double[100];
    public static double[] arrayPriceVolume = new double[100];
    public static double[] arrayPriceOpen = new double[100];

    public static double[] arraypositionPrice = new double[100];
    public static double[] arrayLiquidationPrice = new double[100];
    public static double[] arraycontrats = new double[100];


    public static Object data = new Object();

    public static void Main(string[] args)
    {
        try
        {
            //Config
            Console.ForegroundColor = ConsoleColor.Green;

            log("Deleron - Back to the future - v" + version + " - Bitmex version");
            log("by Matheus Grijo Thanks Lucas Sousa ;) ");
            log("http://botmex.ninja/");
            log("GITHUB http://github.com/matheusgrijo");
            log(" ******* DONATE ********* ");
            log("BTC 39DWjHHGXJh9q82ZrxkA8fiZoE37wL8jgh");
            log("BCH qqzwkd4klrfafwvl7ru7p7wpyt5z3sjk6y909xq0qk");
            log("ETH 0x3017E79f460023435ccD285Ff30Bd10834D20777");
            log("ETC 0x088E7E67af94293DB55D61c7B55E2B098d2258D9");
            log("LTC MVT8fxU4WBzdfH5XgvRPWkp7pE4UyzG9G5");
            log("Load config...");
            log("Considere DOAR para o projeto!");
            log("Vamos aguardar 1 min para voce doar ;) ... ");
            System.Threading.Thread.Sleep(60000);

            String jsonConfig = System.IO.File.ReadAllText(location + "key.txt");
            JContainer jCointaner = (JContainer)JsonConvert.DeserializeObject(jsonConfig, (typeof(JContainer)));

            bitmexKey = jCointaner["key"].ToString();
            bitmexSecret = jCointaner["secret"].ToString();
            bitmexKeyWeb = jCointaner["webserverKey"].ToString();
            bitmexSecretWeb = jCointaner["webserverSecret"].ToString();
            bitmexDomain = jCointaner["domain"].ToString();
            statusShort = jCointaner["short"].ToString();
            statusLong = jCointaner["long"].ToString();
            pair = jCointaner["pair"].ToString();
            timeGraph = jCointaner["timeGraph"].ToString();
            qtdyContacts = int.Parse(jCointaner["contract"].ToString());
            interval = int.Parse(jCointaner["interval"].ToString());
            intervalOrder = int.Parse(jCointaner["intervalOrder"].ToString());
            intervalCapture = int.Parse(jCointaner["webserverIntervalCapture"].ToString());
            profit = double.Parse(jCointaner["profit"].ToString());
            fee = double.Parse(jCointaner["fee"].ToString());



            if (jCointaner["webserver"].ToString() == "enable")
            {
                WebServer ws = new WebServer(WebServer.SendResponse, jCointaner["webserverConfig"].ToString());
                ws.Run();
                System.Threading.Thread tCapture = new Thread(Database.captureDataJob);
                tCapture.Start();
                System.Threading.Thread.Sleep(1000);                
            }

            bitMEXApi = new BitMEX.BitMEXApi(bitmexKey, bitmexSecret, bitmexDomain);

            log("wait 1s...");
            System.Threading.Thread.Sleep(1000);
            log("Total open orders: " + bitMEXApi.GetOpenOrders(pair).Count);
            log("");
            log("Wallet: " + bitMEXApi.GetWallet());

            lstIndicatorsAll.Add(new IndicatorMFI());
            lstIndicatorsAll.Add(new IndicatorBBANDS());
            lstIndicatorsAll.Add(new IndicatorCCI());
            lstIndicatorsAll.Add(new IndicatorCMO());
            lstIndicatorsAll.Add(new IndicatorDI());
            lstIndicatorsAll.Add(new IndicatorDM());
            lstIndicatorsAll.Add(new IndicatorMA());
            lstIndicatorsAll.Add(new IndicatorMACD());
            lstIndicatorsAll.Add(new IndicatorMOM());
            lstIndicatorsAll.Add(new IndicatorPPO());
            lstIndicatorsAll.Add(new IndicatorROC());
            lstIndicatorsAll.Add(new IndicatorRSI());
            lstIndicatorsAll.Add(new IndicatorSAR());
            lstIndicatorsAll.Add(new IndicatorSTOCH());
            lstIndicatorsAll.Add(new IndicatorSTOCHRSI());
            lstIndicatorsAll.Add(new IndicatorTRIX());
            lstIndicatorsAll.Add(new IndicatorULTOSC());
            lstIndicatorsAll.Add(new IndicatorWILLR());

            foreach (var item in jCointaner["indicatorsEntry"])
            {
                foreach (var item2 in lstIndicatorsAll)
                {
                    if (item["name"].ToString().Trim().ToUpper() == item2.getName().Trim().ToUpper())
                    {
                        item2.setPeriod(int.Parse((item["period"].ToString().Trim().ToUpper())));
                        lstIndicatorsEntry.Add(item2);
                    }
                }
            }

            foreach (var item in jCointaner["indicatorsEntryCross"])
            {
                foreach (var item2 in lstIndicatorsAll)
                {
                    if (item["name"].ToString().Trim().ToUpper() == item2.getName().Trim().ToUpper())
                    {
                        item2.setPeriod(int.Parse((item["period"].ToString().Trim().ToUpper())));
                        lstIndicatorsEntryCross.Add(item2);
                    }
                }
            }

            foreach (var item in jCointaner["indicatorsEntryDecision"])
            {
                foreach (var item2 in lstIndicatorsAll)
                {
                    if (item["name"].ToString().Trim().ToUpper() == item2.getName().Trim().ToUpper())
                    {
                        item2.setPeriod(int.Parse((item["period"].ToString().Trim().ToUpper())));
                        lstIndicatorsEntryDecision.Add(item2);
                    }
                }
            }


            if (jCointaner["webserver"].ToString() == "enable")
                System.Diagnostics.Process.Start(jCointaner["webserverConfig"].ToString());
            //LOOP 
            while (true)
            {
                //GET CANDLES
                if (getCandles())
                {

                    log("");
                    log("==========================================================");
                    log(" ==================== Verify LONG OPERATION =============");
                    log("==========================================================");
                    /////VERIFY OPERATION LONG
                    string operation = "buy";
                    //VERIFY INDICATORS ENTRY
                    foreach (var item in lstIndicatorsEntry)
                    {
                        Operation operationBuy = item.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayPriceVolume);
                        log("Indicator: " + item.getName());
                        log("Result1: " + item.getResult());
                        log("Result2: " + item.getResult2());
                        log("Operation: " + operationBuy.ToString());
                        log("");
                        if (operationBuy != Operation.buy)
                        {
                            operation = "nothing";
                            break;
                        }
                    }

                    //VERIFY INDICATORS CROSS
                    if (operation == "buy")
                    {
                        //Prepare to long                        
                        while (true)
                        {
                            log("wait operation long...");
                            getCandles();
                            foreach (var item in lstIndicatorsEntryCross)
                            {
                                Operation operationBuy = item.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayPriceVolume);
                                log("Indicator Cross: " + item.getName());
                                log("Result1: " + item.getResult());
                                log("Result2: " + item.getResult2());
                                log("Operation: " + operationBuy.ToString());
                                log("");

                                if (item.getTypeIndicator() == TypeIndicator.Cross)
                                {
                                    if (operationBuy == Operation.buy)
                                    {
                                        operation = "long";
                                        break;
                                    }
                                }
                                else if (operationBuy != Operation.buy)
                                {
                                    operation = "long";
                                    break;
                                }
                            }
                            if (lstIndicatorsEntryCross.Count == 0)
                                operation = "long";
                            if (operation != "buy")
                                break;
                            log("wait " + interval + "ms");
                            Thread.Sleep(interval);

                        }
                    }

                    //VERIFY INDICATORS DECISION
                    if (operation == "long" && lstIndicatorsEntryDecision.Count > 0)
                    {
                        operation = "decision";
                        foreach (var item in lstIndicatorsEntryDecision)
                        {
                            Operation operationBuy = item.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayPriceVolume);
                            log("Indicator Decision: " + item.getName());
                            log("Result1: " + item.getResult());
                            log("Result2: " + item.getResult2());
                            log("Operation: " + operationBuy.ToString());
                            log("");


                            if (getValue("indicatorsEntryDecision", item.getName(), "decision") == "enable" && getValue("indicatorsEntryDecision", item.getName(), "tendency") == "enable")
                            {
                                int decisionPoint = int.Parse(getValue("indicatorsEntryDecision", item.getName(), "decisionPoint"));
                                if (item.getResult() >= decisionPoint && item.getTendency() == Tendency.high)
                                {
                                    operation = "long";
                                    break;
                                }
                            }

                            if (getValue("indicatorsEntryDecision", item.getName(), "decision") == "enable")
                            {
                                int decisionPoint = int.Parse(getValue("indicatorsEntryDecision", item.getName(), "decisionPoint"));
                                if (item.getResult() >= decisionPoint)
                                {
                                    operation = "long";
                                    break;
                                }
                            }
                            if (getValue("indicatorsEntryDecision", item.getName(), "tendency") == "enable")
                            {                                
                                if (item.getTendency() == Tendency.high)
                                {
                                    operation = "long";
                                    break;
                                }
                            }

                        }
                    }


                    //EXECUTE OPERATION
                    if (operation == "long")
                        makeOrder("Buy");

                    ////////////FINAL VERIFY OPERATION LONG//////////////////



                    //////////////////////////////////////////////////////////////
                    log("");
                    log("==========================================================");
                    log(" ==================== Verify SHORT OPERATION =============");
                    log("==========================================================");
                    /////VERIFY OPERATION LONG
                    operation = "sell";
                    //VERIFY INDICATORS ENTRY
                    foreach (var item in lstIndicatorsEntry)
                    {
                        Operation operationBuy = item.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayPriceVolume);
                        log("Indicator: " + item.getName());
                        log("Result1: " + item.getResult());
                        log("Result2: " + item.getResult2());
                        log("Operation: " + operationBuy.ToString());
                        log("");
                        if (operationBuy != Operation.sell)
                        {
                            operation = "nothing";
                            break;
                        }
                    }

                    //VERIFY INDICATORS CROSS
                    if (operation == "sell")
                    {
                        //Prepare to long                        
                        while (true)
                        {
                            log("wait operation short...");
                            getCandles();
                            foreach (var item in lstIndicatorsEntryCross)
                            {
                                Operation operationBuy = item.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayPriceVolume);
                                log("Indicator Cross: " + item.getName());
                                log("Result1: " + item.getResult());
                                log("Result2: " + item.getResult2());
                                log("Operation: " + operationBuy.ToString());
                                log("");

                                if (item.getTypeIndicator() == TypeIndicator.Cross)
                                {
                                    if (operationBuy == Operation.sell)
                                    {
                                        operation = "short";
                                        break;
                                    }
                                }
                                else if (operationBuy != Operation.sell)
                                {
                                    operation = "short";
                                    break;
                                }
                            }
                            if (lstIndicatorsEntryCross.Count == 0)
                                operation = "short";
                            if (operation != "sell")
                                break;
                            log("wait " + interval + "ms");
                            Thread.Sleep(interval);

                        }
                    }

                    //VERIFY INDICATORS DECISION
                    if (operation == "short" && lstIndicatorsEntryDecision.Count > 0)
                    {
                        operation = "decision";
                        foreach (var item in lstIndicatorsEntryDecision)
                        {
                            Operation operationBuy = item.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayPriceVolume);
                            log("Indicator Decision: " + item.getName());
                            log("Result1: " + item.getResult());
                            log("Result2: " + item.getResult2());
                            log("Operation: " + operationBuy.ToString());
                            log("");


                            if (getValue("indicatorsEntryDecision", item.getName(), "decision") == "enable" && getValue("indicatorsEntryDecision", item.getName(), "tendency") == "enable")
                            {
                                int decisionPoint = int.Parse(getValue("indicatorsEntryDecision", item.getName(), "decisionPoint"));
                                if (item.getResult() <= decisionPoint && item.getTendency() == Tendency.low)
                                {
                                    operation = "short";
                                    break;
                                }
                            }

                            if (getValue("indicatorsEntryDecision", item.getName(), "decision") == "enable")
                            {
                                int decisionPoint = int.Parse(getValue("indicatorsEntryDecision", item.getName(), "decisionPoint"));
                                if (item.getResult() <= decisionPoint)
                                {
                                    operation = "short";
                                    break;
                                }
                            }
                            if (getValue("indicatorsEntryDecision", item.getName(), "tendency") == "enable")
                            {
                                if (item.getTendency() == Tendency.low)
                                {
                                    operation = "short";
                                    break;
                                }
                            }

                        }
                    }


                    //EXECUTE OPERATION
                    if (operation == "short")
                        makeOrder("Sell");

                    ////////////FINAL VERIFY OPERATION LONG//////////////////


                }
                log("wait " + interval + "ms");
                Thread.Sleep(interval);

            }

        }
        catch (Exception ex)
        {
            log("ERROR FATAL::" + ex.Message + ex.StackTrace);
        }
    }



    static void makeOrder(string side)
    {
        try
        {            
            log("Make order " + side);
            int position = getPosition();
            if (side == "Sell" && statusShort == "enable")
            {
                if (position > 0)                                    
                    position = (position*2) * (-1);                
                else
                    position = 0;
                position += qtdyContacts*(-1);
                String json = bitMEXApi.MarketOrder(pair, "Sell", position);                
                log(json);
                log("wait 2s...");
                System.Threading.Thread.Sleep(2000);               
            }
            if (side == "Buy" && statusLong == "enable")
            {
                if (position < 0)
                    position = (position * 2) * (-1);
                else
                    position = 0;
                position += qtdyContacts ;
                String json = bitMEXApi.MarketOrder(pair, "Buy", qtdyContacts + position);
                log(json);                
                log("wait 2s...");
                System.Threading.Thread.Sleep(2000);                
            }
        }
        catch (Exception ex)
        {
            log("makeOrder()::" + ex.Message + ex.StackTrace);
        }

        log("wait " + intervalOrder + "ms");
        Thread.Sleep(intervalOrder);

    }


    static bool getCandles()
    {
        try
        {
            arrayPriceClose = new double[100];
            arrayPriceHigh = new double[100];
            arrayPriceLow = new double[100];
            arrayPriceVolume = new double[100];
            arrayPriceOpen = new double[100];
            List<BitMEX.Candle> lstCandle = bitMEXApi.GetCandleHistory(pair, 100, timeGraph);
            int i = 0;
            foreach (var candle in lstCandle)
            {
                arrayPriceClose[i] = (double)candle.Close;
                arrayPriceHigh[i] = (double)candle.High;
                arrayPriceLow[i] = (double)candle.Low;
                arrayPriceVolume[i] = (double)candle.Volume;
                arrayPriceOpen[i] = (double)candle.Open;
                i++;
            }
            Array.Reverse(arrayPriceClose);
            Array.Reverse(arrayPriceHigh);
            Array.Reverse(arrayPriceLow);
            Array.Reverse(arrayPriceVolume);
            Array.Reverse(arrayPriceOpen);

            Console.Title = DateTime.Now.ToString() + " - " + pair + " - $ " + arrayPriceClose[99].ToString() + " v" + version + " - " + bitmexDomain;
            return true;
        }
        catch (Exception ex)
        {
            log("GETCANDLES::" + ex.Message + ex.StackTrace);
            //log("wait " + intervalOrder + "ms");
            //Thread.Sleep(intervalOrder);
            return false;
        }

    }

    //By Lucas Sousa modify MatheusGrijo
    static int getPosition()
    {
        try
        {
            List<BitMEX.Position> OpenPositions = bitMEXApi.GetOpenPositions(pair);
            int _qtdContacts = 0;
            foreach (var Position in OpenPositions)                         
                _qtdContacts += (int)Position.CurrentQty;                            
            return _qtdContacts;
        }
        catch (Exception ex)
        {
            log("getPosition::" + ex.Message + ex.StackTrace);
            return 0;
        }
    }

    ////Meu (Pegar Ordens Abertas)
    //static bool getOrder()
    //{
    //    try
    //    {
    //        var getorder = bitMEXApi.GetOrders(pair);
    //        string[] words = getorder.Split(' ');
    //        int i = 0;

    //        var array = getorder.Split(new string[] { "},{" }, // lista de separadores complexos
    //            StringSplitOptions.RemoveEmptyEntries);

    //        log("WORDS :" + array[0]);
    //        foreach (var Position in words)
    //        {
    //            // arraypositions[i] = (double)Position.Price;
    //            // arrayLiquidationPrice[i] = (double)Position.Price;
    //            log("ORDERS DENTRO :" + Position);


    //            i++;

    //        }

    //        log("Position FORA:" + arraypositionPrice[0].ToString());
    //        log("LiquidationPrice FORA:" + arrayLiquidationPrice[0].ToString());
    //        return true;




    //    }
    //    catch (Exception ex)
    //    {
    //        log("GETORDER::" + ex.Message + ex.StackTrace);
    //        return false;
    //    }
    //}


    static string getValue(String nameList, String nameIndicator, String nameParameter)
    {
        String jsonConfig = System.IO.File.ReadAllText(location + "key.txt");
        JContainer jCointaner = (JContainer)JsonConvert.DeserializeObject(jsonConfig, (typeof(JContainer)));
        foreach (var item in jCointaner[nameList])
            if (item["name"].ToString().Trim().ToUpper() == nameIndicator.ToUpper().Trim())
                return item[nameParameter].ToString().Trim();
        return null;
    }

    static void log(string value)
    {
        try
        {
            value = "[" + DateTime.Now.ToString() + "] - " + value;
            System.IO.StreamWriter w = new StreamWriter(location + DateTime.Now.ToString("yyyyMMdd") + "_log.txt", true);
            w.WriteLine(value);
            Console.WriteLine(value);
            w.Close();
            w.Dispose();
        }
        catch { }
    }



}
