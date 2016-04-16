using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemTransferBranchDemo
{
    public class B1SystemForm : SystemFormBase
    {
        public static List<SystemFormParameter> GlobalParameters { get; set; }
        public static Dictionary<string, Goods_Receipt> goodsReceiptForms = new Dictionary<string, Goods_Receipt>();
    }

    public class SystemFormParameter
    {
        public string Key{get;set;}
        public string Value{get;set;}
    }
}
