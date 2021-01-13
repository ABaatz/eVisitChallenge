using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace RequetTrackerTests
{
    public class Tests
    {
        private RequestTracker.RequestTracker requestTracker;

        [SetUp]
        public void Setup()
        {
            requestTracker = new RequestTracker.RequestTracker();
        }

        [Test]
        public void RequestHandled_Ok()
        {
            var ip1 = "127.0.0.1";
            var ip2 = "127.0.0.2";
            var ip3 = "127.0.0.3";
            requestTracker.RequestHandled(ip1);
            requestTracker.RequestHandled(ip3);
            requestTracker.RequestHandled(ip2);
            requestTracker.RequestHandled(ip3);
            requestTracker.RequestHandled(ip2);
            requestTracker.RequestHandled(ip3);

            var requestsStored = requestTracker.Top100();

            Assert.NotNull(requestsStored);
            Assert.True(requestsStored.ContainsKey(ip1));
            Assert.True(requestsStored.ContainsKey(ip2));
            Assert.True(requestsStored.ContainsKey(ip3));

            Assert.AreEqual(requestsStored[ip1], 1);
            Assert.AreEqual(requestsStored[ip2], 2);
            Assert.AreEqual(requestsStored[ip3], 3);

            requestTracker.Clear();
        }

        [Test]
        public void Clear_Ok()
        {
            var ip1 = "127.0.0.1";
            var ip2 = "127.0.0.2";
            var ip3 = "127.0.0.3";
            requestTracker.RequestHandled(ip1);
            requestTracker.RequestHandled(ip2);
            requestTracker.RequestHandled(ip3);

            var unclearedCount = requestTracker.Top100()?.Count;
            requestTracker.Clear();
            var clearedCount = requestTracker.Top100()?.Count;

            Assert.NotNull(unclearedCount);
            Assert.NotNull(clearedCount);
            Assert.AreNotEqual(unclearedCount, 0);
            Assert.AreEqual(clearedCount, 0);
        }
        
        [Test]
        public void Clear_Empty_Ok()
        {
            requestTracker.Clear();
            var result = requestTracker.Top100();

            Assert.NotNull(result);
            Assert.AreEqual(result.Count, 0);
        }

        [DatapointSource]
        public int[] entryCounts = { 0, 10, 101, 1000, 10000, 1000000, 10000000, 100000000 };

        [Theory]
        public void Take100_Unique_Ok(int entryCount)
        {
            var baseIp = "127.0.0.";

            //Half of the time taken for this test in the higher entryCounts is due to the loop here.
            for (int i = 0; i < entryCount; i++)
            {
                requestTracker.RequestHandled(baseIp + i.ToString());
            }

            var result = requestTracker.Top100();

            Assert.NotNull(result);
            Assert.LessOrEqual(result.Count, 100);

            requestTracker.Clear();
        }
        
        [Theory]
        public void Take100_Duplicates_Ok(int entryCount)
        {
            var baseIp = "127.0.0.";
            var counter = 0;

            for (int i = 0; i < entryCount; i++)
            {
                if (counter == entryCount)
                {
                    break;
                }
                var duplicates = new Random().Next(1, 100);
                for (int j = 0; j < duplicates; j++)
                {
                    if (counter == entryCount)
                    {
                        break;
                    }
                    counter++;
                    requestTracker.RequestHandled(baseIp + i.ToString());
                }
            }

            var result = requestTracker.Top100();

            Assert.NotNull(result);
            Assert.LessOrEqual(result.Count, 100);

            requestTracker.Clear();
        }
    }
}