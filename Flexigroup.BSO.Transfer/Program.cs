using Flexigroup.BSO.Transfer.Models;
using Flexigroup.BSO.Transfer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Flexigroup.BSO.Transfer
{
    class Program
    {
        static byte[] SourcePrivateKey;
        static byte[] SourceIV;
        static byte[] DestinationPrivateKey;
        static byte[] DestinationIV;

        static void Main(string[] args)
        {
            var dir = Directory.GetCurrentDirectory();
#if (DEBUG)
            dir = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName).FullName;
#endif

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Flexigroup.BSO.Transfer.Program", LogLevel.Debug)
                    .AddConsole()
                    .AddEventLog();
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();

            var config = new ConfigurationBuilder()
                .SetBasePath(dir)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var SourcePrivateKeyPath = config["SourcePrivateKeyPath"];
            var SourceIVPath = config["SourceIVPath"];
            var DestinationPrivateKeyPath = config["DestinationPrivateKeyPath"];
            var DestinationIVPath = config["DestinationIVPath"];

            //if (!File.Exists(SourcePrivateKeyPath))
            //{
            //    AesCryptoServiceProvider myAes = new AesCryptoServiceProvider();
            //    File.WriteAllBytes(SourcePrivateKeyPath, myAes.Key);
            //    File.WriteAllBytes(SourceIVPath, myAes.IV);
            //    File.WriteAllBytes(DestinationPrivateKeyPath, myAes.Key);
            //    File.WriteAllBytes(DestinationIVPath, myAes.IV);
            //}

            SourcePrivateKey = File.ReadAllBytes(config["SourcePrivateKeyPath"]);
            SourceIV = File.ReadAllBytes(config["SourceIVPath"]);
            DestinationPrivateKey = File.ReadAllBytes(config["DestinationPrivateKeyPath"]);
            DestinationIV = File.ReadAllBytes(config["DestinationIVPath"]);

            var db = new BSOContext(config["SourceConnectionString"]);
            var dbDest = new BSOContext(config["DestinationConnectionString"]);

            //if (args != null && args.Contains("LoadSampleData"))
            //{
            //    string folder = config["SampleDataFolder"];
            //    LoadSampleData(folder, db);
            //    return;
            //}

            long LastID = dbDest.BsoDatas.Count();
            if (LastID > 0) LastID = dbDest.BsoDatas.Max(r => r.Id);

            var statements = db.BsoDatas.Where(r => r.Id > LastID).ToList();
            foreach (var statement in statements)
            {
                logger.LogInformation($"Statement {statement.Id} payload is {statement.Json.Length} bytes");
                var decrypted = AESEncryption.DecryptStringFromBytes_Aes(statement.JsonEncrypted, SourcePrivateKey, SourceIV);
                var encrypted = AESEncryption.EncryptStringToBytes_Aes(decrypted, DestinationPrivateKey, DestinationIV);
                var dest = new BsoData
                {
                    Id = statement.Id,
                    Json = statement.Json,
                    JsonIV = DestinationIV, 
                    JsonEncrypted = encrypted,
                    CorrelationId = statement.CorrelationId,
                    Created = statement.Created,
                    Received = statement.Received
                };
                dbDest.BsoDatas.Add(dest);
                dbDest.SaveChanges();
            }
        }

        static void LoadSampleData(string folder, BSOContext db)
        {
            foreach (var file in Directory.GetFiles(folder,"*.json"))
            {
                var json = File.ReadAllText(file);
                var encrypted = AESEncryption.EncryptStringToBytes_Aes(json, SourcePrivateKey, SourceIV);

                var bs = new BsoData
                {
                    Json = json,
                    JsonIV = SourceIV,
                    JsonEncrypted = encrypted,
                    CorrelationId = Guid.NewGuid(),
                    Created = DateTime.Now,
                    Received = DateTime.Now
                };
                db.BsoDatas.Add(bs);
                db.SaveChanges();
            }
        }
    }
}
