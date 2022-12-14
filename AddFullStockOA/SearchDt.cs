using System;
using System.Data;
using System.Data.SqlClient;

namespace AddFullStockOA
{
    //查询
    public class SearchDt
    {
        ConDb conDb = new ConDb();
        SqlList sqlList = new SqlList();

        /// <summary>
        /// 根据SQL语句查询得出对应的DT
        /// </summary>
        /// <param name="conid">0:连接K3 1:连接OA</param>
        /// <param name="sqlscript"></param>
        /// <returns></returns>
        private DataTable UseSqlSearchIntoDt(int conid, string sqlscript)
        {
            var resultdt = new DataTable();

            try
            {
                var sqlDataAdapter = new SqlDataAdapter(sqlscript, conDb.GetK3CloudConn(conid));
                sqlDataAdapter.Fill(resultdt);
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }

            return resultdt;
        }

        /// <summary>
        /// 发货通知单相关信息获取
        /// </summary>
        /// <returns></returns>
        public DataTable SearchDeliveryNotice(string orderno)
        {
            var dt = UseSqlSearchIntoDt(0, sqlList.SearchDeliveryNotice(orderno));
            return dt;
        }

        /// <summary>
        /// 根据客户ID查询相关信息
        /// </summary>
        /// <param name="custid"></param>
        /// <returns></returns>
        public DataTable SearchCustomerInfo(int custid)
        {
            var dt = UseSqlSearchIntoDt(0, sqlList.SearchCustomerInfo(custid));
            return dt;
        }

        /// <summary>
        /// 根据客户ID获取收款单相关信息
        /// </summary>
        /// <param name="custid"></param>
        /// <returns></returns>
        public DataTable SearchReciveBillInfo(int custid)
        {
            var dt = UseSqlSearchIntoDt(0, sqlList.SearchReciveBillInfo(custid));
            return dt;
        }

        /// <summary>
        /// 根据客户ID获取应收单相关信息
        /// </summary>
        /// <param name="custid"></param>
        /// <returns></returns>
        public decimal SearchReceivableInfo(int custid)
        {
            var result = Convert.ToDecimal(UseSqlSearchIntoDt(0, sqlList.SearchReceivableInfo(custid)).Rows[0][0]);
            return result;
        }

        /// <summary>
        /// 检查客户是否有信用额度,有才将客户的所有相关信息插入
        /// </summary>
        /// <param name="custid"></param>
        /// <returns></returns>
        public DataTable CheckCustAccount(int custid)
        {
            var dt = UseSqlSearchIntoDt(0, sqlList.CheckCustAccount(custid));
            return dt;
        }

        /// <summary>
        /// 根据用户名称获取OA-用户ID及部门ID信息
        /// OA使用
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable SearchOaInfo(string username)
        {
            var dt = UseSqlSearchIntoDt(1, sqlList.SearchOaInfo(username));
            return dt;
        }

    }
}
