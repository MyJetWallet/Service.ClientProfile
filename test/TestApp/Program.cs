using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.ClientProfile.Client;
using Service.ClientProfile.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();

            Console.WriteLine(Guid.NewGuid().ToString().ToUpper().Replace("-",""));
            Console.WriteLine(Guid.NewGuid().ToString().ToUpper().Replace("-","").Length);

            var clientId = Guid.NewGuid().ToString().ToUpper().Replace("-", "");
            Console.WriteLine(clientId);
            Console.WriteLine(clientId.Substring(0,6));
            Console.WriteLine(clientId.Substring(6,6));
            Console.WriteLine(clientId.Substring(12,6));
            Console.WriteLine(clientId.Substring(18,6));
            Console.WriteLine(clientId.Substring(24,6));
            
            Console.WriteLine();
            
            Console.WriteLine(Guid.NewGuid().ToString("N").ToUpper().Replace("-","").Substring(0,6));
            Console.WriteLine(Guid.NewGuid().ToString("N").ToUpper().Replace("-","").Substring(0,6));
            Console.WriteLine(Guid.NewGuid().ToString("N").ToUpper().Replace("-","").Substring(0,6));
            Console.WriteLine(Guid.NewGuid().ToString("N").ToUpper().Replace("-","").Substring(0,6));
            Console.WriteLine(Guid.NewGuid().ToString("N").ToUpper().Replace("-","").Substring(0,6));

            // var factory = new ClientProfileClientFactory("http://localhost:5001");
            // var client = factory.GetHelloService();
            //
            // var resp = await  client.AddBlockerToClient(new HelloRequest(){Name = "Alex"});
            // Console.WriteLine(resp?.Message);

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
