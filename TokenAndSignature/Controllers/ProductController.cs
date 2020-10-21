using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TokenAndSignature.Models;

namespace TokenAndSignature.Controllers
{
    /// <summary>
    /// 产品控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetProduct")]
        public ActionResult<ResultMsg> GetProduct(string id)
        {
            var product = new Product() { Id = 1, Name = "哇哈哈", Count = 10, Price = 38.8 };
            ResultMsg resultMsg = null;
            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = StatusCodeEnum.Success.GetEnumText();
            resultMsg.Data = product;
            return resultMsg;
        }


        /// <summary>
        /// 添加商品信息
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost("AddProduct")]
        public ActionResult<ResultMsg> AddProduct([FromBody]Product product)
        {
            ResultMsg resultMsg = null;
            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = StatusCodeEnum.Success.GetEnumText();
            resultMsg.Data = product;
            return resultMsg;
        }


    }
}