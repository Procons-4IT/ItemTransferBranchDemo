
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;

namespace ItemTransferBranchDemo
{

    [FormAttribute("721", "Goods Receipt.b1f")]
    public class Goods_Receipt : B1SystemForm
    {
        public string rr { get; set; }
        private SAPbouiCOM.Button Button0;
        private SAPbouiCOM.ChooseFromList oCFL;
        private SAPbouiCOM.DataTable cflValues;
        public List<string> selectedGoodsIssue;
        private List<Item> selectedItems = new List<Item>();
        private SAPbouiCOM.Matrix grMatrix;
        private bool flagCopyFrom = false;


        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("Item_0").Specific));
            this.Button0.ChooseFromListAfter += new SAPbouiCOM._IButtonEvents_ChooseFromListAfterEventHandler(this.Button0_ChooseFromListAfter);
            this.grMatrix = ((SAPbouiCOM.Matrix)(this.GetItem("13").Specific));
            this.OnCustomInitialize();

        }



        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.ActivateAfter += new SAPbouiCOM.Framework.FormBase.ActivateAfterHandler(this.Form_ActivateAfter);
            this.DataAddAfter += new DataAddAfterHandler(this.Form_DataAddAfter);

        }





        private void OnCustomInitialize()
        {
            B1SystemForm.goodsReceiptForms.Add(this.UIAPIRawForm.UniqueID, this);
            //Add Condictions on the CFL
            AddContionsToCopyFromCFL();

            //rr = B1SystemForm.GlobalParameters.Where(x => x.Key == this.UIAPIRawForm.UDFFormUID).First().Value.ToString();
             //add the conditions

            //SAPbouiCOM.ChooseFromListCollection oCFLs;         
            //SAPbouiCOM.Conditions oCons;
            //SAPbouiCOM.Condition oCon;
            //SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams;
            //oCFLs = this.UIAPIRawForm.ChooseFromLists;
            //oCFLCreationParams = Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams) as SAPbouiCOM.ChooseFromListCreationParams;
            //oCFLCreationParams.MultiSelection = true; //Change this to true
            //oCFLCreationParams.ObjectType = "60";
            //oCFLCreationParams.UniqueID = "CFL1";
            //oCFL = oCFLs.Add(oCFLCreationParams);
            //Button0.ChooseFromListUID = "CFL1";        


        }

        private void AddContionsToCopyFromCFL()
        {
            var cflCopyFromGI = this.UIAPIRawForm.ChooseFromLists.Item("Copy_From_0");
            var conditions = cflCopyFromGI.GetConditions();
            var transferCondition = conditions.Add();
            transferCondition.Alias="U_isTransfered";
            transferCondition.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
            transferCondition.CondVal = "NP";
            cflCopyFromGI.SetConditions(conditions);
        }






        private void Form_ActivateAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {

            if (selectedItems != null && selectedItems.Count > 0 && flagCopyFrom)
            {
                flagCopyFrom = false;
                //Fill the Branch From b
                populateItemsFromGR(selectedItems);
            }
        }

        private void Button0_ChooseFromListAfter(object sboObject,SAPbouiCOM.SBOItemEventArg pVal)
        {
            var ppVal = pVal as SAPbouiCOM.ISBOChooseFromListEventArg;
            if (ppVal.SelectedObjects != null)
            {

                selectedGoodsIssue = ppVal.SelectedObjects.GetColumnValueAsList(1);
                selectedItems = B1Helper.GetItemsForGoodsIssues(selectedGoodsIssue);
                flagCopyFrom = true;
            }
        
        }

        private void updateGoodsIssueTransferedFlag(List<string> goodsIssues)
        {
            var recordSet = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
            try
            {
                string docEntries = goodsIssues[0];
                for(int i=1;i<goodsIssues.Count;i++)
                {
                    string.Concat(docEntries,",",goodsIssues[1]);
                }
                var query = string.Format("UPDATE OIGE SET U_isTransfered = 'P' WHERE DocEntry in ({0})",docEntries);
                recordSet.DoQuery(query);
            }
            catch(Exception ex)
            {
                Utilities.StatusbarMessage(string.Concat("ERR: GR.updateGoodsIssueTrnsfFlag ",ex.ToString()), SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }finally
            {
                recordSet.ReleaseObject();
            }
        }



        internal void populateItemsFromGR(List<Item> items)
        {

            grMatrix.Clear();
            this.UIAPIRawForm.Freeze(true);
            int rows = items.Count;

            for (int i = 1; i <= rows; i++)
            {
                grMatrix.AddRow();
                try
                {
                    //grMatrix.SetCellWithoutValidation(i,"1", items[i - 1].ItemCode);
                    //SAPbouiCOM.EditText tempCell2 = (grMatrix.Columns.Item("2").Cells.Item(i).Specific as SAPbouiCOM.EditText);

                
                    grMatrix.SetCellValue("1", i, items[i - 1].ItemCode); //ItemCode
                    grMatrix.SetCellValue("9", i, items[i - 1].Quantity); //Item Quantity                    
                    grMatrix.SetCellValue("15", i, items[i - 1].WhsCode); //Item Whs Code 
                    //grMatrix.SetCellWithoutValidation(i, "15", items[i - 1].WhsCode);
                    //(grMatrix.Columns.Item("15").Cells.Item(i).Specific as SAPbouiCOM.EditText).String = items[i - 1].WhsCode;
                   
                     
                   // grMatrix.SetCellValue("231000074", i, items[i - 1].BaseDocEntry); //Base Reference
                }
                catch (Exception ex)
                {
                    Utilities.LogErrors(string.Concat("Exception Occured at GoodsReceipt.populateItemsFromGR: ", ex.ToString()));
                    Application.SBO_Application.MessageBox(string.Format("Error Can't Add {0}. {1}", items[i - 1].ItemCode, ex.ToString()));
                }
            }
            this.UIAPIRawForm.Freeze(false);

        }

        private void Form_DataAddAfter(ref SAPbouiCOM.BusinessObjectInfo pVal)
        {
            if (selectedGoodsIssue !=null)
            { 
                //Update the Flag in the Good Issue Flag
                updateGoodsIssueTransferedFlag(selectedGoodsIssue);
            }   

        }


    }
}
