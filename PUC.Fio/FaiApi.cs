using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using PUC.Fio.Model;
using PUC.Fio.Model.Buildings;
using PUC.Fio.Model.Inventory;
using PUC.Fio.Model.Materials;
using PUC.Fio.Model.Planets;
using PUC.Fio.Model.Prices;
using PUC.Fio.Model.Recipes;

namespace PUC.Fio
{
    public class FaiApi
    {
        private string Token { get; set; }
        private DateTime TokenTTL { get; set; }

        public FioApiConfig Config { get; set; }

        public FaiApi(FioApiConfig cfg)
        {
            Config = cfg;

            if (!Directory.Exists("cache"))
            {
                Directory.CreateDirectory("cache");
            }
        }
        
        public void Auth()
        {
            if (string.IsNullOrEmpty(Config.Password)) return;

            if (string.IsNullOrEmpty(Token) || DateTime.Now >= TokenTTL)
            {
                var username = Config.Username;
                var password = Config.Password;

                using (var wc = new WebClient())
                {
                    wc.Headers.Add("accept", "application/json");
                    wc.Headers.Add("Content-Type", "application/json");

                    var resp = JsonConvert.DeserializeObject<AuthReq>(
                        wc.UploadString("https://rest.fnar.net/auth/login",
                            $"{{\n  \"UserName\": \"{username}\",\n  \"Password\": \"{password}\"\n}}"
                        )
                    );

                    Token = resp.AuthToken;
                    TokenTTL = resp.Expiry;
                }
            }
        }

     


        public List<PlanetNameEntry> GetPlanetNames()
        {
            Auth();

            using (var wc = new WebClient())
            {
                wc.Headers.Add("accept", "application/json");

                return JsonConvert.DeserializeObject<List<PlanetNameEntry>>(
                    wc.DownloadString("https://rest.fnar.net/planet/allplanets")
                );
            }
        }

        public List<Planet> GetPlanets()
        {
            Auth();

            var cachePath = Path.Combine("./cache", "planets.json");

            if (File.Exists(cachePath))
            {
                return JsonConvert.DeserializeObject<List<Planet>>(File.ReadAllText(cachePath));
            }

            var re = new List<Planet>();

            var names = GetPlanetNames();

            for (var i = 0; i < names.Count; i++)
            {
                var entry = names[i];
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("accept", "application/json");
                    wc.Headers.Add("Authorization", Token);


                    var planet = JsonConvert.DeserializeObject<Planet>(
                        wc.DownloadString("https://rest.fnar.net/planet/" + entry.PlanetName));

                    Console.WriteLine($"Downloading Planet {i} of {names.Count}");

                    re.Add(planet);
                }
            }

            File.WriteAllText(cachePath, JsonConvert.SerializeObject(names));

            return re;
        }

        public List<Recipe> GetRecipies()
        {
            Auth();

            var cachePath = Path.Combine("./cache", "recipes.json");

            if (File.Exists(cachePath))
            {
                return JsonConvert.DeserializeObject<List<Recipe>>(File.ReadAllText(cachePath));
            }

            using (var wc = new WebClient())
            {
                wc.Headers.Add("accept", "application/json");
                wc.Headers.Add("Authorization", Token);

                var recipes = JsonConvert.DeserializeObject<List<Recipe>>(
                    wc.DownloadString("https://rest.fnar.net/recipes/allrecipes"));

                File.WriteAllText(cachePath, JsonConvert.SerializeObject(recipes));
                return recipes;
            }
        }

        private List<PriceEntry> _cache = new List<PriceEntry>();

        public List<PriceEntry> GetPrices()
        {
            if (_cache.Count > 0) return _cache;
            Auth();

            var cachePath = Path.Combine("./cache", "prices.json");

            var fi = new FileInfo(cachePath);

            var dt = DateTime.Now - fi.CreationTime;

            if (fi.Exists && dt.Hours <= 1)
            {
                return JsonConvert.DeserializeObject<List<PriceEntry>>(File.ReadAllText(cachePath));
            }

            using (var wc = new WebClient())
            {
                wc.Headers.Add("accept", "application/json");
                wc.Headers.Add("Authorization", Token);

                var data = wc.DownloadString("https://rest.fnar.net/rain/prices");

                var prices = JsonConvert.DeserializeObject<List<PriceEntry>>(data,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});

                File.WriteAllText(cachePath, JsonConvert.SerializeObject(prices));
                _cache = prices;
                return prices;
            }
        }

        public List<Building> GetBuildings()
        {
            Auth();

            var cachePath = Path.Combine("./cache", "building.json");

            var fi = new FileInfo(cachePath);
            var dt = DateTime.Now - fi.CreationTime;

            if (fi.Exists && dt.Minutes <= Config.CacheTimeOut)
            {
                return JsonConvert.DeserializeObject<List<Building>>(File.ReadAllText(cachePath));
            }

            using (var wc = new WebClient())
            {
                wc.Headers.Add("accept", "application/json");
                wc.Headers.Add("Authorization", Token);

                var data = wc.DownloadString("https://rest.fnar.net/building/allbuildings");

                var buildings = JsonConvert.DeserializeObject<List<Building>>(data,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});

                File.WriteAllText(cachePath, JsonConvert.SerializeObject(buildings));
                return buildings;
            }
        }

        public List<Material> GetMaterials()
        {
            Auth();

            var cachePath = Path.Combine("./cache", "materials.json");

            var fi = new FileInfo(cachePath);
            var dt = DateTime.Now - fi.CreationTime;

            if (fi.Exists && dt.Minutes <= Config.CacheTimeOut)
            {
                return JsonConvert.DeserializeObject<List<Material>>(File.ReadAllText(cachePath));
            }

            using (var wc = new WebClient())
            {
                wc.Headers.Add("accept", "application/json");
                wc.Headers.Add("Authorization", Token);

                var data = wc.DownloadString("https://rest.fnar.net/material/allmaterials");

                var materials = JsonConvert.DeserializeObject<List<Material>>(data,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});

                File.WriteAllText(cachePath, JsonConvert.SerializeObject(materials));
                return materials;
            }
        }

        public List<Storage> GetStorage()
        {
            Auth();

            var cachePath = Path.Combine("./cache", "storage.json");

            var fi = new FileInfo(cachePath);

            var dt = DateTime.Now - fi.CreationTime;

            if (fi.Exists && dt.Minutes <= Config.CacheTimeOut)
            {
                return JsonConvert.DeserializeObject<List<Storage>>(File.ReadAllText(cachePath));
            }

            using (var wc = new WebClient())
            {
                wc.Headers.Add("accept", "application/json");
                wc.Headers.Add("Authorization", Token);

                var data = wc.DownloadString("https://rest.fnar.net/storage/myvar");

                var storage = JsonConvert.DeserializeObject<List<Storage>>(data,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});

                File.WriteAllText(cachePath, JsonConvert.SerializeObject(storage));
                return storage;
            }
        }
    }
}