using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemTransferBranchDemo
{
    public static class ExtensionMethods
    {
        public static List<string> GetColumnValueAsList(this SAPbouiCOM.DataTable control, object column)
        {
            List<string> values = new List<string>();
            for (int i = 0; i < control.Rows.Count; i++)
            {
                values.Add(control.GetValue(column, i).ToString());
            }
                return values;
        }

        public static void SetCellValue(this SAPbouiCOM.Matrix control, object column, object row, object newValue)
        {
         
             try
             {
                 control.Columns.Item(column).Cells.Item(row).Click();
                 ((dynamic)control.Columns.Item(column).Cells.Item(row).Specific).Value = newValue;
             }
             catch (Exception ex)
             {

             }
             }
        public static object GetCellValue(this SAPbouiCOM.Matrix control, string column, object row)
        {
            return ((dynamic)control.Columns.Item(column).Cells.Item(row).Specific).Value;
        }
        public static int GetColumnIndex(this SAPbouiCOM.Matrix mtxControl, string columnName)
        {
            var index = 0;
            for (int i = 0; i < mtxControl.Columns.Count; i++)
            {
                if (mtxControl.Columns.Item(i).UniqueID == columnName)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public static void ReleaseObject(this object ob)
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ob);
            ob = null;
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }


    }
}
