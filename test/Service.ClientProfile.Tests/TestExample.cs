using System;
using NUnit.Framework;

namespace Service.ClientProfile.Tests
{
    public class TestExample
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var str = Guid.NewGuid().ToString("N").Replace("-", "").ToUpper();
            str = "SP-BrokerFee";
            for (int i = 0; i < str.Length-6; i++)
            {
                Console.WriteLine($"{i}: {str.Substring(i, 6)}");
            }
            Assert.Pass();
        }
    }
}
