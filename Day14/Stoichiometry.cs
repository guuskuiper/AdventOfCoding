using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day14
{
    public class Stoichiometry
    {
        private Dictionary<string, long> stock;
        private Dictionary<string, (int, List<(string, int)>)> production;

        public Stoichiometry(IEnumerable<string> text)
        {
            stock = new Dictionary<string, long>();
            production = new Dictionary<string, (int, List<(string, int)>)>();

            foreach(var line in text)
            {
                var inputoutput = line.Split("=>");
                var inputStr = inputoutput[0];
                var outputStr = inputoutput[1];

                var outputDesc = outputStr.Trim().Split(' ');
                var outputQuantity = int.Parse(outputDesc[0]);
                var outputName = outputDesc[1];
                AddIfNew(outputName);

                
                var inputData = new List<(string, int)>();

                var inputs = inputStr.IndexOf(',') >= 0 ? inputStr.Split(',') : new [] {inputStr};
                foreach(var input in inputs)
                {
                    var inputDesc = input.Trim().Split(' ');
                    var inputQuantity = int.Parse(inputDesc[0]);
                    var inputName = inputDesc[1];

                    AddIfNew(inputName);

                    inputData.Add( (inputName, inputQuantity) );

                }

                production.Add(outputName, (outputQuantity, inputData));
            }
        }

        public long Solve(int number = 1)
        {
            stock["FUEL"] = -number;

            bool produced = true;
            while(produced)
            {
                produced = false;
                foreach(var stockitem in stock)
                {
                    if(stockitem.Key == "ORE") continue;
                    if(stockitem.Value < 0)
                    {
                        produced = true;
                        Produce(stockitem.Key, -(int)stockitem.Value);
                        break;
                    }
                }
            }

            return -stock["ORE"];
        }

        public int ConsumeOre(long amout)
        {
            var fuel = 0;

            // by experiment, first the large digits
            for(int i = 0; i < 785; i++)
            {
                var initialBatch = 10000;
                Solve(initialBatch);
                fuel += initialBatch;
            }

            System.Console.WriteLine($"ORE used after initial: {-stock["ORE"]}");

            int batchCount = 0;

            while(true)
            {
                var batch = 1;
                Solve(batch);

                if(-stock["ORE"] > amout)
                {
                    System.Console.WriteLine($"ORE used: {-stock["ORE"]} batch {batchCount}");
                    return fuel;
                }

                fuel+=batch;
                batchCount++;

                //System.Console.WriteLine($"Fuel {fuel} ore {-stock["ORE"]}");
            }

            return fuel;
        }

        private void Produce(string name, int minimumQuntity)
        {
            var res = production[name];
            var outputQuantity = res.Item1;
            var inputs = res.Item2;

            var productionTimes = (int)Math.Ceiling((double)minimumQuntity / outputQuantity);

            stock[name] += productionTimes * outputQuantity;

            foreach(var input in inputs)
            {
                var inputName = input.Item1;
                var inputQuantity = input.Item2;

                stock[inputName] -= productionTimes * inputQuantity;
            }
        }

        private void AddIfNew(string name)
        {
            if(!stock.ContainsKey(name))
            {
                stock.Add(name, 0);
            }
        }
    }
}