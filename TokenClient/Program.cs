using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TokenAndSignature.Models;
using TokenClient.Common;

namespace TokenClient
{
    class Program
    {
        static void Main(string[] args)
        {



            int staffId = 1;
            var tokenResult = WebApiHelper.GetSignToken(staffId);
            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("id", "1");
            parames.Add("name", "wahaha");
            Tuple<string, string> parameters = WebApiHelper.GetQueryString(parames);
            var product1 = WebApiHelper.Get<ProductResultMsg>("http://localhost:5000/api/product/getproduct", parameters.Item1, parameters.Item2, staffId);
            Product product = new Product() { Id = 1, Name = "安慕希", Count = 10, Price = 58.8 };
            var product2 = WebApiHelper.Post<ProductResultMsg>("http://localhost:5000/api/product/addProduct", JsonConvert.SerializeObject(product), staffId);
            Console.Read();




        }
    }
}
