using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework_MAS
{
    class Settings
    {
        //bidder
        public static int NoBidders = 50;
        public static int NoServers = 50;
        public static int MinPrice = 30;
        public static int MaxPrice = 50;

        public static int MinBand = 1;
        public static int MaxBand = 3;
        public static int MinRam = 1;
        public static int MaxRam = 3;
        public static int MinCpu = 1;
        public static int MaxCpu = 3;

        //server stats
        public static int MinAsk = 30;
        public static int MaxAsk = 50;

        public static int MinBandS = 3;
        public static int MaxBandS = 5;
        public static int MinRamS = 3;
        public static int MaxRamS = 5;
        public static int MinCpuS = 3;
        public static int MaxCpuS = 5;


        //public static int Increment = 10;
        //public static int ReservePrice = 100;
        public static int noRuns = 3;
        public static double rate {get; set;}
        public static int noMessages { get; set; }
        public static List<double> successRates = new List<double>();
        public static List<int> breakevens = new List<int>();
        public static List<int> profits = new List<int>();
        public static List<int> prices = new List<int>();
    }
}
