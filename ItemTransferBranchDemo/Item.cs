using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemTransferBranchDemo
{
    public class Item
    {

        public string ItemCode { get; set; }
        public double Quantity { get; set; }

        public int BaseDocEntry { get; set; }
        public int BaseDocType{ get; set; }

        public string WhsCode { get; set; }
    }
}
