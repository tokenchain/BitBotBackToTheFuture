using BitBotBackToTheFuture;
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


enum TendencyMarket
{
    HIGH,
    NORMAL,
    LOW,
    VERY_LOW,
    VERY_HIGH
}

class MainClass
{


    //REAL NET
    public static string version = "0.0.0.11";
    public static string location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
    public static double roe = 0;
    public static TendencyMarket tendencyMarket = TendencyMarket.NORMAL;
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

    private static JsonParse configJson = new JsonParse();

    public static Object data = new Object();

    public static void Main(string[] args)
    {
        try
        {
            //Config
            Console.Title = "Loading...";

            Console.ForegroundColor = ConsoleColor.White;

            log("Deleron - Back to the future - v" + version + " - Bitmex version");
            log("by Matheus Grijo ", ConsoleColor.Green);
            log(" ======= HALL OF FAME BOTMEX  ======= ");
            log(" - Lucas Sousa", ConsoleColor.Magenta);
            log(" - Carlos Morato", ConsoleColor.Magenta);
            log(" - Luis Felipe Alves", ConsoleColor.Magenta);
            log(" ======= END HALL OF FAME BOTMEX  ======= ");

            log("http://botmex.ninja/");
            log("GITHUB http://github.com/matheusgrijo", ConsoleColor.Blue);
            log(" ******* DONATE ********* ");
            log("BTC 39DWjHHGXJh9q82ZrxkA8fiZoE37wL8jgh");
            log("BCH qqzwkd4klrfafwvl7ru7p7wpyt5z3sjk6y909xq0qk");
            log("ETH 0x3017E79f460023435ccD285Ff30Bd10834D20777");
            log("ETC 0x088E7E67af94293DB55D61c7B55E2B098d2258D9");
            log("LTC MVT8fxU4WBzdfH5XgvRPWkp7pE4UyzG9G5");
            log("Load config...");
            log("Considere DOAR para o projeto!", ConsoleColor.Green);
            log("Vamos aguardar 10 min para voce doar ;) ... ", ConsoleColor.Blue);
            log("ATENCAO, PARA FACILITAR A DOACAO DAQUI A 30 SEGUNDOS VAMOS ABRIR UMA PAGINA PARA VOCE!", ConsoleColor.Green);
            //System.Threading.Thread.Sleep(30000);
            //System.Diagnostics.Process.Start("https://www.blockchain.com/btc/payment_request?address=1AnttTLGhzJsX7T96SutWS4N9wPYuBThu8&amount_local=30&currency=USD&nosavecurrency=true");
            //log("Perfeito, agora aguarde os 9 minutos e 30 segundos restantes para iniciar o BOTMEX, enquanto isto estamos carregando as suas configuracoes...", ConsoleColor.Magenta);
            //String jsonConfig = System.IO.File.ReadAllText(location + "key.txt");
            //JContainer jCointaner = (JContainer)JsonConvert.DeserializeObject(jsonConfig, (typeof(JContainer)));

            //System.Threading.Thread.Sleep(60000 * 10);

           

            bitMEXApi = new BitMEX.BitMEXApi(configJson.bitmexKey, configJson.bitmexSecret, configJson.bitmexDomain);



            //TESTS HERE

            //makeOrder("Buy");

            //FINAL

            if (configJson.webserver == "enable")
            {
                WebServer ws = new WebServer(WebServer.SendResponse, configJson.webserverConfig);
                ws.Run();
                System.Threading.Thread tCapture = new Thread(Database.captureDataJob);
                tCapture.Start();
                System.Threading.Thread.Sleep(1000);
                System.Diagnostics.Process.Start(configJson.webserverConfig);

            }

            log("wait 1s...");
            System.Threading.Thread.Sleep(1000);
            log("Total open orders: " + bitMEXApi.GetOpenOrders(configJson.pair).Count);
            log("");
            log("Wallet: " + bitMEXApi.GetWallet());

            lstIndicatorsAll.Add(new IndicatorADX());
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

            foreach (var item in configJson.indicatorsEntry)
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

            foreach (var item in configJson.indicatorsEntryCross)
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

            foreach (var item in configJson.indicatorsEntryDecision)
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


            


            bool automaticTendency = configJson.statusLong == "automatic";

            //LOOP 
            while (true)
            {

                configJson.positionContracts = getPosition(); // FIX CARLOS MORATO
                roe = getRoe();


                //Stop Loss
                if (roe < 0)
                {
                    if ((roe * (-1)) >= configJson.stoploss)
                    {
                        //Stop loss
                        bitMEXApi.CancelAllOpenOrders(configJson.pair);
                        String side = "Buy";
                        if (configJson.positionContracts > 0)
                            side = "Sell";
                        bitMEXApi.MarketOrder(configJson.pair, side, configJson.positionContracts);
                    }
                }

                //Stop Gain
                if (roe > 0)
                {
                    if (roe >= configJson.stopgain)
                    {
                        //Stop loss
                        bitMEXApi.CancelAllOpenOrders(configJson.pair);
                        String side = "Buy";
                        if (configJson.positionContracts > 0)
                            side = "Sell";
                        bitMEXApi.MarketOrder(configJson.pair, side, configJson.positionContracts);
                    }
                }

                //SEARCH POSITION AND MAKE ORDER
                //By Carlos Morato
                if (configJson.roeAutomatic && (Math.Abs(getOpenOrderQty()) < Math.Abs(configJson.positionContracts)))
                {
                    log("Get Position " + configJson.positionContracts);

                    int qntContacts = (Math.Abs(configJson.positionContracts) - Math.Abs(getOpenOrderQty()));


                    if (configJson.positionContracts > 0)
                    {
                        string side = "Sell";
                        double priceContacts = Math.Abs(getPositionPrice());
                        double actualPrice = Math.Abs(getPriceActual(side));
                        double priceContactsProfit = Math.Abs(Math.Floor(priceContacts + (priceContacts * (configJson.profit + configJson.fee) / 100)));

                        if (actualPrice > priceContactsProfit)
                        {
                            double price = actualPrice + 1;
                            String json = bitMEXApi.PostOrderPostOnly(configJson.pair, side, price, qntContacts);
                            JContainer jCointaner2 = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                            log(json);

                        }
                        else
                        {
                            double price = priceContactsProfit;
                            String json = bitMEXApi.PostOrderPostOnly(configJson.pair, side, price, qntContacts);
                            JContainer jCointaner2 = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                            log(json);
                        }
                    }

                    if (configJson.positionContracts < 0)
                    {
                        string side = "Buy";
                        double priceContacts = Math.Abs(getPositionPrice());
                        double actualPrice = Math.Abs(getPriceActual(side));
                        double priceContactsProfit = Math.Abs(Math.Floor(priceContacts - (priceContacts * (configJson.profit + configJson.fee) / 100)));

                        if (actualPrice < priceContactsProfit)
                        {
                            double price = actualPrice - 1;
                            String json = bitMEXApi.PostOrderPostOnly(configJson.pair, side, price, qntContacts);
                            JContainer jCointaner2 = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                            log(json);

                        }
                        else
                        {
                            double price = priceContactsProfit;
                            String json = bitMEXApi.PostOrderPostOnly(configJson.pair, side, price, qntContacts);
                            JContainer jCointaner2 = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                            log(json);

                        }
                    }
                }





                //CANCEL ORDER WITHOUT POSITION
                //By Carlos Morato

                if (configJson.roeAutomatic && Math.Abs(configJson.positionContracts) != Math.Abs(getOpenOrderQty()))
                    bitMEXApi.CancelAllOpenOrders(configJson.pair);

                if (automaticTendency)
                    verifyTendency();
                //GET CANDLES
                if (getCandles())
                {

                    if (configJson.statusLong == "enable")
                    {
                        log("");
                        log("==========================================================");
                        log(" ==================== Verify LONG OPERATION =============", ConsoleColor.Green);
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
                                log("wait " + configJson.interval + "ms");
                                Thread.Sleep(configJson.interval);

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


                                if (configJson.getValue("indicatorsEntryDecision", item.getName(), "decision") == "enable" &&
                                    configJson.getValue("indicatorsEntryDecision", item.getName(), "tendency") == "enable")
                                {
                                    int decisionPoint = int.Parse(configJson.getValue("indicatorsEntryDecision", item.getName(), "decisionPoint"));
                                    if (item.getResult() >= decisionPoint && item.getTendency() == Tendency.high)
                                    {
                                        operation = "long";
                                        break;
                                    }
                                }

                                if (configJson.getValue("indicatorsEntryDecision", item.getName(), "decision") == "enable")
                                {
                                    int decisionPoint = int.Parse(configJson.getValue("indicatorsEntryDecision", item.getName(), "decisionPoint"));
                                    if (item.getResult() >= decisionPoint)
                                    {
                                        operation = "long";
                                        break;
                                    }
                                }
                                if (configJson.getValue("indicatorsEntryDecision", item.getName(), "tendency") == "enable")
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
                    }


                    if (configJson.statusShort == "enable")
                    {
                        //////////////////////////////////////////////////////////////
                        log("");
                        log("==========================================================");
                        log(" ==================== Verify SHORT OPERATION =============", ConsoleColor.Red);
                        log("==========================================================");
                        /////VERIFY OPERATION LONG
                        string operation = "sell";
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
                                log("wait " + configJson.interval + "ms");
                                Thread.Sleep(configJson.interval);

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


                                if (configJson.getValue("indicatorsEntryDecision", item.getName(), "decision") == "enable" && 
                                    configJson.getValue("indicatorsEntryDecision", item.getName(), "tendency") == "enable")
                                {
                                    int decisionPoint = int.Parse(configJson.getValue("indicatorsEntryDecision", item.getName(), "decisionPoint"));
                                    if (item.getResult() <= decisionPoint && item.getTendency() == Tendency.low)
                                    {
                                        operation = "short";
                                        break;
                                    }
                                }

                                if (configJson.getValue("indicatorsEntryDecision", item.getName(), "decision") == "enable")
                                {
                                    int decisionPoint = int.Parse(configJson.getValue("indicatorsEntryDecision", item.getName(), "decisionPoint"));
                                    if (item.getResult() <= decisionPoint)
                                    {
                                        operation = "short";
                                        break;
                                    }
                                }
                                if (configJson.getValue("indicatorsEntryDecision", item.getName(), "tendency") == "enable")
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

                }
                log("wait " + configJson.interval + "ms", ConsoleColor.Blue);
                Thread.Sleep(configJson.interval);

            }

        }
        catch (Exception ex)
        {
            log("ERROR FATAL::" + ex.Message + ex.StackTrace);
        }
    }


    static bool existsOrderOpenById(string id)
    {
        List<BitMEX.Order> lst = bitMEXApi.GetOpenOrders(configJson.pair);
        foreach (var item in lst)
        {
            if (item.OrderId.ToUpper().Trim() == id.ToUpper().Trim())
                return true;
        }
        return false;
    }


    static void makeOrder(string side)
    {
        bool execute = false;
        try
        {
            log("Make order " + side);

            if (side == "Sell" && configJson.statusShort == "enable" && Math.Abs(configJson.limiteOrder) > Math.Abs(bitMEXApi.GetOpenOrders(configJson.pair).Count))
            {
                double price = Math.Abs(getPriceActual(side) + 1);
                String json = bitMEXApi.PostOrderPostOnly(configJson.pair, side, price, configJson.qtdyContacts);
                JContainer jCointaner = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                log(json);
                log("wait total...");
                for (int i = 0; i < configJson.intervalCancelOrder; i++)
                {
                    if (!existsOrderOpenById(jCointaner["orderID"].ToString()))
                    {
                        price -= (price * configJson.profit) / 100;
                        price = Math.Abs(Math.Floor(price));
                        json = bitMEXApi.PostOrderPostOnly(configJson.pair, "Buy", price, configJson.qtdyContacts);
                        log(json);
                        execute = true;
                        break;
                    }
                    log("wait order limit 1s...");
                    Thread.Sleep(1000);
                }


                if (!execute)
                    bitMEXApi.DeleteOrders(jCointaner["orderID"].ToString());

            }
            if (side == "Buy" && configJson.statusLong == "enable" && Math.Abs(configJson.limiteOrder) > Math.Abs(bitMEXApi.GetOpenOrders(configJson.pair).Count))
            {
                double price = Math.Abs(getPriceActual(side) - 1);
                String json = bitMEXApi.PostOrderPostOnly(configJson.pair, side, price, configJson.qtdyContacts);
                JContainer jCointaner = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                log(json);
                log("wait total...");
                for (int i = 0; i < configJson.intervalCancelOrder; i++)
                {
                    if (!existsOrderOpenById(jCointaner["orderID"].ToString()))
                    {
                        price += (price * configJson.profit) / 100;
                        price = Math.Abs(Math.Floor(price));
                        json = bitMEXApi.PostOrderPostOnly(configJson.pair, "Sell", price, configJson.qtdyContacts);
                        log(json);
                        execute = true;
                        break;
                    }
                    log("wait order limit 1s...");
                    Thread.Sleep(1000);
                }


                if (!execute)
                    bitMEXApi.DeleteOrders(jCointaner["orderID"].ToString());


            }
        }
        catch (Exception ex)
        {
            log("makeOrder()::" + ex.Message + ex.StackTrace);
        }

        if (execute)
        {
            log("wait " + configJson.intervalOrder * 2 + "ms", ConsoleColor.Blue);
            Thread.Sleep(configJson.intervalOrder * 2);

        }
    }

    static void verifyTendency()
    {
        try
        {
            String json = Http.get("https://api.binance.com/api/v1/ticker/24hr?symbol=BTCUSDT");
            JContainer j = (Newtonsoft.Json.Linq.JContainer)JsonConvert.DeserializeObject(json);

            tendencyMarket = TendencyMarket.NORMAL;
            decimal priceChangePercent = decimal.Parse(j["priceChangePercent"].ToString().Replace(".", ","));
            if (priceChangePercent < 0m)
                tendencyMarket = TendencyMarket.LOW;
            if (priceChangePercent > 0m && priceChangePercent < 1.5m)
                tendencyMarket = TendencyMarket.NORMAL;
            if (priceChangePercent > 1.5m)
                tendencyMarket = TendencyMarket.HIGH;
            if (priceChangePercent < -2)
                tendencyMarket = TendencyMarket.VERY_LOW;
            if (priceChangePercent > 3.5m)
                tendencyMarket = TendencyMarket.VERY_HIGH;


            if (tendencyMarket == TendencyMarket.VERY_HIGH || tendencyMarket == TendencyMarket.HIGH)
            {
                configJson.statusShort = "disable";
                configJson.statusLong = "enable";
            }
            else if (tendencyMarket == TendencyMarket.NORMAL)
            {
                configJson.statusShort = "enable";
                configJson.statusLong = "enable";
            }
            else if (tendencyMarket == TendencyMarket.LOW || tendencyMarket == TendencyMarket.VERY_LOW)
            {
                configJson.statusShort = "enable";
                configJson.statusLong = "disable";
            }



        }
        catch (Exception ex)
        {

        }
    }


    static double getPriceActual(string type)
    {
        List<BitMEX.OrderBook> listBook = bitMEXApi.GetOrderBook(configJson.pair, 1);
        foreach (var item in listBook)
        {
            if (item.Side.ToUpper() == type.ToUpper())
                return item.Price;
        }

        return 0;
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
            List<BitMEX.Candle> lstCandle = bitMEXApi.GetCandleHistory(configJson.pair, 100, configJson.timeGraph);
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




            Console.Title = DateTime.Now.ToString() + " - " + configJson.pair + " - $ " + arrayPriceClose[99].ToString() + " v" + version + " - " + configJson.bitmexDomain + " | Price buy " + getPriceActual("Buy") + " | Price Sell " + getPriceActual("Sell") + " | " + tendencyMarket + "| Roe " + roe;
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
            List<BitMEX.Position> OpenPositions = bitMEXApi.GetOpenPositions(configJson.pair);
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

    static double getRoe()
    {
        try
        {
            List<BitMEX.Position> OpenPositions = bitMEXApi.GetOpenPositions(configJson.pair);
            double _roe = 0;
            foreach (var Position in OpenPositions)
                _roe += Position.percentual();
            return _roe;
        }
        catch (Exception ex)
        {
            log("getRoe::" + ex.Message + ex.StackTrace);
            return 0;
        }
    }

    //GetOpenOrderQty
    //by Carlos Morato
    static int getOpenOrderQty()
    {
        try
        {
            List<BitMEX.Order> OpenOrderQty = bitMEXApi.GetOpenOrders(configJson.pair);
            int _contactsQty = 0;
            foreach (var Order in OpenOrderQty)
                if (Order.Side == "Sell")
                    _contactsQty += (int)Order.OrderQty * (-1);
                else
                    _contactsQty += (int)Order.OrderQty;
            return _contactsQty;
        }
        catch (Exception ex)
        {
            log("getOpenOrderQty::" + ex.Message + ex.StackTrace);
            return 0;
        }
    }


    //GetPositionPrice
    //by Carlos Morato
    static double getPositionPrice()
    {
        try
        {
            List<BitMEX.Position> OpenPositionsPrice = bitMEXApi.GetOpenPositions(configJson.pair);
            double _priceContacts = 0;
            foreach (var Position in OpenPositionsPrice)
                _priceContacts = (double)Position.AvgEntryPrice;
            return _priceContacts;
        }
        catch (Exception ex)
        {
            log("getPositionPrice::" + ex.Message + ex.StackTrace);
            return 0;
        }
    }

    static void log(string value, ConsoleColor color = ConsoleColor.White)
    {
        try
        {

            value = "[" + DateTime.Now.ToString() + "] - " + value;
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ForegroundColor = ConsoleColor.White;

            System.IO.StreamWriter w = new StreamWriter(location + DateTime.Now.ToString("yyyyMMdd") + "_log.txt", true);
            w.WriteLine(value);
            w.Close();
            w.Dispose();

        }
        catch { }
    }



}





