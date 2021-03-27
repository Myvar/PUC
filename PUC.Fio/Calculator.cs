using System;
using System.Collections.Generic;
using System.Linq;

namespace PUC.Fio
{
    public static class Calculator
    {
        /*
         * The Goal:
         *
         * Givin a range of starting capital and a range for production per day and give the short and long term and the ROI profit(calc burnrate, jump cost etc)
         */

        public static List<string> UsedBuildings { get; set; } = new List<string>();

        public static float GetAveragePrice(this FaiApi api, string ticker, bool avg = false)
        {
            var prices = api.GetPrices();

            var item = prices.First(x => x.Ticker == ticker);

            if (ticker == "LBH") return 0;
            if (item.CI1AskAmt == 0 && item.IC1AskAmt == 0 && item.NI1AskAmt == 0 && !avg) return 0;

            var items = new[] {item.CI1Average, item.IC1Average, item.NI1Average};

            return items.Average();
        }


        public static float GetOrAproxPrice(this FaiApi api, string ticker)
        {
            var debug = false;

            var price = api.GetAveragePrice(ticker);
            if (debug) Console.WriteLine($"Calculating price of {ticker}");
            if (price == 0)
            {
                if (debug) Console.WriteLine($"Cant find price of {ticker} on CX, using recipe to Approximate it");
                var recpies = api.GetRecipies();
                if (recpies.Any(x => x.Outputs.Any(z => z.Ticker == ticker)))
                {
                    var recipie = recpies.First(x => x.Outputs.Any(z => z.Ticker == ticker));

                    if (debug)
                        Console.WriteLine(
                            $"Found recipe for {ticker}: {recipie.RecipeName} in [{recipie.BuildingTicker}]");
                    if (!UsedBuildings.Contains(recipie.BuildingTicker))
                    {
                        UsedBuildings.Add(recipie.BuildingTicker);
                    }

                    var x = "";
                    foreach (var input in recipie.Inputs)
                    {
                        var p = api.GetOrAproxPrice(input.Ticker);
                        price += p * input.Amount;
                        x += $"({p} * {input.Amount}{input.Ticker}) + ";
                    }

                    if (debug) Console.WriteLine($"Calculated price for {ticker}: {price} = {x.Trim().TrimEnd('+')}");
                }
                else
                {
                    price = api.GetAveragePrice(ticker, true);
                    if (price == 0)
                    {
                        Console.WriteLine($"Cant find Price for {ticker}");
                    }
                }
            }
            else
            {
                if (debug) Console.WriteLine($"Found price for {ticker} on CX using average price of {price}");
            }


            return price;
        }
    }
}