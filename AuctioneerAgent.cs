using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActressMas;

namespace Coursework_MAS
{
    class AuctioneerAgent : Agent
    {
        //bid class for bidder info
        private class Bid
        {
            public string Bidder { get; set; }
            public int BidValue { get; set; }
            public int BidBand;
            public int BidRam;
            public int BidCpu;

            public void SetBand(int value)
            {
                this.BidBand = value;
            }
            public void SetRam(int value)
            {
                this.BidRam = value;
            }
            public void SetCpu(int value)
            {
                this.BidCpu = value;
            }

            public Bid(string bidder, int bidValue, int bidBand, int bidRam, int bidCpu)
            {
                Bidder = bidder;
                BidValue = bidValue;
                BidBand = bidBand;
                BidRam = bidRam;
                BidCpu = bidCpu;

            }
        }
        //ask class to stor server info
        private class Ask
        {
            public string Server { get; set; }
            public int AskValue { get; set; }
            public int AskBand;
            public int AskRam;
            public int AskCpu;

            public void SetBand(int value)
            {
                this.AskBand = value;
            }
            public void SetRam(int value)
            {
                this.AskRam = value;
            }
            public void SetCpu(int value)
            {
                this.AskCpu = value;
            }

            public Ask(string server, int askValue, int askBand, int askRam, int askCpu)
            {
                Server = server;
                AskValue = askValue;
                AskBand = askBand;
                AskRam = askRam;
                AskCpu = askCpu;
            }
        }

        //unsorted lists of bid and ask classes
        private List<Bid> _bids;
        private List<Ask> _asks;
        private int _turnsToWait;

        public AuctioneerAgent()
        {
            _bids = new List<Bid>();
            _asks = new List<Ask>();
        }

        public override void Setup()
        {
            Broadcast("start");
            _turnsToWait = 2;
        }

        //act: record bidder and seller info
        public override void Act(Message message)
        {
            try
            {
                Console.WriteLine($"\t{message.Format()}");
                message.Parse(out string action, out List<string> parameters);

                switch (action)
                {
                    case "bid":
                        HandleBid(message.Sender, Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]));
                        break;
                    
                    case "ask":
                        HandleAsk(message.Sender, Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]));
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void ActDefault()
        {
            if (--_turnsToWait <= 0)
                HandleFinish();
        }

        //set info through the classes, and list them
        private void HandleBid(string sender, int price, int band, int ram, int cpu)
        {
            _bids.Add(new Bid(sender, price, band, ram, cpu));
        }
        private void HandleAsk(string sender, int ask, int band, int ram, int cpu)
        {
            _asks.Add(new Ask(sender, ask, band, ram, cpu));
        }
        

        private void HandleFinish()
        {
            //sorting the bids in descending and asks in ascending order
            List<Bid> SortedBids = _bids.OrderByDescending(b => b.BidValue).ToList();
            List<Ask> SortedAsks = _asks.OrderBy(a => a.AskValue).ToList();

           
            int breakeven = 0;
            int k = 0;
            int bid = 0;
            int ask = 0;
            int index = 0;

            //find breakeven, where the bid is still bigger than ask
            while (index < SortedBids.Count && index < SortedAsks.Count)
            {
                ask = SortedAsks[index].AskValue;
                bid = SortedBids[index].BidValue;
                if (bid >= ask)
                {
                    breakeven = bid;
                    k = index;
                }
                index++;
            }
            Console.WriteLine("breakeven: " + breakeven + " k: " + k);
            Settings.breakevens.Add(breakeven);
            //write out valuations for checking
            for (int i = 0; i < SortedBids.Count; i++)
            {
                Console.WriteLine(SortedBids[i].Bidder + " value: " + SortedBids[i].BidValue);
                //prices list filled
                Settings.prices.Add(SortedBids[i].BidValue);
            }
            for (int i = 0; i < SortedAsks.Count; i++)
            {
                Console.WriteLine(SortedAsks[i].Server + " value: " + SortedAsks[i].AskValue);
                //prices list filled
                Settings.prices.Add(SortedAsks[i].AskValue);
            }
            
            List<string> winners = new List<string>();
            List<string> losers = new List<string>();

            // average price of breakeven index price
            int winningPrice = (SortedBids[k].BidValue + SortedAsks[k].AskValue) / 2; 
            Console.WriteLine("Price per unit to be paid: " + winningPrice);

            

            //main check for winners: check if the matched up server first then move to one above until we get to the last one (k index)
            int winnerCounter = 0;
            for (int i = 0; i <= k; i++)
            {
                for (int j = 0; j <= k; j++)
                {
                    int bidBand = SortedBids[i].BidBand;
                    int bidRam = SortedBids[i].BidRam;
                    int bidCpu = SortedBids[i].BidCpu;

                    int serverBand = SortedAsks[j].AskBand;
                    int serverRam = SortedAsks[j].AskRam;
                    int serverCpu = SortedAsks[j].AskCpu;

                    //check that the seller has the requirements for the buyer, and that the buyer still has things to buy 
                    //(meaning they're qualified for the auction)
                    if (bidBand <= serverBand && bidBand != 0 && serverBand > 0 &&
                        bidRam <= serverRam && bidRam != 0 && serverRam > 0 &&
                        bidCpu <= serverCpu && bidCpu != 0 && serverCpu > 0)
                    {

                        int amountBought = 0;
                        Console.WriteLine("Server band: " + serverBand);
                        Console.WriteLine("Server ram: " + serverRam);
                        Console.WriteLine("Server cpu: " + serverCpu);

                        //server bandwidth handling, change server stats to reflect change in quantities
                        int newBand = serverBand - bidBand;
                        Ask currentServer = SortedAsks[j];
                        currentServer.SetBand(SortedAsks[j].AskBand - SortedBids[i].BidBand);
                        SortedAsks[j] = currentServer;
                        serverBand = SortedAsks[j].AskBand;
                        Console.WriteLine("Bandwidth sold: " + bidBand);
                        Console.WriteLine("Server band left: " + serverBand);

                        //server ram, change server stats to reflect change in quantities
                        int newRam = serverRam - bidRam;
                        currentServer.SetRam(SortedAsks[j].AskRam - SortedBids[i].BidRam);
                        SortedAsks[j] = currentServer;
                        serverRam = SortedAsks[j].AskRam;
                        Console.WriteLine("RAM sold: " + bidRam);
                        Console.WriteLine("Server RAM left: " + serverRam);

                        //server cpu, change server stats to reflect change in quantities
                        int newcpu = serverCpu - bidCpu;
                        currentServer.SetCpu(SortedAsks[j].AskCpu - SortedBids[i].BidCpu);
                        SortedAsks[j] = currentServer;
                        serverCpu = SortedAsks[j].AskCpu;
                        Console.WriteLine("Cpu sold: " + bidCpu);
                        Console.WriteLine("Server CPU left: " + serverCpu);

                        //quantity for price calculation
                        amountBought = bidBand + bidRam + bidCpu;
                        Console.WriteLine("Total amount of units bought: " + amountBought);

                        //seller profit
                        int sellerProfit = winningPrice - SortedAsks[j].AskValue;
                        Settings.profits.Add(sellerProfit);

                        //winner and statistics of auction
                        Console.WriteLine($"[auctioneer]: Auction finished. Sold BandWidth: {SortedBids[i].BidBand} unit(s), RAM: {SortedBids[i].BidRam} unit(s) and CPU {SortedBids[i].BidCpu} unit(s) " +
                                            $"to {SortedBids[i].Bidder} from server {SortedAsks[j].Server} for total price  of {winningPrice * amountBought}.\n");
                        winners.Add(SortedBids[i].Bidder);
                        winnerCounter++;
                        
                        //handling current bidder: resetting info to reflect items were baught, thus they don't need more
                        //(effectively this will remove them from further runs in the auction)
                        Bid currentBid = SortedBids[i];
                        currentBid.SetBand(0);
                        currentBid.SetRam(0);
                        currentBid.SetCpu(0);
                        SortedBids[i] = currentBid;
                        //tell winners the results, no broadcasting for every agent just the winners
                        Send(SortedBids[i].Bidder, $"winner {SortedBids[i].Bidder}");
                        Settings.noMessages++;
                    }
                }
            }

            //send message to losers about their penalty price
            List<string> allBidders = new List<string>();
            for (int i = 0; i < SortedBids.Count; i++)
            {
                if (!winners.Contains(SortedBids[i].Bidder))
                {
                    string penaltyPrice = Convert.ToString(2 * winningPrice);
                    Send(SortedBids[i].Bidder, $"loser {SortedBids[i].Bidder} {penaltyPrice}");
                    Settings.noMessages++;
                }   
            }

            
            //list out winners
            if (winnerCounter != 0)
            {
                for (int i = 0; i < winners.Count; i++)
                {
                    Console.WriteLine($"Winner: {winners[i]}");
                }
            }
            else
            {
                //no winner handling
                Console.WriteLine("[auctioneer]: Auction finished. No winner.");
            }

            //stop servers
            for (int i = 0; i < SortedAsks.Count; i++)
            {
                Send(SortedAsks[i].Server, $"stop");
                Settings.noMessages++;
            }
            Settings.rate = (double)winnerCounter / (double)SortedBids.Count;
            Console.WriteLine(winnerCounter);
            Console.WriteLine(SortedBids.Count);

            //statistics for evaluation
            Settings.successRates.Add(Settings.rate);
            Console.WriteLine("Rate of successful trades: " + Settings.rate * 100 + "%");
            Stop();
        }
    }
}
