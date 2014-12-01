using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;


namespace X9_37
{
    public class X9_DepositItem
    {
        
        #region Member Variables
        public Image checkimage { get; set; }
        public String checkImageLocation = "";
        public List<Record> records { get; set; }
        public CheckDetailRecord checkDetail { get; set; }
        public ImageViewDataRecord imageViewDataBack { get; set; }
        public ImageViewDataRecord imageViewDataFront { get; set; }
        public ImageViewDetailRecord imageViewDetailBack { get; set; }
        public ImageViewDetailRecord imageViewDetailFront { get; set; }
        public ImageViewAnalysisRecord imageViewAnalysisBack { get; set; }
        public ImageViewAnalysisRecord imageViewAnalysisFront { get; set; }
        
        #endregion
               
        public X9_DepositItem(int seqNum, Image image= null, string imageLocation="") 
        {            
            checkDetail = new CheckDetailRecord();            
            imageViewDataBack = new ImageViewDataRecord();
            imageViewDataFront = new ImageViewDataRecord();
            imageViewDetailBack = new ImageViewDetailRecord();
            imageViewDetailFront = new ImageViewDetailRecord();
            imageViewAnalysisBack = new ImageViewAnalysisRecord();
            imageViewAnalysisFront = new ImageViewAnalysisRecord();
            records = new List<Record>();
            imageViewDataFront.ImageData = image;
            imageViewDataBack.ImageData = image;
            checkImageLocation = imageLocation;

            checkDetail = new CheckDetailRecord("044000037", "", "", "0",seqNum.ToString());
            //FRONT OF CHECK
            imageViewDetailFront = new ImageViewDetailRecord('0', DateTime.Now);
            imageViewDataFront = new ImageViewDataRecord(seqNum.ToString(), image, imageLocation);
            //BACK OF CHECK            
            imageViewDetailBack = new ImageViewDetailRecord('0', DateTime.Now);
            imageViewDataBack = new ImageViewDataRecord(seqNum.ToString(), image, imageLocation);
            addRecords();

        }

        public void addRecords()
        {
            records.Add(checkDetail);
            records.Add(imageViewDetailBack);
            records.Add(imageViewDetailFront);
            records.Add(imageViewDataBack);
            records.Add(imageViewDataFront);            
            records.Add(imageViewAnalysisBack);    
            records.Add(imageViewAnalysisFront);
        }

        public string printRecords(ref string recordString) 
        {            
            try
            {
                foreach (Record record in records)
                {
                    recordString += record.GenerateHeaderString();

                    if (record is ImageViewDataRecord)
                    {
                        //recordString += "==========IMAGE DATA==============\r\n";                        
                        writeToFile(recordString);                        
                        //pushByteDataToFile(((ImageViewDataRecord)record).GetImageByteArray());
                        
                        copyBinaryToX9();
                        recordString = "";
                    }
                }
                
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return recordString; 
        }

        public void writeToFile(string writeInfo)
        {
            FileInfo file = new FileInfo("C:\\Users\\jjv\\Desktop\\Deposit.X9");            
            StreamWriter sWriter = file.AppendText();            
            sWriter.Write(writeInfo);
            sWriter.Flush();
            sWriter.Close();
        }

        public void pushByteDataToFile(byte[] imageData) 
        {
            try
            {                
                string filename = "C:\\Users\\jjv\\Desktop\\Deposit.X9";
                FileStream _FileStream = new FileStream(filename, FileMode.Append, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(_FileStream);
                //_FileStream.Write(imageData, 0, imageData.Length);                
                bw.Write(imageData);
                _FileStream.Close();
            }
            catch (Exception e)
            {                
                Console.WriteLine(e.Message);
            }

        }

        public void copyBinaryToX9() 
        {
            using (FileStream stream = File.OpenRead(checkImageLocation))
            using (FileStream writeStream = new FileStream("C:\\Users\\jjv\\Desktop\\Deposit.X9", FileMode.Append, FileAccess.Write))
            {
                BinaryReader reader = new BinaryReader(stream);
                BinaryWriter writer = new BinaryWriter(writeStream);

                byte[] buffer = new Byte[1024];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, 1024)) > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}
