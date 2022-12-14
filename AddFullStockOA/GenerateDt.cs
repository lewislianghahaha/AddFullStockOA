using System;
using System.Data;
using AddFullStockOA.WebReference;

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
            var custDt = new DataTable();            //收集'客户'记录表 
            var receivebillDt = new DataTable();    //收集‘收款单’记录表
            decimal receiveable = 0;               //收集‘应收单’记录
            var custAccount = new DataTable();      //收集‘客户信用额度’记录

            try
            {
                //获取临时表-新增OA流程使用
                var oatempdt = tempdt.InsertOaRecord();

                //根据orderno获取‘发货通知单’信息
                var noticeDt = searchDt.SearchDeliveryNotice(orderno).Copy();

                //根据username获取OA-人员ID 及 部门ID
                var oaDt = searchDt.SearchOaInfo(username).Copy();

                //根据noticeDt中的custid获取‘客户’信息
                if (noticeDt.Rows.Count > 0)
                    custDt = searchDt.SearchCustomerInfo(Convert.ToInt32(noticeDt.Rows[0][0])).Copy();

                //根据noticeDt中的custid获取‘收款单’信息
                if (noticeDt.Rows.Count > 0)
                    receivebillDt = searchDt.SearchReciveBillInfo(Convert.ToInt32(noticeDt.Rows[0][0])).Copy();

                //根据noticeDt中的custid获取'应收单'信息
                if (noticeDt.Rows.Count > 0)
                    receiveable = searchDt.SearchReceivableInfo(Convert.ToInt32(noticeDt.Rows[0][0]));

                //检查客户是否有信用额度,有才将客户的所有相关信息插入
                if (noticeDt.Rows.Count > 0)
                    custAccount = searchDt.CheckCustAccount(Convert.ToInt32(noticeDt.Rows[0][0]));

                //将以上收集的信息插入至oatempdt内
                oatempdt.Merge(InsertDtIntoTemp(oatempdt,noticeDt,oaDt,custDt,receivebillDt,receiveable, custAccount));

                //将oatempdt数据作为OA接口进行输出,并最后执行OA API方法
                var resultid = CreateOaWorkFlow(Convert.ToInt32(oaDt.Rows[0][0]),oatempdt);
                result = Convert.ToInt32(resultid) > 0 ? "Finish" : "生成OA-超额客户出货流程导常,请联系管理员";
               // result = Convert.ToString(oatempdt.Rows.Count);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }


        /// <summary>
        /// 将相关记录插入至临时表内
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="noticedt">发货通知单临时表</param>
        /// <param name="oadt">OA临时表</param>
        /// <param name="custdt">客户临时表</param>
        /// <param name="receivebillDt">收款单临时表</param>
        /// <param name="receiveable">应收单临时表</param>
        /// <param name="custAccount">客户信用额度-需要在此临时表有记录才将客户相关值插入;反之插入空值</param>
        /// <returns></returns>
        private DataTable InsertDtIntoTemp(DataTable dt,DataTable noticedt,DataTable oadt,DataTable custdt, DataTable receivebillDt, decimal receiveable
                                           , DataTable custAccount)
        {
            //将相关值插入至tempdt表内
            var newrow = dt.NewRow();
            newrow[0] = oadt.Rows.Count > 0 ? Convert.ToInt32(oadt.Rows[0][0]) : 0;                   //申请人   --来源:OA临时表
            newrow[1] = oadt.Rows.Count > 0 ? Convert.ToString(DateTime.Now.Date) : "";               //申请日期 --来源:OA临时表
            newrow[2] = oadt.Rows.Count > 0 ? Convert.ToInt32(oadt.Rows[0][2]) : 0;                   //申请部门  --来源:OA临时表
            newrow[3] = oadt.Rows.Count > 0 ? Convert.ToInt32(oadt.Rows[0][3]) : 0;                   //岗位      --来源:OA临时表

            if (custAccount.Rows.Count > 0)
            {
                newrow[4] = custAccount.Rows.Count > 0 ? Convert.ToString(custAccount.Rows[0][0]) : "";   //客户ID  --来源:custAccount
                newrow[5] = custdt.Rows.Count > 0 ? Convert.ToString(custdt.Rows[0][2]) : "";             //客户名称  --来源:custdt
                newrow[6] = noticedt.Rows.Count > 0 ? Convert.ToDecimal(noticedt.Rows[0][3]) : 0;         //当前信用额度(元) --来源:noticedt *
                newrow[7] = custdt.Rows.Count > 0 ? Convert.ToString(custdt.Rows[0][3]) : "";             //经销商名称(营业执照为准) --来源:custdt
                newrow[8] = custdt.Rows.Count > 0 ? Convert.ToString(custdt.Rows[0][4]) : "";             //法人姓名 --来源:custdt
                newrow[9] = custdt.Rows.Count > 0 ? Convert.ToString(custdt.Rows[0][5]) : "";             //开始合作时间 --来源:custdt
                newrow[10] = noticedt.Rows.Count > 0 ? Convert.ToString(noticedt.Rows[0][1]) : "";        //K3出库单号 --来源:noticedt *
                newrow[11] = noticedt.Rows.Count > 0 ? Convert.ToString(noticedt.Rows[0][2]) : "";        //销售订单号  --来源:noticedt *
                newrow[12] = custdt.Rows.Count > 0 ? Convert.ToString(custdt.Rows[0][6]) : "";            //经营区域 --来源:custdt
                newrow[13] = custdt.Rows.Count > 0 ? Convert.ToInt32(custdt.Rows[0][7]) : 0;              //币别     --来源:custdt
                newrow[14] = receiveable; //月均销售额(元)  --来源:receiveable
                newrow[15] = noticedt.Rows.Count > 0 ? Convert.ToDecimal(noticedt.Rows[0][4]) : 0;        //信用周期(天)    --来源:noticedt *
                newrow[16] = custdt.Rows.Count > 0 ? Convert.ToString(custdt.Rows[0][8]) : "";            //收款条件  --来源:custdt
                newrow[17] = noticedt.Rows.Count > 0 ? Convert.ToDecimal(noticedt.Rows[0][5]) : 0;        //超额欠款(元)    --来源:noticedt *
                newrow[18] = noticedt.Rows.Count > 0 ? Convert.ToString(noticedt.Rows[0][6]) : "";        //超期天数(天)    --来源:noticedt *

                newrow[19] = receivebillDt.Rows.Count > 0 ? Convert.ToDecimal(receivebillDt.Rows[0][1]) : 0;            //最后一次收款金额  --来源:receivebillDt
                newrow[20] = receivebillDt.Rows.Count > 0 ? Convert.ToString(receivebillDt.Rows[0][0]) : "";            //最后一次收款时间  --来源:receivebillDt

                newrow[21] = noticedt.Rows.Count > 0 ? Convert.ToDecimal(noticedt.Rows[0][7]) : 0;         //当天申请出货金额(元) --来源:noticedt *
                newrow[22] = noticedt.Rows.Count > 0 ? Convert.ToDecimal(noticedt.Rows[0][8]) : 0;         //出货后超出信用额度欠款(元) --来源:noticedt *
            }
            else
            {
                newrow[4] = "";        //客户代码
                newrow[5] = "";        //客户名称
                newrow[6] = 0;         //当前信用额度(元) 
                newrow[7] = "";        //经销商名称(营业执照为准)
                newrow[8] = "";        //法人姓名
                newrow[9] = "";        //开始合作时间
                newrow[10] = "";       //K3出库单号
                newrow[11] = "";       //销售订单号
                newrow[12] = "";       //经营区域
                newrow[13] = 0;        //币别
                newrow[14] = 0;        //月均销售额(元)
                newrow[15] = 0;        //信用周期(天)
                newrow[16] = "";       //收款条件
                newrow[17] = 0;        //超额欠款(元)
                newrow[18] = "";       //超期天数(天)

                newrow[19] = 0;        //最后一次收款金额
                newrow[20] = "";       //最后一次收款时间

                newrow[21] = 0;        //当天申请出货金额(元)
                newrow[22] = 0;        //出货后超出信用额度欠款(元)
            }
            dt.Rows.Add(newrow);
            return dt;
        }

        /// <summary>
        /// 根据获取的临时表记录,并利用OA API创建流程接口,创建流程
        /// </summary>
        /// <param name="createid">用户ID;创建流程时必需</param>
        /// <param name="resultdt"></param>
        /// <returns></returns>
        private string CreateOaWorkFlow(int createid,DataTable resultdt)
        {
            var result = string.Empty;

            WorkflowService workflow=new WorkflowService();

            WorkflowRequestInfo workflowRequestInfo = new WorkflowRequestInfo();
            WorkflowBaseInfo baseInfo = new WorkflowBaseInfo();

            //设置工作流ID_必须添加(重)
            baseInfo.workflowId = "68";  //"129";
            baseInfo.workflowName = "超额客户出货";

            //设置如能否修改 查询等基础信息
            workflowRequestInfo.canView = true;
            workflowRequestInfo.canEdit = true;
            workflowRequestInfo.requestName = baseInfo.workflowName;   //设置标题_此项必须添加(重)
            workflowRequestInfo.requestLevel = "0";
            workflowRequestInfo.creatorId = "249";//Convert.ToString(createid);  //设置创建者ID(重要:创建流程时必须填)

            workflowRequestInfo.workflowBaseInfo = baseInfo;

            //主表设置
            WorkflowMainTableInfo workflowMainTableInfo = new WorkflowMainTableInfo();
            WorkflowRequestTableRecord[] workflowRequestTableRecords = new WorkflowRequestTableRecord[1]; //设置主表字段有一条记录
            WorkflowRequestTableField[] workflowtabFields = new WorkflowRequestTableField[23];  //设置主表有多少个字段

            //循环设置各列字段的相关信息
            workflowtabFields[0] = new WorkflowRequestTableField();
            workflowtabFields[0].fieldName = "sqr";
            workflowtabFields[0].fieldValue = "249";
            workflowtabFields[0].view = true;
            workflowtabFields[0].edit = true;

            //for (var i = 0; i < resultdt.Columns.Count; i++)
            //{
            //    workflowtabFields[i] = new WorkflowRequestTableField();
            //    workflowtabFields[i].fieldName = resultdt.Columns[i].ColumnName;  //字段名称
            //    workflowtabFields[i].fieldValue = Convert.ToString(resultdt.Rows[0][i]); //字段值
            //    workflowtabFields[i].view = true;  //能否查阅
            //    //除‘销售订单号’(11)可以修改外,其它都不能修改
            //    //workflowtabFields[i].edit = i == 11;
            //    workflowtabFields[i].edit = true;   //这里必须要设置为true,可修改,不然会插入不到记录至表格
            //}

            //将workflowtableFields所设置的字段加载到workflowRequestTableRecords内
            workflowRequestTableRecords[0] = new WorkflowRequestTableRecord();
            workflowRequestTableRecords[0].workflowRequestTableFields = workflowtabFields;

            //然后将workflowRequestTableRecords加载到workflowMainTableInfo.requestRecords内
            workflowMainTableInfo.requestRecords = workflowRequestTableRecords;

            //最后将workflowMainTableInfo加载到workflowRequestInfo.workflowMainTableInfo内
            workflowRequestInfo.workflowMainTableInfo = workflowMainTableInfo;

            //执行doCreateWorkflowRequest()方法,若返回值>0 就成功;反之,出现异常
            result = workflow.doCreateWorkflowRequest(workflowRequestInfo, 249);

            return result;
        }

    }
}
