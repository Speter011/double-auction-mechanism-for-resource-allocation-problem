using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActressMas;

namespace Coursework_MAS
{
    class ServerAgent : Agent
    {
        private int _ask;
        private int _bandwidth;
        private int _RAM;
        private int _CPU;

        public ServerAgent(int ask, int band, int ram, int cpu)
        {
            //price per one unit! all units are assumed to be worth the same
             _ask = ask;
            _bandwidth = band;
            _RAM = ram;
            _CPU = cpu;
        }


        public override void Setup()
        {
            Console.WriteLine($"[{Name}]: My cost, bandwidth, ram, and cpu capacity are: {_ask} {_bandwidth} {_RAM} {_CPU}");
        }

        //act: send info, stop
        public override void Act(Message message)
        {
            try
            {
                Console.WriteLine($"\t{message.Format()}");
                message.Parse(out string action, out string parameters);

                switch (action)
                {
                    case "start":
                        HandleStart();
                        break;

                    case "stop":
                        HandleFinish();
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
            Send("auctioneer", $"ask {_ask} {_bandwidth} {_RAM} {_CPU}");
            Settings.noMessages++;
        }
        private void HandleFinish()
        {
            Stop();
        }
    }
}
