using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbouiCOM.Framework;

namespace ItemTransferBranchDemo
{
   public class B1Helper
    {
       private static SAPbobsCOM.Company diCompany = null;
       public static SAPbobsCOM.Company DiCompany
       {
           get
           {
               if (diCompany == null)
                   diCompany = Application.SBO_Application.Company.GetDICompany() as SAPbobsCOM.Company;
               return diCompany;
           }
       }
       public static List<Item> getItemsForGoodsIssue(int id)
       {
           var goodsIssue = DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit) as SAPbobsCOM.Documents;
           List<Item> items = new List<Item>() ;

           if (goodsIssue.GetByKey(id))
           {
               for (int i = 0; i < goodsIssue.Lines.Count; i++)
               {
                   goodsIssue.Lines.SetCurrentLine(i);
                   items.Add(new Item { ItemCode = goodsIssue.Lines.ItemCode
                       , Quantity = goodsIssue.Lines.Quantity,BaseDocEntry=goodsIssue.DocEntry
                       ,WhsCode = goodsIssue.UserFields.Fields.Item("U_toWhs").Value.ToString(),BaseDocType = 60
                   });                
               
               }
           }

           return items;
       }
       public static List<Item> getItemsForGoodsIssues(List<string> goodsIssueIDs)
       {
           List<Item> items = new List<Item>();
           foreach (var id in goodsIssueIDs)
           {
               var goodsIssueItems = getItemsForGoodsIssue(Convert.ToInt32(id));
               items.AddRange(goodsIssueItems);
           }
           return items;
       }
       #region FIELD METHODS

       public static DateTime getDateTimeEditTxt(SAPbouiCOM.EditText txtbox)
       {

           try
           {

               return txtbox.String == String.Empty ? System.DateTime.Now : Convert.ToDateTime(txtbox.String);
           }
           catch (Exception ex)
           {
               Utilities.LogErrors(string.Format("Error Occured At Class {0}, Method {1}: {2}", "B1Helper", "getDateTimeEditTxt", ex.ToString()));
               return System.DateTime.Now;

           }

       }
       /// <summary>
       /// A method for adding new field to B1 table
       /// </summary>
       /// <param name="name">Field Name</param>
       /// <param name="description">Field description</param>
       /// <param name="tableName">Table the field will be added to</param>
       /// <param name="fieldType">Field Type</param>
       /// <param name="size">Field size in the database</param>
       /// <param name="subType"></param>
       /// <param name="mandatory"></param>
       /// <param name="addedToUDT">If this field will be added to system table or User defined table</param>
       /// <param name="valiedValue">The default selected value</param>
       /// <param name="validValues">Add the values seperated by comma "," for value and description ex:(Value,Description)</param>
       public static void AddField(string name, string description, string tableName, SAPbobsCOM.BoFieldTypes fieldType, Nullable<int> size, SAPbobsCOM.BoYesNoEnum mandatory, SAPbobsCOM.BoFldSubTypes subType, bool addedToUDT, string validValue, params string[] validValues)
       {

           var objUserFieldMD = DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields) as SAPbobsCOM.UserFieldsMD;
           try
           {
               if (addedToUDT)
                   tableName = string.Format("@{0}", tableName);
               if (!IsFieldExists(name, tableName))
               {
                   objUserFieldMD.TableName = tableName;
                   objUserFieldMD.Name = name;
                   objUserFieldMD.Description = description;
                   objUserFieldMD.Type = fieldType;
                   objUserFieldMD.Mandatory = mandatory;

                   if (size == null || size <= 0)
                       size = 10;

                   if (fieldType != SAPbobsCOM.BoFieldTypes.db_Numeric)
                       objUserFieldMD.Size = (int)size;
                   else
                       objUserFieldMD.EditSize = (int)size;


                   objUserFieldMD.SubType = subType;

                   if (validValue != null)
                       objUserFieldMD.DefaultValue = validValue;

                   if (validValues != null)
                   {
                       foreach (string s in validValues)
                       {
                           var valuesAttributes = s.Split(',');
                           if (valuesAttributes.Length == 2)
                               objUserFieldMD.ValidValues.Description = valuesAttributes[1];
                           objUserFieldMD.ValidValues.Value = valuesAttributes[0];
                           objUserFieldMD.ValidValues.Add();
                       }
                   }

                   if (objUserFieldMD.Add() != 0)
                   {
                       var error = Utilities.GetErrorMessage();
                       Utilities.LogException(error);
                   }
               }
           }
           catch (Exception ex)
           {
               Utilities.LogException(ex.ToString());
               Utilities.LogErrors(string.Format("Error Occured At Class {0}, Method {1}: {2}", "B1Helper", "AddField", ex.ToString()));

           }
           finally
           {
               objUserFieldMD.ReleaseObject();
           }
       }

       /// <summary>
       /// A method for adding new field to B1 table
       /// </summary>
       /// <param name="name">Field Name</param>
       /// <param name="description">Field description</param>
       /// <param name="tableName">Table the field will be added to</param>
       /// <param name="fieldType">Field Type</param>
       /// <param name="size">Field size in the database</param>       
       /// <param name="mandatory">bool: if the value is mandatory to be filled</param>
       /// <param name="subType"></param>
       /// <param name="addedToUDT">If this field will be added to system table or User defined table</param>
       public static void AddField(string name, string description, string tableName, SAPbobsCOM.BoFieldTypes fieldType, Nullable<int> size, SAPbobsCOM.BoYesNoEnum mandatory, SAPbobsCOM.BoFldSubTypes subType, bool addedToUDT)
       {
           AddField(name, description, tableName, fieldType, size, mandatory, subType, addedToUDT, null);
       }

       /// <summary>
       /// A method for adding new field to B1 table
       /// </summary>
       /// <param name="name">Field Name</param>
       /// <param name="description">Field description</param>
       /// <param name="tableName">Table the field will be added to</param>
       /// <param name="fieldType">Field Type</param>
       /// <param name="size">Field size in the database</param>     
       /// <param name="mandatory">bool: if the value is mandatory to be filled</param>
       /// <param name="subType">Sub field type</param>
       public static void AddField(string name, string description, string tableName, SAPbobsCOM.BoFieldTypes fieldType, SAPbobsCOM.BoYesNoEnum mandatory, SAPbobsCOM.BoFldSubTypes subType, bool addedToUDT)
       {
           AddField(name, description, tableName, fieldType, null, mandatory, subType, addedToUDT);
       }

       public static void AddField(string name, string description, string tableName, SAPbobsCOM.BoFieldTypes fieldType, int size, SAPbobsCOM.BoYesNoEnum mandatory, bool addedToUDT)
       {
           AddField(name, description, tableName, fieldType, size, mandatory, 0, addedToUDT);
       }

       /// <summary>
       /// A method for adding new field to B1 table
       /// </summary>
       /// <param name="name">Field Name</param>
       /// <param name="description">Field description</param>
       /// <param name="tableName">Table the field will be added to</param>
       /// <param name="fieldType">Field Type</param>
       /// <param name="size">Field size in the database</param>     
       /// <param name="mandatory">bool: if the value is mandatory to be filled</param>
       public static void AddField(string name, string description, string tableName, SAPbobsCOM.BoFieldTypes fieldType, SAPbobsCOM.BoYesNoEnum mandatory, bool addedToUDT)
       {
           AddField(name, description, tableName, fieldType, null, mandatory, 0, addedToUDT);
       }


       /// <summary>
       /// Check if the field is already created in a table
       /// </summary>
       /// <param name="fieldName">Field name to be checked</param>
       /// <param name="tableName">table to checked the values in</param>
       /// <returns>bool: return the value if teh field is created or not</returns>
       public static bool IsFieldExists(string fieldName, string tableName)
       {
           var recordsSet = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
           try
           {
               StringBuilder query = new StringBuilder("SELECT COUNT('AliasID') AS Count ");
               query.Append("FROM CUFD ");
               query.Append("WHERE AliasID ='{0}' AND TableID = '{1}'");


               recordsSet.DoQuery(string.Format(query.ToString(), fieldName, tableName));
               recordsSet.MoveFirst();
               if (Convert.ToInt32(recordsSet.Fields.Item("Count").Value) > 0)
                   return true;
               else
                   return false;
           }
           catch (Exception ex)
           {
               Utilities.LogErrors(string.Format("Error Occured At Class {0}, Method {1}: {2}", "B1Helper", "IsFieldExists", ex.ToString()));

               return true;
           }
           finally
           {
               recordsSet.ReleaseObject();
           }

           //var records = SqlHelper.SBODEMOUSEntities.CUFDs.Where(x => x.AliasID == fieldName && x.TableID == tableName).Count();
           //if (records > 0)
           //    return true;
           //else
           //    return false;
       }

       #endregion
    }
}
