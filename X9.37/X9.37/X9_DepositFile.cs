using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace X9_37
{   
    public class X9_DepositFile
    {
        #region Member Variables
        public Dictionary<FileHeaderRecord, FileControlRecord> records { get; set; }
        public List<X9_Deposit> deposits { get; set; }        
        public FileHeaderRecord fileHeader { get; set; }
        public FileControlRecord fileControl { get; set; }
        private static X9_DepositFile depositFile;  //Singleton
        #endregion

        public X9_DepositFile() 
        {
            deposits = new List<X9_Deposit>();       
            #if DEBUG
                fileHeader = new FileHeaderRecord('T', 'N', "044000037");
            #else
                fileHeader = new FileControlRecord('P','N',"044000037");              
            #endif
                records = new Dictionary<FileHeaderRecord, FileControlRecord>();

                fileHeader = new FileHeaderRecord('T', 'N', "044000037");
                fileControl = new FileControlRecord(1, 14, 0, 0);
                addRecords();
        }

        public X9_DepositFile Instance
        {
            get 
            {
                if (depositFile == null) 
                {
                    depositFile = new X9_DepositFile();
                }
                return depositFile;
            }
        }

        public void addDeposits(List<X9_Deposit> inDeposits) 
        {
            foreach (X9_Deposit d in inDeposits)
                deposits.Add(d);            
        }

        public void addRecords() 
        {
            records.Add(fileHeader, fileControl);            
        }

        public void createDepositFile(string filename) 
        {   
            String X9String = printRecords();            
            FileInfo file = new FileInfo(filename);
            StreamWriter sWriter = file.AppendText();
            sWriter.Write(X9String);
            sWriter.Flush();
            sWriter.Close();            
        }

        public string printRecords()
        {
            string recordString = "";
            try
            {
                foreach (KeyValuePair<FileHeaderRecord, FileControlRecord> record in records)
                {
                    //recordString += "==========DEPOSIT FILE==============\r\n";                        
                    recordString += record.Key.GenerateHeaderString();
                    foreach (X9_Deposit d in deposits)
                        recordString += d.printRecords(ref recordString);
                    recordString += record.Value.GenerateHeaderString();
                }

            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return recordString;
        }
        
        public byte[] convertToASCIIcharacter(string input)
        {
            System.Text.ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] character = ascii.GetBytes(input);
            ASCII2EBCDIC_Char(ref character);

            return character;
        }

        public void ASCII2EBCDIC_Char(ref byte[] input)
        {
            int[] A2E = new int[128]{0,1,2,3,55,45,46,47,22,5,37,11,12,13,14,15,
                                   16,17,18,19,60,61,50,38,24,25,63,39,28,29,30,31,
                                   64,79,127,123,91,108,80,125,77,93,92,78,107,96,75,97,
                                   240,241,242,243,244,245,246,247,248,249,122,94,76,126,110,111,
                                   124,193,194,195,196,197,198,199,200,201,209,210,211,212,213,214,
                                   215,216,217,226,227,228,229,230,231,232,233,74,224,90,95,109,
                                   121,129,130,131,132,133,134,135,136,137,145,146,147,148,149,150,
                                   151,152,153,162,163,164,165,166,167,168,169,192,106,208,161,7};

            for (int counter = 0; counter < input.Length; counter++)
            {
                byte temp = (byte)A2E[input[counter]];
                input[counter] = temp;
            }
        }

        public void EBCDIC2ASCII(ref byte[] input)
        {
            int[] E2A = new int[256] {0,1,2,3,26,9,26,241,26,26,26,11,12,13,14,15,
                                    16,17,18,19,26,26,8,26,24,25,26,26,28,29,30,
                                    31,26,26,26,26,26,10,23,27,26,26,26,26,26,5,6,7,
                                    26,26,22,26,26,26,26,4,26,26,26,26,20,21,26,26,
                                    32,26,26,26,26,26,26,26,26,26,91,46,60,40,43,33,
                                    38,26,26,26,26,26,26,26,26,26,93,36,42,41,59,94,
                                    45,47,26,26,26,26,26,26,26,26,124,44,37,95,62,63,
                                    26,26,26,26,26,26,26,26,26,96,58,35,64,39,61,34,
                                    26,97,98,99,100,101,102,103,104,105,26,26,26,26,26,26,
                                    26,106,107,108,109,110,111,112,113,114,26,26,26,26,26,26,
                                    26,126,115,116,117,118,119,120,121,122,26,26,26,26,26,26,
                                    26,26,26,26,26,26,26,26,26,26,26,26,26,26,26,26,
                                    123,65,66,67,68,69,70,71,72,73,26,26,26,26,26,26,
                                    125,74,75,76,77,78,79,80,81,82,26,26,26,26,26,26,
                                    92,26,83,84,85,86,87,88,89,90,26,26,26,26,26,26,
                                    48,49,50,51,52,53,54,55,56,57,26,26,26,26,26,26};

            for (int counter = 0; counter < input.Length; counter++)
            {
                input[counter] = (byte)E2A[input[counter]];
            }
        }
        
    }
}
