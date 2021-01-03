using System;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ItemDBEditor.Data {

    public static class Extensions {
        public static void AddWithValueOrNull(this SqliteCommand cmd, string key, object value) {
            if(value == null) {
                cmd.Parameters.AddWithValue(key, DBNull.Value);
            }else {
                cmd.Parameters.AddWithValue(key, value);
            }
        }
    }

    public class Item {

        public string Code { get; set; }
        public string Name { get; set; }
        public string Prefab { get; set; }
        public double Weight { get; set; }
        public long Grist { get; set; }
        public string Strifekind { get; set; }
        public string Weaponsprite { get; set; }
        public bool CustomMade { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public long Speed { get; set; }
        public bool Spawn { get; set; }

        public Item(string code, string name, string prefab, double weight, long grist, string strifekind, string weaponsprite, bool customMade, string icon, string description, List<string> tags, long speed, bool spawn) {
            Code = code;
            Name = name;
            Prefab = prefab;
            Weight = weight;
            Grist = grist;
            Strifekind = strifekind;
            Weaponsprite = weaponsprite;
            CustomMade = customMade;
            Icon = icon;
            Description = description;
            Tags = tags;
            Speed = speed;
            Spawn = spawn;
        }

        public Item() {
            Tags = new List<string>();
        }
    }

    public class ItemService
    {
        private string dbPath = "items.db";

        public Task DeleteItem(string code) {
            using (var connection = new SqliteConnection($"Data Source={dbPath}")) {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM item_tags WHERE item = $code";
                cmd.AddWithValueOrNull("$code", code);
                int rows = cmd.ExecuteNonQuery();
                
                cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM items WHERE code = $code;";
                cmd.AddWithValueOrNull("$code", code);
                rows = cmd.ExecuteNonQuery();
            }
            return Task.CompletedTask;
        }

        public Task<bool> CreateItem(Item item) {
            var results = new List<int>();
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                using(var cmd = connection.CreateCommand()) {
                    cmd.CommandText = "INSERT INTO items VALUES ($code, $name, $prefab, $weight, $grist, $strifekind, $weaponsprite, $custommade, $icon, $description, $tags, $speed, $spawn);";
                    cmd.AddWithValueOrNull("$code", item.Code);
                    cmd.AddWithValueOrNull("$name", item.Name);
                    cmd.AddWithValueOrNull("$prefab", item.Prefab != null ? item.Prefab : "ItemObject");
                    cmd.AddWithValueOrNull("$weight", item.Weight);
                    cmd.AddWithValueOrNull("$grist", item.Grist);
                    cmd.AddWithValueOrNull("$strifekind", item.Strifekind);
                    cmd.AddWithValueOrNull("$weaponsprite", item.Weaponsprite);
                    cmd.AddWithValueOrNull("$custommade", item.CustomMade == true ? "y" : "n");
                    cmd.AddWithValueOrNull("$icon", item.Icon);
                    cmd.AddWithValueOrNull("$description", item.Description);
                    cmd.AddWithValueOrNull("$tags", "");
                    cmd.AddWithValueOrNull("$speed", item.Speed);
                    cmd.AddWithValueOrNull("$spawn", item.Spawn);

                    results.Add(cmd.ExecuteNonQuery());

                    using(var transaction = connection.BeginTransaction()) {
                        var cmdString = "INSERT INTO item_tags VALUES ($code, $tag);";
                        foreach(var tag in item.Tags) {
                            var tagCommand = connection.CreateCommand();
                            tagCommand.CommandText = cmdString;
                            tagCommand.AddWithValueOrNull("$code", item.Code);
                            tagCommand.AddWithValueOrNull("$tag", tag);
                            tagCommand.Transaction = transaction;
                            results.Add(tagCommand.ExecuteNonQuery());
                        }
                        transaction.Commit();
                    }
                }
            }
            return Task.FromResult(results.Sum() > 0);
        }

        public Task<bool> UpdateItem(Item item) {
            DeleteItem(item.Code);
            return CreateItem(item);
        }

        public Task<Item> GetItem(string code) {
            Item item = null;
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                
                List<string> tags = new List<string>();
                cmd.CommandText = "SELECT tag FROM item_tags WHERE item = $code;";
                cmd.Parameters.AddWithValue("$code", code);
                using(var reader = cmd.ExecuteReader()) {
                    while(reader.Read()) {
                        string tag = reader["tag"] as string;
                        tags.Add(tag);
                    }
                }

                cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT * FROM items WHERE code = $code;";
                cmd.Parameters.AddWithValue("$code", code);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["name"] as string;
                        string prefab = reader["prefab"] as string;
                        double weight = (double)reader["weight"];
                        long grist = (long)reader["grist"];
                        string strifekind = reader["strifekind"] as string;
                        string weaponsprite = reader["weaponsprite"] as string;
                        bool customMade = (reader["custommade"] as string) == "y";
                        string icon = reader["icon"] as string;
                        string description = reader["description"] as string;
                        long speed = (long)reader["speed"];
                        bool spawn = ((long)reader["spawn"]) == 1;
                        item = new Item(
                        code,
                        name,
                        prefab,
                        weight,
                        grist,
                        strifekind,
                        weaponsprite,
                        customMade,
                        icon,
                        description,
                        tags,
                        speed,
                        spawn);
                    }
                }
            }
            return Task.FromResult(item);
        }

        public Task<List<Item>> GetItems() {
            var items = new List<Item>();
            using(var connection = new SqliteConnection($"Data Source={dbPath}")) {
                connection.Open();
                var cmd = connection.CreateCommand();

                Dictionary<string, List<string>> tags = new Dictionary<string, List<string>>();
                cmd.CommandText = "SELECT * FROM item_tags;";
                using(var reader = cmd.ExecuteReader()) {
                    while(reader.Read()) {
                        string item = reader["item"] as string;
                        string tag = reader["tag"] as string;
                        
                        if(!tags.ContainsKey(item)) {
                            tags.Add(item, new List<string>());
                        }
                        var list = tags[item];
                        list.Add(tag);
                    }
                }

                cmd.CommandText = "SELECT * FROM items;";
                using(var reader = cmd.ExecuteReader()) {
                    while(reader.Read()) {
                        string code = reader["code"] as string;
                        string name = reader["name"] as string;
                        string prefab = reader["prefab"] as string;
                        double weight = (double)reader["weight"];
                        long grist = (long)reader["grist"];
                        string strifekind = reader["strifekind"] as string;
                        string weaponsprite = reader["weaponsprite"] as string;
                        bool customMade = (reader["custommade"] as string) == "y";
                        string icon = reader["icon"] as string;
                        string description = reader["description"] as string;
                        if(!tags.ContainsKey(code)) tags.Add(code, new List<string>());
                        List<string> itemTags = tags[code];
                        long speed = (long)reader["speed"];
                        bool spawn = ((long)reader["spawn"]) == 1;

                        items.Add(new Item(
                        code, 
                        name, 
                        prefab, 
                        weight, 
                        grist,
                        strifekind,
                        weaponsprite,
                        customMade,
                        icon,
                        description,
                        itemTags,
                        speed,
                        spawn));
                    }
                }
            }
            return Task.FromResult(items);
        }
    }
}