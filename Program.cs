using System;
using System.Collections.Generic;
using System.Linq;
using ActressMas;

namespace Coursework_MAS
{
    class Program
    {
        private static void Main(string[] args)
        {
            double avgRate = 0;
            double avgBreak = 0;
            double avgProfit = 0;
            double avgPrice = 0;

            for(int j = 0; j < 1 ; j++)
            {
                var env = new EnvironmentMas(parallel: false, randomOrder: false);
                //var env = new EnvironmentMas(parallel: false, randomOrder: true);

                var rand = new Random();

                //setup bidders
                for (int i = 1; i <= Settings.NoBidders; i++)
                {
                    int bidderBand = Settings.MinBand + rand.Next(Settings.MaxBand - Settings.MinBand);
                    int bidderRam = Settings.MinRam + rand.Next(Settings.MaxRam - Settings.MinRam);
                    int bidderCpu = Settings.MinCpu + rand.Next(Settings.MaxCpu - Settings.MinCpu);

                    int agentValuation = Settings.MinPrice + rand.Next(Settings.MaxPrice - Settings.MinPrice);
                    var bidderAgent = new BidderAgent(agentValuation, bidderBand, bidderRam, bidderCpu);
                    env.Add(bidderAgent, $"bidder{i:D2}");
                }

                //add servers
                for (int i = 1; i <= Settings.NoServers; i++)
                {
                    int serverCost = Settings.MinAsk + rand.Next(Settings.MaxAsk - Settings.MinAsk); //?????

                    int serverBand = Settings.MinBandS + rand.Next(Settings.MaxBandS - Settings.MinBandS);
                    int serverRam = Settings.MinRamS + rand.Next(Settings.MaxRamS - Settings.MinRamS);
                    int serverCpu = Settings.MinCpuS + rand.Next(Settings.MaxCpuS - Settings.MinCpuS);
                    var serverAgent = new ServerAgent(serverCost, serverBand, serverRam, serverCpu);
                    env.Add(serverAgent, $"server{i:D2}");
                }

                //auctioneer and start
                var auctioneerAgent = new AuctioneerAgent();
                env.Add(auctioneerAgent, "auctioneer");

                env.Start();
                Console.WriteLine(j);
            }

            //calculate avg successrate
            for (int i = 0; i < Settings.successRates.Count; i++)
            {
                avgRate += Settings.successRates[i];
            }
            avgRate = avgRate / Settings.successRates.Count;


            //avg break even price
            for (int i = 0; i < Settings.breakevens.Count; i++)
            {
                 avgBreak+= Settings.breakevens[i];
            }
            avgBreak = avgBreak / Settings.breakevens.Count;


            //avg pofit of sellers
            for (int i = 0; i < Settings.profits.Count; i++)
            {
                avgProfit += Settings.profits[i];
            }
            avgProfit = avgProfit / Settings.profits.Count;

            //standard deviation of prices
            double standardDeviation = 0;

            //avg of prices     
            for (int i = 0; i < Settings.prices.Count; i++)
            {
                avgPrice += Settings.prices[i];
            }
            avgPrice = avgPrice / Settings.prices.Count;     

            //calculate variance
            double sum = Settings.prices.Sum(d => Math.Pow(d - avgPrice, 2));     
            //std deviation
            standardDeviation = Math.Sqrt((sum) / (Settings.prices.Count() - 1));



            Console.WriteLine("Average successrate: " + avgRate);
            Console.WriteLine("Average breakeven: " + avgBreak);
            Console.WriteLine("Average profit: " + avgProfit);
            Console.WriteLine("Standard deviation of prices: " + standardDeviation);
            Console.WriteLine("Number of messages: " + Settings.noMessages);
            Console.ReadLine();
            }
    }
}
