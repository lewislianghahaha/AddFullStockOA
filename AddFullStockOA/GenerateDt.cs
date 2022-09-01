using System;
using System.Data;

namespace AddFullStockOA
{
    //运算
    public class GenerateDt
    {
        SearchDt searchDt=new SearchDt();
        Tempdt tempdt=new Tempdt();
        
        /// <summary>
        /// 获取相关信息,并将K3信息通过OA接口传输至OA,最后达到创建新流程目的
        /// </summary>
        /// <param name="orderno">发货通知单号</param>
        /// <param name="username">当前用户名称</param>
        /// <returns></returns>
        public string GetMessageIntoOa(string orderno,string username)
        {
            var result = "Finish";

            try
            {
                //获取临时表-新增OA流程使用
                var oatempdt = tempdt.InsertOaRecord();

                //根据orderno获取‘发货通知单’信息
                var noticeDt = searchDt.SearchDeliveryNotice(orderno).Copy();

                //根据username获取OA-人员ID 及 部门ID
                var oaDt = searchDt.SearchOaInfo(username).Copy();

                //todo:根据客户

                //todo:

                //todo:

                //todo:


            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }


        private DataTable InsertDtIntoTemp()
        {
            
        }



    }
}
