using Kingdee.BOS.Core.Bill.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;

namespace AddFullStockOA
{
    public class ButtonEvents : AbstractBillPlugIn
    {
        GenerateDt generateDt=new GenerateDt();



        public override void BarItemClick(BarItemClickEventArgs e)
        {
            var mesage = string.Empty;

            base.BarItemClick(e);

            if (e.BarItemKey == "tbClickOAFullStock")
            {
                var docScddIds1 = View.Model.DataObject;
                //获取表头中单据编号信息(注:这里的BillNo为单据编号中"绑定实体属性"项中获得)
                var orderno = docScddIds1["BillNo"].ToString();

                //todo:需检测此单据为‘审核’状态才能继续
                //获取当前登录用户
                var username = this.Context.UserName;  



            }

        }
    }
}
