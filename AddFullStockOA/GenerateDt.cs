using System;

namespace AddFullStockOA
{
    //运算
    public class GenerateDt
    {
        /// <summary>
        /// 获取相关信息,并将K3信息通过OA接口传输至OA,最后达到创建新流程目的
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public string GetMessageIntoOa(string orderno)
        {
            var result = "Finish";

            try
            {

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
    }
}
