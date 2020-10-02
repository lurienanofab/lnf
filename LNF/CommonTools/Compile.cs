using LNF.Repository;
using System;
using System.CodeDom.Compiler;
using System.Data;
using System.Linq;
using System.Text;

namespace LNF.CommonTools
{
    /// <summary>
    /// Used in Reservation.aspx.vb to estimate reservation fees for the create reservation confirmation message.
    /// </summary>
    public class Compile
    {
        // these are public so conFormula has access to them for display purposes
        public string[] RoomVars;
        public string[] ToolVars;
        public string[] StoreInvVars;
        public string[] StoreJEVars;
        public string[][] aryVars;

        public string[] RoomVarTypes;
        public string[] ToolVarTypes;
        public string[] StoreInvVarTypes;
        public string[] StoreJEVarTypes;
        public string[][] aryVarTypes;

        public string[] RoomVarFormats;
        public string[] ToolVarFormats;
        public string[] StoreInvVarFormats;
        public string[] StoreJEVarFormats;
        public string[][] aryVarFormats;

        public string[] RoomNotes;
        public string[] ToolNotes;
        public string[] StoreInvNotes;
        public string[] StoreJENotes;
        public string[][] aryNotes;

        public enum AggType
        {
            None = 0,
            // means that no aggregating of costs happens
            CliAcctType = 1,
            // means that aggregation happens based on client, account and the item type (toom, tool, store cat)
            CliAcct = 2
            // means that aggregation happens based on client and account
        }

        private DataSet ds = null;

        public DataTable GetTable(int index)
        {
            if (ds != null)
            {
                if (ds.Tables.Count > index)
                {
                    return ds.Tables[index];
                }
            }
            return null;
        }

        public double CapCost = 0.0;
        private System.Reflection.Assembly mAssembly;
        private Type scriptType;
        private object instance;

        // the new is needed to get the definitions of the auxilliary parameters
        public Compile()
        {
            InitArrays();

            var dtAuxCost = DefaultDataCommand.Create()
                .Param("CostType", "All")
                .FillDataTable("dbo.AuxCost_Select");

            string[] arySearch = { "'Room*'", "'Tool*'", "'Store*'", "'Store*'" };

            DataRow[] fdr = null;

            for (int j = 0; j <= 3; j++)
            {
                fdr = dtAuxCost.Select("AuxCostParm like " + arySearch[j]);
                for (int i = 0; i <= fdr.GetUpperBound(0); i++)
                {
                    if (Convert.ToBoolean(fdr[i]["AllowPerUse"]))
                        AddAuxParamToArray(j, fdr[i]["AuxCostParm"].ToString(), "Add", fdr[i]["Description"].ToString());
                    if (Convert.ToBoolean(fdr[i]["AllowPerPeriod"]))
                        AddAuxParamToArray(j, fdr[i]["AuxCostParm"].ToString(), "Mul", fdr[i]["Description"].ToString());
                }
            }
        }

        public void InitArrays()
        {
            RoomVars = new string[]
            {
                "Entries",
                "Hours",
                "Days",
                "Months",
                "CostPeriod",
                "PerEntry",
                "PerPeriod"
            };

            string[] ToolVars = new string[]
            {
                "Uses",
                "SchedDuration",
                "ActDuration",
                "OverTime",
                "IsStarted",
                "Days",
                "Months",
                "CostPeriod",
                "PerUse",
                "PerPeriod"
            };

            StoreInvVars = new string[]
            {
                "ItemCost",
                "Rebate"
            };

            StoreJEVars = new string[]
            {
                "ItemCost",
                "Rebate"
            };

            aryVars = new string[][]
            {
                RoomVars,
                ToolVars,
                StoreInvVars,
                StoreJEVars
            };

            RoomVarTypes = new string[]
            {
                "Double",
                "Double",
                "Double",
                "Double",
                "String",
                "Double",
                "Double"
            };

            ToolVarTypes = new string[]
            {
                "Double",
                "Double",
                "Double",
                "Double",
                "Boolean",
                "Double",
                "Double",
                "String",
                "Double",
                "Double"
            };

            StoreInvVarTypes = new string[]
            {
                "Double",
                "Double"
            };

            StoreJEVarTypes = new string[]
            {
                "Double",
                "Double"
            };

            aryVarTypes = new string[][]
            {
                RoomVarTypes,
                ToolVarTypes,
                StoreInvVarTypes,
                StoreJEVarTypes
            };

            RoomVarFormats = new string[]
            {
                "{0:#.0}",
                "{0:#.0}",
                "{0:#.0}",
                "{0:#.0}",
                "",
                "{0:$#,##0.00}",
                "{0:$#,##0.00}"
            };

            ToolVarFormats = new string[]
            {
                "{0:#.0}",
                "{0:#.0}",
                "{0:#.0}",
                "{0:#.0}",
                "",
                "{0:#.0}",
                "{0:#.0}",
                "",
                "{0:$#,##0.00}",
                "{0:$#,##0.00}"
            };

            StoreInvVarFormats = new string[]
            {
                "{0:$#,##0.00}",
                "{0:#,##0.0}"
            };

            StoreJEVarFormats = new string[]
            {
                "{0:$#,##0.00}",
                "{0:#,##0.0}"
            };

            aryVarFormats = new string[][]
            {
                RoomVarFormats,
                ToolVarFormats,
                StoreInvVarFormats,
                StoreJEVarFormats
            };

            RoomNotes = new string[]
            {
                "number of (apportioned) entries into the room for the month",
                "apportioned hours spent in room during the month",
                "apportioned days spent in room during the month",
                "apportioned months spent in room during the month",
                "one of None, Hourly, Daily, or Monthly",
                "cost per room entry",
                "cost per Hour, Day, or Month"
            };

            ToolNotes = new string[]
            {
                "number of uses of the tool per reservation",
                "number of hours reserved",
                "actual reservation duration",
                "time used beyond scheduled end time",
                "true/false - was reservation started by user",
                "apportioned days reserved on this tool",
                "apportioned months reserved on this tool",
                "one of None, Hourly, Daily, or Monthly",
                "cost per reservation",
                "cost per Hour, Day, or Month"
            };

            StoreInvNotes = new string[]{
                "summed cost of all items of a given type",
                "rebate amount that applied to the purchase"
            };

            StoreJENotes = new string[]
            {
                "summed cost of all items of a given type",
                "rebate amount that applied to the purchase"
            };

            aryNotes = new string[][]{
                RoomNotes,
                ToolNotes,
                StoreInvNotes,
                StoreJENotes
            };
        }

        private void AddAuxParamToArray(int idxAry, string AuxCostParm, string parmType, string Description)
        {
            int baseLen = aryVars[idxAry].Length;
            Array.Resize(ref aryVars[idxAry], baseLen + 1);
            Array.Resize(ref aryVarTypes[idxAry], baseLen + 1);
            Array.Resize(ref aryVarFormats[idxAry], baseLen + 1);
            Array.Resize(ref aryNotes[idxAry], baseLen + 1);
            aryVars[idxAry][baseLen] = AuxCostParm + parmType;
            aryVarTypes[idxAry][baseLen] = "Double";
            aryVarFormats[idxAry][baseLen] = "{0:#.0}";
            aryNotes[idxAry][baseLen] = Description;
        }

        private void CompileCode(int selType, string formulaText)
        {
            int i = 0;
            StringBuilder sbFunction = new StringBuilder();

            var _with1 = sbFunction;
            _with1.Append("Imports Microsoft.VisualBasic.Interaction");
            _with1.Append(Environment.NewLine);
            _with1.Append("Imports System.Math");
            _with1.Append(Environment.NewLine);
            _with1.Append("Public Class clCalcCost");
            _with1.Append(Environment.NewLine);
            _with1.Append("Public Function fnCalcCost(");
            for (i = 0; i <= aryVars[selType].Length - 1; i++)
            {
                _with1.Append("ByVal " + aryVars[selType][i] + " As " + aryVarTypes[selType][i]);
                if (i != aryVars[selType].Length - 1)
                    _with1.Append(", ");
            }
            _with1.Append(") As Double");
            _with1.Append(Environment.NewLine);
            _with1.Append("Dim calcCost as Double");
            _with1.Append(Environment.NewLine);
            _with1.Append(formulaText);
            _with1.Append(Environment.NewLine);
            _with1.Append("Return calcCost");
            _with1.Append(Environment.NewLine);
            _with1.Append("End Function");
            _with1.Append(Environment.NewLine);
            _with1.Append("End Class");

            //2008-07-24 the string builder looks like this
            //Imports System.Math

            //Public Class clCalcCost
            //	Public Function fnCalcCost(ByVal Entries As Double, ByVal Hours As Double, ByVal Days As Double, ByVal Months As Double, ByVal CostPeriod As String, ByVal PerEntry As Double, ByVal PerPeriod As Double, ByVal RoomCapCostAdd As Double) As Double
            //		Dim calcCost As Double
            //		calcCost = 0.0
            //		If (CostPeriod = "Monthly") Then
            //			calcCost = PerPeriod * Months
            //		ElseIf (CostPeriod = "Hourly") Then
            //			calcCost = PerPeriod * Hours + PerEntry * Entries
            //		End If
            //		Return calcCost
            //	End Function
            //End Class

            // code from: http://www.codeproject.com/vb/net/DotNetCompilerArticle.asp
            // Need to do a two part test:
            //  Step 1 - check compilation
            //  Step 2 - run function and show results
            Microsoft.VisualBasic.VBCodeProvider vbProvider = new Microsoft.VisualBasic.VBCodeProvider();
            //Dim compiler As System.CodeDom.Compiler.ICodeCompiler
            CompilerResults results = null;

            CompilerParameters vbParams = new CompilerParameters();
            vbParams.ReferencedAssemblies.Add("System.dll");
            vbParams.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
            vbParams.ReferencedAssemblies.Add("mscorlib.dll");
            vbParams.GenerateInMemory = true;
            //Assembly is created in memory
            vbParams.TreatWarningsAsErrors = false;
            vbParams.WarningLevel = 4;

            //compiler = provider.CreateCompiler
            results = vbProvider.CompileAssemblyFromSource(vbParams, sbFunction.ToString());

            if (results.Errors.Count > 0)
            {
                StringBuilder sbError = new StringBuilder();
                sbError.Append("Error compiling cost function" + Environment.NewLine);
                foreach (System.CodeDom.Compiler.CompilerError err in results.Errors)
                {
                    var _with2 = sbError;
                    _with2.Append("Line " + err.Line.ToString() + ", Col " + err.Column.ToString() + ": Error " + err.ErrorNumber.ToString() + " - " + err.ErrorText);
                    _with2.Append("<br>");
                }
                throw new Exception(sbError.ToString());
            }

            mAssembly = results.CompiledAssembly;
        }

        private double CalcSingleItem(int selType, DataRow dr)
        {
            if (scriptType == null)
            {
                scriptType = mAssembly.GetType("clCalcCost");
                //Get the type from the assembly.  This will allow us access to all the properties and methods.
                instance = mAssembly.CreateInstance("clCalcCost");
                //Create an instance of my object
            }

            //Set up an array of objects to pass as arguments.
            object[] args = null;
            args = new object[aryVars[selType].GetUpperBound(0) + 1];

            object objVar = null;
            object objTemp = null;
            for (int i = 0; i <= aryVars[selType].GetUpperBound(0); i++)
            {
                objVar = new object();
                objTemp = dr[aryVars[selType][i]];
                switch (aryVarTypes[selType][i])
                {
                    case "Double":
                        // have to do this ugly because VB doesn't short-circuit
                        if (objTemp == DBNull.Value)
                        {
                            objVar = 0.0;
                        }
                        else
                        {
                            objVar = Convert.ToDouble(objTemp);
                        }
                        break;
                    case "String":
                        if (objTemp == DBNull.Value)
                        {
                            objVar = "";
                        }
                        else
                        {
                            objVar = Convert.ToString(objTemp);
                        }
                        break;
                    case "Boolean":
                        if (objTemp == DBNull.Value)
                        {
                            objVar = false;
                        }
                        else
                        {
                            objVar = Convert.ToBoolean(objTemp);
                        }
                        break;
                }

                args[i] = objVar;
            }

            //And call the non-static function
            object rslt = scriptType.InvokeMember("fnCalcCost", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Instance, null, instance, args);

            if (rslt == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToDouble(rslt);
                //Return value is an object, cast it back to a double
            }
        }

        public DataTable CalcCost(string type, string formula, string costFilter, int costFilterValue, DateTime period, int recordId, int clientId, AggType agg = AggType.None, bool exp = false, string tableNamePrefix = "")
        {
            // strange, but makes things consistent
            if (type == "Misc")
            {
                var dtMisc = DefaultDataCommand.Create()
                    .Param("Action", "ByPeriod")
                    .Param("Period", period)
                    .Param(costFilter, !string.IsNullOrEmpty(costFilter), costFilterValue)
                    .FillDataTable("dbo.ExternalMiscExp_Select");

                // leave the description column
                if (dtMisc.Rows.Count > 0 && agg != AggType.None)
                {
                    dtMisc.Columns.Remove(dtMisc.Columns["ExpID"]);
                    dtMisc.Columns["CalcCost"].ColumnName = "TotalCalcCost";
                }

                return dtMisc;
            }
            else
            {
                string CostType = "";
                int fTypeKey = 0;
                switch (type)
                {
                    case "Room":
                        CostType = "Room";
                        fTypeKey = 0;
                        break;
                    case "Tool":
                        CostType = "Tool";
                        fTypeKey = 1;
                        break;
                    case "StoreInv":
                        CostType = "Store";
                        fTypeKey = 2;
                        break;
                    case "StoreJE":
                        //From FinOps's Material JE
                        CostType = "Store";
                        fTypeKey = 3;
                        break;
                }

                //4 tables will be returned from the code below
                //1st table - all instance of client order history in a specific month
                //2nd table - Client table - list all clients who had ordered in a specific month
                //3rd table - table contains three columns - ClientID, DebitAccountID, CreditAccountID
                //4th table - one column called colName, two rows, CalcCost, ItemCost
                //5th table - One column called CapCost, just one row value = 0
                DataTable dt;
                DataTable dtClient;
                DataTable dtCliAcctType;
                DataTable dtAggCols;

                var ds = DefaultDataCommand.Create()
                    .Timeout(300)
                    .Param("CostType", CostType)
                    .Param("Period", period)
                    .Param(costFilter, !string.IsNullOrEmpty(costFilter), costFilterValue)
                    .Param("RecordID", recordId > 0, recordId)
                    .Param("ClientID", clientId > 0, clientId)
                    .Param("CostToday", exp, true)
                    .Param("PartialAgg", agg == AggType.CliAcctType, true)
                    .FillDataSet($"dbo.{tableNamePrefix}CostData_Select");

                dt = ds.Tables[0].Copy();

                //dt table for room type has 16 columns as below
                //0	"ClientID"	
                //1	"AccountID"	
                //2	"RoomID"	
                //3	"Entries"	
                //4	"Hours"	
                //5	"Days"	
                //6	"Months"	
                //7	"Period"	
                //8	"CostPeriod"	
                //9	"PerEntry"	
                //10	"PerPeriod"	
                //11	"ChargeTypeID"	
                //12	"RoomCapCostAdd"
                //13	"RoomCapCostMul"
                //14	"CalcCost"	
                //15	"BillingType"	

                //dt table for tool type has 26 columns as below
                //0  ClientID
                //1  AccountID
                //2  ResourceID
                //3  Uses
                //4  SchedDuration
                //5  ActDuration
                //6  OverTime
                //7  Days
                //8  Months
                //9  IsStarted
                //10 ToolChargeMultiplierMul
                //11 Period
                //12 CostPeriod
                //13 PerUse
                //14 PerPeriod
                //15 ChargeTypeID
                //16 ToolCapCostAdd
                //17 ToolCapCostMul
                //18 ToolCreateReservCostAdd
                //19 ToolCreateReservCostMul
                //20 ToolMissedReservCostAdd
                //21 ToolMissedReservCostMul
                //22 ToolOvertimeCostAdd
                //23 ToolOvertimeCostMul
                //24 CalcCost
                //25 IsStarted1
                //26 BillingType

                //It will be true only when no one orders anything in that specific month
                if (dt.Rows.Count == 0)
                    return dt;

                dtClient = ds.Tables[1].Copy();
                dtCliAcctType = ds.Tables[2].Copy();
                dtAggCols = ds.Tables[3].Copy();
                CapCost = Convert.ToDouble(ds.Tables[4].Rows[0]["CapCost"]);

                // get formula from DB
                if (formula.Length == 0)
                {
                    formula = DefaultDataCommand.Create()
                        .Param(new { FormulaType = type, sDate = period, CostToday = exp })
                        .ExecuteScalar<string>($"dbo.{tableNamePrefix}CostFormula_Select").Value;
                }

                CompileCode(fTypeKey, formula);

                // will barf if CalcSingleItem returns nothing - needs to be cleaned up
                //Every single row in the first table has CalcCost as column to hold the newly calculated value
                double lineTotalCost = 0.0;
                foreach (DataRow dr in dt.Rows)
                {
                    lineTotalCost = CalcSingleItem(fTypeKey, dr);

                    //2008-09-05 If it's tool cost, we also have to consider the forgiven charges
                    if (fTypeKey == 1)
                        dr["CalcCost"] = lineTotalCost * Convert.ToDouble(dr["ToolChargeMultiplierMul"]);
                    else
                        dr["CalcCost"] = lineTotalCost;
                }

                // to allow calling this again
                mAssembly = null;
                scriptType = null;
                instance = null;

                // when agging, the only thing of true interest is the summed cost
                if (agg == AggType.None)
                    return dt;
                else
                {
                    //Create a new table dtRet with all columns of dtCliAcctType table and dtAggCols's rows (CalcCost, ItemCost)
                    //This code is execuated whenever aggregate data is needed - and it's all grouped by the columns in dtCliAcctType
                    DataRow ndr = default(DataRow);
                    DataTable dtRet = new DataTable();

                    foreach (DataColumn dc in dtCliAcctType.Columns)
                    {
                        dtRet.Columns.Add(dc.ColumnName, typeof(int));
                    }

                    foreach (DataRow dr in dtAggCols.Rows)
                    {
                        dtRet.Columns.Add("Total" + dr["colName"].ToString(), typeof(double));
                    }

                    string strCompute = null;
                    foreach (DataRow drCliAcctType in dtCliAcctType.Rows)
                    {
                        strCompute = "";
                        ndr = dtRet.NewRow();
                        foreach (DataColumn dc in dtCliAcctType.Columns)
                        {
                            ndr[dc.ColumnName] = drCliAcctType[dc.ColumnName];
                            if (strCompute.Length > 0)
                                strCompute += " AND ";
                            strCompute += dc.ColumnName + "=" + drCliAcctType[dc.ColumnName].ToString();
                        }

                        foreach (DataRow dr in dtAggCols.Rows)
                        {
                            ndr["Total" + dr["colName"].ToString()] = dt.Compute("sum(" + dr["colName"].ToString() + ")", strCompute);
                        }
                        dtRet.Rows.Add(ndr);
                    }

                    return dtRet;
                }
            }
        }

        public DataTable CalcCost2(string type, string formula, string costFilter, int costFilterValue, DateTime period, int recordId, int clientId, AggType agg = AggType.None, bool exp = false, string tableNamePrefix = "")
        {
            // strange, but makes things consistent
            if (type == "Misc")
            {
                var dtMisc = DefaultDataCommand.Create()
                        .Param("Action", "ByPeriod")
                        .Param("Period", period)
                        .Param(costFilter, !string.IsNullOrEmpty(costFilter), costFilterValue)
                        .FillDataTable("dbo.ExternalMiscExp_Select");

                // leave the description column
                if (dtMisc.Rows.Count > 0 && agg != AggType.None)
                {
                    dtMisc.Columns.Remove(dtMisc.Columns["ExpID"]);
                    dtMisc.Columns["CalcCost"].ColumnName = "TotalCalcCost";
                }

                return dtMisc;
            }
            else
            {
                string costType = "";
                int typeKey = 0;
                switch (type)
                {
                    case "Room":
                        costType = "Room";
                        typeKey = 0;
                        break;
                    case "Tool":
                        costType = "Tool";
                        typeKey = 1;
                        break;
                    case "StoreInv":
                        costType = "Store";
                        typeKey = 2;
                        break;
                    case "StoreJE":
                        //From FinOps's Material JE
                        costType = "Store";
                        typeKey = 3;
                        break;
                }

                //4 tables will be returned from the code below
                //1st table - all instance of client order history in a specific month
                //2nd table - Client table - list all clients who had ordered in a specific month
                //3rd table - table contains three columns - ClientID, DebitAccountID, CreditAccountID
                //4th table - one column called colName, two rows, CalcCost, ItemCost
                //5th table - One column called CapCost, just one row value = 0
                DataTable dt;
                DataTable dtClient;
                DataTable dtCliAcctType;
                DataTable dtAggCols;

                var ds = DefaultDataCommand.Create()
                    .Param("CostType", costType)
                    .Param("Period", period)
                    .Param(costFilter, !string.IsNullOrEmpty(costFilter), costFilterValue)
                    .Param("RecordID", recordId > 0, recordId)
                    .Param("ClientID", clientId > 0, clientId)
                    .Param("CostToday", exp, true)
                    .Param("PartialAgg", agg == AggType.CliAcctType, true)
                    .FillDataSet($"dbo.{tableNamePrefix}CostData_Select2");

                dt = ds.Tables[0].Copy();

                //dt table for room type has 16 columns as below
                //0	"ClientID"	
                //1	"AccountID"	
                //2	"RoomID"	
                //3	"Entries"	
                //4	"Hours"	
                //5	"Days"	
                //6	"Months"	
                //7	"Period"	
                //8	"CostPeriod"	
                //9	"PerEntry"	
                //10	"PerPeriod"	
                //11	"ChargeTypeID"	
                //12	"RoomCapCostAdd"
                //13	"RoomCapCostMul"
                //14	"CalcCost"	
                //15	"BillingType"	

                //dt table for tool type has 26 columns as below
                //0  ClientID
                //1  AccountID
                //2  ResourceID
                //3  Uses
                //4  SchedDuration
                //5  ActDuration
                //6  OverTime
                //7  Days
                //8  Months
                //9  IsStarted
                //10 ToolChargeMultiplierMul
                //11 Period
                //12 CostPeriod
                //13 PerUse
                //14 PerPeriod
                //15 ChargeTypeID
                //16 ToolCapCostAdd
                //17 ToolCapCostMul
                //18 ToolCreateReservCostAdd
                //19 ToolCreateReservCostMul
                //20 ToolMissedReservCostAdd
                //21 ToolMissedReservCostMul
                //22 ToolOvertimeCostAdd
                //23 ToolOvertimeCostMul
                //24 CalcCost
                //25 IsStarted1
                //26 BillingType

                //It will be true only when no one orders anything in that specific month
                if (dt.Rows.Count == 0)
                    return dt;

                dtClient = ds.Tables[1].Copy();
                dtCliAcctType = ds.Tables[2].Copy();
                dtAggCols = ds.Tables[3].Copy();
                CapCost = Convert.ToDouble(ds.Tables[4].Rows[0]["CapCost"]);

                // get formula from DB
                if (formula.Length == 0)
                {
                    formula = DefaultDataCommand.Create()
                        .Param(new { FormulaType = type, sDate = period, CostToday = exp })
                        .ExecuteScalar<string>($"dbo.{tableNamePrefix}CostFormula_Select2").Value;
                }

                CompileCode(typeKey, formula);

                // will barf if CalcSingleItem returns nothing - needs to be cleaned up
                //Every single row in the first table has CalcCost as column to hold the newly calculated value
                double lineTotalCost = 0.0;
                foreach (DataRow dr in dt.Rows)
                {
                    lineTotalCost = CalcSingleItem(typeKey, dr);

                    //2008-09-05 IF it's tool cost, we also have to consider the forgiven charges
                    if (typeKey == 1)
                    {
                        dr["CalcCost"] = lineTotalCost * Convert.ToDouble(dr["ToolChargeMultiplierMul"]);
                    }
                    else
                    {
                        dr["CalcCost"] = lineTotalCost;
                    }
                }

                // to allow calling this again
                mAssembly = null;
                scriptType = null;
                instance = null;

                // when agging, the only thing of true interest is the summed cost
                if (agg == AggType.None)
                    return dt;
                else
                {
                    //Create a new table dtRet with all columns of dtCliAcctType table and dtAggCols's rows (CalcCost, ItemCost)
                    //This code is execuated whenever aggregate data is needed - and it's all grouped by the columns in dtCliAcctType
                    DataRow ndr = null;
                    DataTable dtRet = new DataTable();
                    foreach (DataColumn dc in dtCliAcctType.Columns)
                    {
                        dtRet.Columns.Add(dc.ColumnName, typeof(int));
                    }
                    foreach (DataRow dr in dtAggCols.Rows)
                    {
                        dtRet.Columns.Add("Total" + dr["colName"].ToString(), typeof(double));
                    }

                    string strCompute = null;
                    foreach (DataRow drCliAcctType in dtCliAcctType.Rows)
                    {
                        strCompute = "";
                        ndr = dtRet.NewRow();
                        foreach (DataColumn dc in dtCliAcctType.Columns)
                        {
                            ndr[dc.ColumnName] = drCliAcctType[dc.ColumnName];
                            if (strCompute.Length > 0)
                                strCompute += " AND ";
                            strCompute += dc.ColumnName + "=" + drCliAcctType[dc.ColumnName].ToString();
                        }

                        foreach (DataRow dr in dtAggCols.Rows)
                        {
                            ndr["Total" + dr["colName"].ToString()] = dt.Compute("sum(" + dr["colName"].ToString() + ")", strCompute);
                        }
                        dtRet.Rows.Add(ndr);
                    }

                    return dtRet;
                }
            }
        }

        public double EstimateToolRunCost(int accountId, int resourceId, double duration)
        {
            DataTable dt = DefaultDataCommand.Create()
                .Param(new { AccountID = accountId, ResourceID = resourceId, Duration = duration })
                .FillDataTable("dbo.ToolCost_Estimate");

            string formula = DefaultDataCommand.Create()
                .Param(new { FormulaType = "Tool", sDate = DateTime.Now.Date })
                .ExecuteScalar<string>("dbo.CostFormula_Select").Value;

            CompileCode(1, formula);

            // will barf if CalcSingleItem returns nothing - needs to be cleaned up
            DataRow dr = dt.AsEnumerable().FirstOrDefault();
            if (dr != null)
                return CalcSingleItem(1, dr);
            else
                throw new Exception("CalcSingleItem returned nothing");
        }
    }
}