using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X9_37
{    
    public class X9_Bundle
    {
        #region Member Variables
        public BundleHeaderRecord bundleHeader { get; set;}
        public BundleControlRecord bundleControl {get; set;}
        public List<X9_DepositItem> depositItems { get; set; }
        public Dictionary<BundleHeaderRecord, BundleControlRecord> records { get; set; }
        #endregion
        
        public X9_Bundle() 
        {
            depositItems = new List<X9_DepositItem>();            
            bundleHeader = new BundleHeaderRecord("123456789", 1, 1);
            bundleControl = new BundleControlRecord(1, 0, 0);
            records = new Dictionary<BundleHeaderRecord, BundleControlRecord>();
            addRecords();
        }

        public void addDepositItems(List<X9_DepositItem> inDepositItems)
        {
            foreach (X9_DepositItem d in inDepositItems)
                depositItems.Add(d);
        }

        public void addRecords() 
        {
            records.Add(bundleHeader, bundleControl);
        }

        public void addRecords(BundleHeaderRecord bHeader, BundleControlRecord bControl)
        {
            records.Add(bHeader, bControl);
        }

        public string printRecords(ref string recordString)
        {
            string depositStrings = "";
            try
            {
                foreach (KeyValuePair<BundleHeaderRecord, BundleControlRecord> record in records)
                {
                    //recordString += "==========BUNDLE==============\r\n";                        
                    recordString += record.Key.GenerateHeaderString();
                    int depositItemCount = 0;
                    foreach (X9_DepositItem d in depositItems)
                    {
                            depositStrings += d.printRecords(ref recordString);
                            depositItemCount++;
                    }
                    
                    recordString = depositStrings + record.Value.GenerateHeaderString();                    
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return recordString;
        }
    }
}
