using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenAndSignature.Attributes
{
    /// <summary>
    /// api异常统一处理过滤器
    /// 系统级别异常 500 应用级别异常501
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 处理程序中的异常
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            context.Result = BuildExceptionResult(context.Exception);
            base.OnException(context);
        }

        /// <summary>
        /// 包装处理异常格式
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private JsonResult BuildExceptionResult(Exception ex)
        {
            int code = 0;
            string message = "";
            string innerMessage = "";
            //应用程序业务级异常
            if (ex is ApplicationException)
            {
                code = 501;
                message = ex.Message;
            }
            else
            {
                // exception 系统级别异常，不直接明文显示的
                code = 500;
                message = "发生系统级别异常";
                innerMessage = ex.Message;
            }

            if (ex.InnerException != null && ex.Message != ex.InnerException.Message)
                innerMessage += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "——" +
                                ex.GetType().ToString() + "：" + ex.Message + "——堆栈信息：" +
                                ex.StackTrace;
            return new JsonResult(new { code, message, innerMessage });
        }
    }
}
