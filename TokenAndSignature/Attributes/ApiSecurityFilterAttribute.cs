using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TokenAndSignature.Models;
using TokenAndSignature.Tools;

namespace TokenAndSignature.Attributes
{
    public class ApiSecurityFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ResultMsg resultMsg = null;
            var request = context.HttpContext.Request;
            string method = request.Method;
            string staffid = String.Empty, timestamp = string.Empty, nonce = string.Empty, signature = string.Empty;
            int id = 0;

            if (request.Headers.ContainsKey("staffid"))
            {
                staffid = HttpUtility.UrlDecode(request.Headers["staffid"].FirstOrDefault());
            }
            if (request.Headers.ContainsKey("timestamp"))
            {
                timestamp = HttpUtility.UrlDecode(request.Headers["timestamp"].FirstOrDefault());
            }
            if (request.Headers.ContainsKey("nonce"))
            {
                nonce = HttpUtility.UrlDecode(request.Headers["nonce"].FirstOrDefault());
            }

            if (request.Headers.ContainsKey("signature"))
            {
                signature = HttpUtility.UrlDecode(request.Headers["signature"].FirstOrDefault());
            }

            //GetToken方法不需要进行签名验证
            if (((ControllerActionDescriptor)context.ActionDescriptor).ActionName == "GetToken")
            {
                if (string.IsNullOrEmpty(staffid) || (!int.TryParse(staffid, out id) || string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(nonce)))
                {
                    resultMsg = new ResultMsg();
                    resultMsg.StatusCode = (int)StatusCodeEnum.ParameterError;
                    resultMsg.Info = StatusCodeEnum.ParameterError.GetEnumText();
                    resultMsg.Data = "";
                    context.Result =new JsonResult(resultMsg);
                    base.OnActionExecuting(context);
                    return;
                }
                else
                {
                    base.OnActionExecuting(context);
                    return;
                }
            }


            //判断请求头是否包含以下参数
            if (string.IsNullOrEmpty(staffid) || (!int.TryParse(staffid, out id) || string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(nonce) || string.IsNullOrEmpty(signature)))
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.ParameterError;
                resultMsg.Info = StatusCodeEnum.ParameterError.GetEnumText();
                resultMsg.Data = "";
                context.Result = new JsonResult(resultMsg); 
                base.OnActionExecuting(context);
                return;
            }

           //判断timespan是否有效
            double ts1 = 0;
            double ts2 = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
            bool timespanvalidate = double.TryParse(timestamp, out ts1);
            double ts = ts2 - ts1;
            bool falg = ts >120*1000;
            if (falg || (!timespanvalidate))
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.URLExpireError;
                resultMsg.Info = StatusCodeEnum.URLExpireError.GetEnumText();
                resultMsg.Data = "";
                context.Result = new JsonResult(resultMsg);
                base.OnActionExecuting(context);
                return;
            }


            //判断token是否有效
            Token token = (Token)CacheHelper.CacheValue(id.ToString());
            string signtoken = string.Empty;
            if (CacheHelper.CacheValue(id.ToString()) == null)
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.TokenInvalid;
                resultMsg.Info = StatusCodeEnum.TokenInvalid.GetEnumText();
                resultMsg.Data = "";
                context.Result = new JsonResult(resultMsg);
                base.OnActionExecuting(context);
                return;
            }
            else
            {
                signtoken = token.SignToken.ToString();
            }

            //根据请求类型拼接参数
            IQueryCollection form = context.HttpContext.Request.Query;
            string data = string.Empty;
            switch (method)
            {
                case "POST":
                    Stream stream =context.HttpContext.Request.Body;
                    string responseJson = string.Empty;
                    StreamReader streamReader = new StreamReader(stream);
                    data = streamReader.ReadToEnd();
                    break;
                case "GET":
                    //第一步：取出所有get参数
                    IDictionary<string, string> parameters = new Dictionary<string, string>();

                    foreach(string item in form.Keys)
                    {
                        parameters.Add(item, form[item]);
                    }
                    // 第二步：把字典按Key的字母顺序排序
                    IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
                    IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

                    // 第三步：把所有参数名和参数值串在一起
                    StringBuilder query = new StringBuilder();
                    while (dem.MoveNext())
                    {
                        string key = dem.Current.Key;
                        string value = dem.Current.Value;
                        if (!string.IsNullOrEmpty(key))
                        {
                            query.Append(key).Append(value);
                        }
                    }
                    data = query.ToString();
                    break;
                default:
                    resultMsg = new ResultMsg();
                    resultMsg.StatusCode = (int)StatusCodeEnum.HttpMehtodError;
                    resultMsg.Info = StatusCodeEnum.HttpMehtodError.GetEnumText();
                    resultMsg.Data = "";
                    context.Result = new JsonResult(resultMsg);
                    base.OnActionExecuting(context);
                    return;
            }

            bool result = SignExtension.Validate(timestamp, nonce, id, signtoken, data, signature);
            if (!result)
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.HttpRequestError;
                resultMsg.Info = StatusCodeEnum.HttpRequestError.GetEnumText();
                resultMsg.Data = "";
                context.Result = new JsonResult(resultMsg);
                base.OnActionExecuting(context);
                return;
            }
            else
            {
                base.OnActionExecuting(context);
            }
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }

    }
}
