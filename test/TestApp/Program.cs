using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.Service;
using ProtoBuf.Grpc.Client;
using Service.ClientProfile.Client;
using Service.ClientProfile.Domain.Models;
using Service.ClientProfile.Grpc.Models;
using Service.ClientProfile.Grpc.Models.Requests;
using Service.ClientProfile.Postgres;
using Service.PersonalData.Client;
using Service.PersonalData.Grpc.Contracts;
using Service.PersonalData.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();

            await SetMarketingAllowedToAll();

            Console.WriteLine("End");
            Console.ReadLine();
        }


        private static async Task SetMarketingAllowedToAll()
        {
            //var cp = new ClientProfileClientFactory("http://clientprofile.spot-services.svc.cluster.local:80", null).GetClientProfileService();
            var cp = new ClientProfileClientFactory("http://localhost:80", null).GetClientProfileService();

            int take = 100;
            int skip = 0;
            var profiles = new List<ClientProfile>();
            do
            {
                var count = 0;
                var response = await cp.GetAllProfilesPaged(new GetAllRequest()
                {
                    Take = take,
                    Skip = skip
                });
                profiles = response.ClientProfiles?.ToList() ?? new List<ClientProfile>();
                skip += take;

                foreach (var profile in profiles)
                {
                    if(profile.MarketingEmailAllowed)
                        continue;
                    
                    var resp = await cp.SetMarketingEmailSettings(new SetMarketingEmailSettingsRequest()
                        {ClientId = profile.ClientId, IsAllowed = true});
                    Console.WriteLine(resp.ToJson());

                    if (!resp.IsSuccess)
                    {
                        Thread.Sleep(10000);
                        resp = await cp.SetMarketingEmailSettings(new SetMarketingEmailSettingsRequest()
                            {ClientId = profile.ClientId, IsAllowed = true});
                        Console.WriteLine(resp.ToJson());
                    }

                    count++;
                }
                Console.WriteLine($"Skip {skip}. Set {count}/{take}");
            } while (profiles.Any());

        }
    }
}
