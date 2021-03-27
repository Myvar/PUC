using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using PUC.Fio;
using PUC.Fio.Model.Buildings;

namespace PUC.TechTree
{
    public class TechTreeWebServer : Nancy.NancyModule
    {
        private static FioApiConfig _cfg = new FioApiConfig();
        public FaiApi Api = new FaiApi(_cfg);

        public TechTreeWebServer()
        {
            Get("/{ticker}", Serve);
            Get("/", ServeHome);
            Get("/DragZoomingTool.js",
                _ => Response.AsText(Utils.GetResourceFile("DragZoomingTool.js"), "application/javascript"));
        }

        public object ServeHome(dynamic args)
        {
            /*
             *  <tr>
            <th scope="row">1</th>
            <td>POL</td>
            <td>polplant</td>
            <td>OPEN</td>
        </tr>
             */

            var res = Utils.GetResourceFile("index.html");

            var sb = new StringBuilder();
            var buildings = Api.GetBuildings();
            int offset = 1;
            foreach (var building in buildings)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<th scope=\"row\">{offset++}</th>");
                sb.AppendLine($"<td>{building.Name}</td>");
                sb.AppendLine($"<td>{building.Ticker}</td>");
                sb.AppendLine($"<td><a href=\"/{building.Ticker}\">Open</a></td>");
                sb.AppendLine("</tr>");
            }


            res = res.Replace("//{{ROWS}}}", sb.ToString());

            return Response.AsText(res, "text/html");
        }

        public object Serve(dynamic args)
        {
            if (string.IsNullOrEmpty(args.ticker))
            {
                return Response.AsText(GetHtml(), "text/html");
            }

            return Response.AsText(GetHtml((string) args.ticker), "text/html");
        }


        private class GraphNode
        {
            public string Text { get; set; }
            public string Color { get; set; }
            public string LinkColor { get; set; } = "white";

            public List<GraphNode> Prev { get; set; } = new List<GraphNode>();
            public GraphNode Next { get; set; }
        }

        private List<GraphNode> _usedNodes = new List<GraphNode>();


        private void IterateBuildings(List<Building> buildings, GraphNode root, Building building)
        {
            var prices = Api.GetPrices();
            var mats = Api.GetMaterials();

            _usedNodes.Add(root);

            foreach (var cost in building.BuildingCosts)
            {
                if (_usedNodes.Any(x => x.Text == cost.CommodityTicker))
                {
                    root.Prev.Add(_usedNodes.First(x => x.Text == cost.CommodityTicker));
                }
                else
                {
                    var node = new GraphNode();
                    node.Text = cost.CommodityTicker;
                    node.Color = "gray";

                    root.Prev.Add(node);


                    var price = prices.First(x => x.Ticker == cost.CommodityTicker);

                    if (price.MMSell != 0)
                    {
                        var buildNode = new GraphNode();
                        buildNode.Text = $"MM({cost.CommodityTicker})";
                        buildNode.Color = "green";
                        buildNode.LinkColor = "cornflowerblue";
                        node.Prev.Add(buildNode);
                    }

                    foreach (var building1 in buildings)
                    {
                        if (building1.Recipes.Any(x =>
                                x.Outputs.Any(o => o.CommodityTicker == cost.CommodityTicker)
                            //|| x.Inputs.Any(o => o.CommodityTicker == cost.CommodityTicker )
                        ))
                        {
                            if (_usedNodes.Any(x => x.Text == building1.Ticker))
                            {
                                node.Prev.Add(_usedNodes.First(x => x.Text == building1.Ticker));
                            }
                            else
                            {
                                var buildNode = new GraphNode();
                                buildNode.Text = building1.Ticker;
                                buildNode.Color = "cornflowerblue";
                                buildNode.LinkColor = "cornflowerblue";
                                node.Prev.Add(buildNode);


                                IterateBuildings(buildings, buildNode, building1);
                            }
                        }
                    }
                }
            }
        }

        private int _nodeCounter = 0;

        private List<(int offset, string name)> _nodeIdx = new List<(int offset, string name)>();

        private void IterateRenderNodeNode(StringBuilder nodes, GraphNode root)
        {
            var index = _nodeCounter++;
            nodes.AppendLine($"{{ key: {index}, text: \"{root.Text}\", color: \"{root.Color}\" }},");
            _nodeIdx.Add((index, root.Text));
            foreach (var node in root.Prev)
            {
                if (_nodeIdx.Any(x => x.name == node.Text)) continue;
                IterateRenderNodeNode(nodes, node);
            }
        }

        private List<(int from, int to)> _links = new List<(int from, int to)>();

        private void IterateRenderNodeLinks(StringBuilder links, GraphNode root)
        {
            var from = _nodeIdx.First(x => x.name == root.Text).offset;
            foreach (var node in root.Prev)
            {
                var to = _nodeIdx.First(x => x.name == node.Text).offset;

                if (_links.Any(x => x.from == from && x.to == to)) continue;


                links.AppendLine($"{{ from: {to}, to: {from}, text: \"\", color: \"{node.LinkColor}\" }},");
                _links.Add((from, to));
                IterateRenderNodeLinks(links, node);
            }
        }

        private string GetHtml(string ticker)
        {
            var template = Utils.GetResourceFile("prodtree-template.html");

            var buildings = Api.GetBuildings();
            var building = buildings.First(x => x.Ticker == ticker);

            var root = new GraphNode();
            root.Text = building.Ticker;
            root.Color = "cornflowerblue";
            _usedNodes.Clear();

            IterateBuildings(buildings, root, building);

            var nodes = new StringBuilder();
            var links = new StringBuilder();

            _nodeCounter = 0;
            _nodeIdx.Clear();
            IterateRenderNodeNode(nodes, root);
            _links.Clear();
            IterateRenderNodeLinks(links, root);

            template = template.Replace("//{{NODES}}", nodes.ToString());
            template = template.Replace("//{{LINKS}}", links.ToString());


            return template;
        }

        private string GetHtml()
        {
            var template = Utils.GetResourceFile("prodtree-template.html");
            int key = 1;

            var recpies = Api.GetRecipies();
            var buildings = Api.GetBuildings();
            var prices = Api.GetPrices();


            var sb = new StringBuilder();
            var sb2 = new StringBuilder();
            var lst = new List<object>();
            lst.Add(0);


            foreach (var price in prices)
            {
                if (price.CI1AskAmt == 0 && price.IC1AskAmt == 0 && price.NI1AskAmt == 0 &&
                    //price.CI1BidAmt == 0 && price.IC1BidAmt == 0 && price.NI1BidAmt == 0 &&
                    // price.CI1Average == 0 && price.IC1Average == 0 && price.NI1Average == 0 &&
                    price.MMBuy == 0 && price.MMSell == 0)
                {
                    continue;
                }

                sb.AppendLine($"{{ key: {key++}, text: \"{price.Ticker}\", color: \"grey\" }},");
                lst.Add(price.Ticker);
            }

            foreach (var building in buildings)
            {
                var flag = true;
                foreach (var recipe in building.Recipes)
                {
                    foreach (var output in recipe.Outputs)
                    {
                        if (!lst.Contains(output.CommodityTicker))
                        {
                            if (flag) flag = false;
                            break;
                        }
                    }

                    foreach (var intput in recipe.Inputs)
                    {
                        if (!lst.Contains(intput.CommodityTicker))
                        {
                            if (flag) flag = false;
                            break;
                        }
                    }

                    if (!flag) break;
                }

                if (flag) continue;

                sb.AppendLine(
                    $"{{ key: {key++}, text: \"{building.Name} [{building.Ticker}]\", color: \"cornflowerblue\" }},");
                lst.Add(building.Ticker);
            }

            var dups = new List<(int, int)>();

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                foreach (var recipe in recpies)
                {
                    if (!lst.Contains(recipe.BuildingTicker)) continue;
                    foreach (var input in recipe.Inputs)
                    {
                        if (!lst.Contains(input.Ticker)) continue;

                        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input.Ticker);
                        byte[] hashBytes = md5.ComputeHash(inputBytes);

                        hashBytes[0] = Math.Clamp(hashBytes[0], (byte) 0, (byte) 200);
                        hashBytes[1] = Math.Clamp(hashBytes[1], (byte) 0, (byte) 200);
                        hashBytes[2] = Math.Clamp(hashBytes[2], (byte) 0, (byte) 50);

                        var c = Color.FromArgb(hashBytes[0], hashBytes[1], hashBytes[2]);

                        var a = lst.IndexOf(input.Ticker);
                        var b = lst.IndexOf(recipe.BuildingTicker);
                        if (!dups.Contains((a, b)))
                        {
                            sb2.AppendLine(
                                $"{{ from: {a}, to: {b}, " +
                                $"text: \"{input.Amount}\", color: \"rgb({c.R},{c.G},{c.B})\" }},");

                            dups.Add((a, b));
                        }
                    }

                    foreach (var output in recipe.Outputs)
                    {
                        if (!lst.Contains(output.Ticker)) continue;

                        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(output.Ticker);
                        byte[] hashBytes = md5.ComputeHash(inputBytes);

                        hashBytes[0] = Math.Clamp(hashBytes[0], (byte) 0, (byte) 200);
                        hashBytes[1] = Math.Clamp(hashBytes[1], (byte) 0, (byte) 200);
                        hashBytes[2] = Math.Clamp(hashBytes[2], (byte) 0, (byte) 50);

                        var c = Color.FromArgb(hashBytes[0], hashBytes[1], hashBytes[2]);
                        var a = lst.IndexOf(recipe.BuildingTicker);
                        var b = lst.IndexOf(output.Ticker);
                        if (!dups.Contains((a, b)))
                        {
                            dups.Add((a, b));
                            sb2.AppendLine(
                                $"{{ from: {a}, to: {b}, " +
                                $"text: \"{output.Amount}\", color: \"rgb({c.R},{c.G},{c.B})\" }},");
                        }
                    }
                }
            }

            template = template.Replace("//{{NODES}}", sb.ToString());
            template = template.Replace("//{{LINKS}}", sb2.ToString());

            return template;
        }
    }
}