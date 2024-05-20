using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActressMas;

namespace Coursework_MAS
{
    public class BidderAgent : Agent
    {
        private int _valuation;
        private int _bandwidth;
        private int _RAM;
        private int _CPU;

        public BidderAgent(int val, int band, int ram, int cpu)
        {
            _valuation = val;
            _bandwidth = band;
            _RAM = ram;
            _CPU = cpu;
        }

        public override void Setup()
        {
            Console.WriteLine($"[{Name}]: My valuation and requirements are {_valuation} {_bandwidth} {_RAM} {_CPU}");
        }

        //act: initiation, winning auction, losing auction
        public override void Act(Message message)
        {
            try
            {
                Console.WriteLine($"\t{message.Format()}");
                message.Parse(out string action, out List<string> parameters);

                switch (action)
                {
                    case "start":
                        HandleStart();
                        break;

                    case "winner":
                        HandleWinner(parameters[0]);
                        break;

                    case "loser":
                        HandleLoser(parameters[0], parameters[1]);
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

        private void HandleStart()
        {
            //send info
            Send("auctioneer", $"bid {_valuation} {_bandwidth} {_RAM} {_CPU}");
            Settings.noMessages++;
        }

        private void HandleWinner(string winner)
        {
            if (winner == Name)
                Console.WriteLine($"[{Name}]: I have won.");

            Stop();
        }
        private void HandleLoser(string loser, string failurePrice)
        {
            //total price due to penalty is claculated
            int price = Convert.ToInt32(failurePrice);
            int total = price * (_bandwidth + _CPU + _RAM);
            if (loser == Name)
                Console.WriteLine($"[{Name}]: I have lost, my payment is: {total}");

            Stop();
        }
    }
}
