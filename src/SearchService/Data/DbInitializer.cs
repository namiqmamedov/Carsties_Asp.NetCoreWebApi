using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data
{
    public class DbInitializer
    { 
        public static async Task InitDb(WebApplication app)
        {
            await DB.InitAsync("SearchDb", MongoClientSettings
                .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

            await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();
            
            var count = await DB.CountAsync<Item>();

            if(count == 0)
            {
                Console.WriteLine("No data - will attempty to seed");
                var itemData = await File.ReadAllTextAsync("Data/auctions.json");

                var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

                var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

                await DB.SaveAsync(items);
            }
        }
    }
}