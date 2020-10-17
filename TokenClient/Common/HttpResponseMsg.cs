using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TokenAndSignature.Models;

namespace TokenClient.Common
{
    public class HttpResponseMsg
    {
        public int StatusCode { get; set; }
        public object Data { get; set; }
        public string Info { get; set; }
    }

    public class ProductResultMsg : HttpResponseMsg
    {
        public Product Result
        {
            get
            {
                if (StatusCode == (int)StatusCodeEnum.Success)
                {
                    return JsonConvert.DeserializeObject<Product>(Data.ToString());
                }

                return null;
            }
        }
    }

    public class TokenResultMsg : HttpResponseMsg
    {
        public Token Result
        {
            get
            {
                if (StatusCode == (int)StatusCodeEnum.Success)
                {
                    return JsonConvert.DeserializeObject<Token>(Data.ToString());
                }

                return null;
            }
        }
    }
}
