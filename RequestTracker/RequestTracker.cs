using System;
using System.Collections.Generic;
using System.Linq;

namespace RequestTracker
{
    public class RequestTracker
    {
        private Dictionary<string, int> requests;

        public RequestTracker()
        {
            requests = new Dictionary<string, int>();
        }
        
        /// <summary>
        /// Adds the IP Address to the stored collection or increments the count of requests related to an IP.
        /// </summary>
        /// <param name="ipAddress">IP Address to be logged</param>
        public void RequestHandled(string ipAddress)
        {
            if (!requests.ContainsKey(ipAddress))
            {
                requests.Add(ipAddress, 1);
            }
            else
            {
                requests[ipAddress]++;
            }
        }

        /// <summary>
        /// Sorts the collection and returns the 100 IP addresses with the most visits.
        /// </summary>
        /// <returns>Returns a dictionary containing the IP Adresses and the times that IP address was used.</returns>
        public Dictionary<string, int> Top100()
        {
            SortRequests();
            return requests.Take(100).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Removes all entries from the tracker.
        /// </summary>
        public void Clear()
        {
            requests.Clear();
        }

        /// <summary>
        /// Sorts the collection by highest value (count), efficiency is nLogn
        /// </summary>
        private void SortRequests()
        {
            requests = requests.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
