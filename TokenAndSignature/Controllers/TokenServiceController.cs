using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using TokenAndSignature.Models;
using TokenAndSignature.Utils;

namespace TokenAndSignature.Controllers
{
    /// <summary>
    /// Token服务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TokenServiceController : ControllerBase
    {
        /// <summary>
        /// 根据用户名获取token
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [HttpGet("GetToken")]
        public ActionResult<ResultMsg> GetToken(string staffId)
        {
            ResultMsg resultMsg = null;
            int id = 0;

            //判断参数是否合法
            if (string.IsNullOrEmpty(staffId) || (!int.TryParse(staffId, out id)))
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.ParameterError;
                resultMsg.Info = StatusCodeEnum.ParameterError.GetEnumText();
                resultMsg.Data = "";
                return resultMsg;
            }

            //先从缓存中获取获取不到插入缓存
            Token token = (Token)CacheHelper.CacheValue(id.ToString());
            if (CacheHelper.CacheValue(id.ToString()) == null)
            {
                token = new Token();
                token.StaffId = id;
                token.SignToken = Guid.NewGuid();
                token.ExpireTime = DateTime.Now.AddDays(1);
                CacheHelper.CacheInsertAddMinutes(token.StaffId.ToString(), token, 10);
            }

            //返回token信息
            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = "";
            resultMsg.Data = token;

            return resultMsg;
        }

    }
}