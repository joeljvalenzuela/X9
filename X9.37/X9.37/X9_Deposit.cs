using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X9_37
{
    public class X9_Deposit
    {
        #region Member Variables
        public List<X9_Bundle> bundles { get; set; }
        public Dictionary<CashLetterHeaderRecord, CashLetterControlRecord> records { get; set; }
        public CashLetterHeaderRecord cashLetterHeader { get; set; }
        public CashLetterControlRecord cashLetterControl { get; set; }
        #endregion

        public X9_Deposit() 
        {
            bundles = new List<X9_Bundle>();
            cashLetterHeader = new CashLetterHeaderRecord();
            cashLetterControl = new CashLetterControlRecord();
            records = new Dictionary<CashLetterHeaderRecord, CashLetterControlRecord>();

            cashLetterHeader = new CashLetterHeaderRecord(1);
            cashLetterControl = new CashLetterControlRecord(1, 0, 0, 1);
            addRecords();
        }

        public void addBundles(List<X9_Bundle> inBundles) 
        {            
            foreach (X9_Bundle b in inBundles)
                bundles.Add(b);
        }

        public void addRecords() 
        {
            records.Add(cashLetterHeader, cashLetterControl);            
        }
        
        public void addRecords(CashLetterHeaderRecord clHeader, CashLetterControlRecord clControl)
        {
            records.Add(clHeader, clControl);
        }

        public string printRecords(ref string recordString)
        {            
            string bundleStrings = "";
            try
            {
                foreach (KeyValuePair<CashLetterHeaderRecord, CashLetterControlRecord> record in records)
                {
                    //recordString += "==========DEPOSIT==============\r\n";                        
                    recordString += record.Key.GenerateHeaderString();
                    
                    foreach (X9_Bundle b in bundles)
                        bundleStrings = b.printRecords(ref recordString);

                    recordString = bundleStrings + record.Value.GenerateHeaderString();
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return recordString;
        }
    }
}
