using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace X9_37
{

    /// <summary>
    /// Record BaseRecordType Indicator. Used as first parameter
    /// of Header Field.
    /// </summary>
    public enum RecordType  
    {
        FileHeaderRecord_Type           = 01,
        CashLetterHeaderRecord_Type     = 10,
        BundleHeaderRecord_Type         = 20,
        CheckDetailRecord_Type          = 25,
        ImageViewDetailRecord_Type      = 50,
        ImageViewDataRecord_Type        = 52,
        ImageViewAnalysisRecord_Type    = 54,
        BundleControlRecord_Type        = 70,        
        CashLetterControlRecord_Type    = 90,
        FileControlRecord_Type          = 99,       

    }

    /// <summary>
    /// Generic Record Base Class
    /// </summary>
    public class Record
    {
        #region Member Variables
        public RecordType BaseRecordType { get; set; }
        public char[] CollectionTypeIndicator { get; set; }
        #endregion
        
        #region Constructors
        public Record()
        {
            CollectionTypeIndicator = new char[2];     
        }
        #endregion
        
        #region Methods
        public virtual String GenerateHeaderString() { return BaseRecordType.ToString("00") + " "; }
        
        public Byte[] HeaderToByteArray(String Header) { return Encoding.ASCII.GetBytes(Header); }
        
        public String GetRecordTypeString() { return Convert.ToInt32(BaseRecordType).ToString(); }        
        
        public Byte[] ImagetoByteArray(string fileLocation)
        {
            FileStream fStream = new FileStream(fileLocation, FileMode.Open);
            StreamReader sReader = new StreamReader(fStream);

            List<Byte> byteList = new List<Byte>();
            while (!sReader.EndOfStream)
            {
                char[] buffer = new char[54];
                sReader.Read();
            }

            Byte[] imageData = new Byte[54];
            return imageData;
        }

        public char[] FillArray(string input, char[] array, int length)
        {
            for(int i =0; i<length; i++)
                array[i] = input.ToCharArray()[i];
            return array;
        }

        public char[] FillArray(string input, int length)
        {
            char[] array = new char[length];

            for (int i = 0; i < length; i++)
            {
                if(i<input.Length)
                    array[i] = input.ToCharArray()[i];
                else
                    array[i] = ' ';
            }

            return array;
        }

        public char[] FillArrayBackEnd(string input, int length)
        {
            char[] array = new char[length];
            int j = input.Length-1;
            for (int i = length-1; i >0 ; i--)
            {
                if (i > input.Length)
                {
                    array[i] = input.ToCharArray()[j];
                    j--;
                }
                else
                    array[i] = ' ';
            }

            return array;
        }
        
        public string ImageToString(Image im)
        {
            MemoryStream ms = new MemoryStream();

            im.Save(ms, im.RawFormat);

            byte[] array = ms.ToArray();

            return Convert.ToBase64String(array);

        }

        public Image StringToImage(string imageString)
        {

            if (imageString == null)

                throw new ArgumentNullException("imageString");

            byte[] array = Convert.FromBase64String(imageString);

            Image image = Image.FromStream(new MemoryStream(array));

            return image;

        }

        #endregion
    }

    /// <summary>
    /// The File Header Record is mandatory. It is the first record of the file. If a corresponding File Control Record (BaseRecordType 99) is not present as the
    /// last record in this file, the file will be rejected. The data in the fields are created by the depositor sending the file, the immediate origin
    /// depositor.
    /// </summary>
    public class FileHeaderRecord           : Record    
    {
        #region HeaderFields
        public char Reserved                { get; set; }
        public char testFileIndicator       { get; set; }
        public char ResendIndicator         { get; set; }
        public char FileIdModifier          { get; set; }
        public char[] standardLevel         { get; set; }
        public char[] ImmDestRouteNumber    { get; set; }
        public char[] ImmOriginIdentifier   { get; set; }
        public char[] FileCreationDate      { get; set; }
        public char[] FileCreationTime      { get; set; }
        public char[] ImmDestName           { get; set; }
        public char[] ImmOriginName         { get; set; }
        public char[] CountryCode           { get; set; }
        public char[] UserField             { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Generic Constructor. Use Prefilled.
        /// </summary>
        public FileHeaderRecord() 
        {
            BaseRecordType = RecordType.FileHeaderRecord_Type;
            Reserved            = new char();   //Position: 14
            testFileIndicator   = new char();   //Position: 03
            ResendIndicator     = new char();   //Position: 08 
            FileIdModifier      = new char();   //Position: 11
            standardLevel       = new char[2];  //Position: 02 
            ImmDestRouteNumber  = new char[9];  //Position: 04
            ImmOriginIdentifier = new char[9];  //Position: 05 
            FileCreationDate    = new char[8];  //Position: 06
            FileCreationTime    = new char[4];  //Position: 07
            ImmDestName         = new char[18]; //Position: 09
            ImmOriginName       = new char[18]; //Position: 10
            CountryCode         = new char[2];  //Position: 12
            UserField           = new char[4];  //Position: 13        
        }

        /// <summary>
        /// Pre-Filled Constructor will create a File Header Record following
        /// JP Morgan Chase's specifications.
        /// </summary>
        /// <param name="testFile">Indicates the generated file is a test file. 'P'=Production 'T'=Test</param>
        /// <param name="resend">Indicates whether the file has been previously transmitted in its entirety. 
        /// <br />‘Y’= Resend File. File contains the same data as a previously sent file.
        /// <br />‘N’ original file – This is the original file.</param>        
        /// <param name="origRoute">Field contents will be specified by JPMorgan Chase and provided to customer.</param>
        public FileHeaderRecord(char testFile, char resend,string origRoute) 
        {
            BaseRecordType = RecordType.FileHeaderRecord_Type;
            
            testFileIndicator   = testFile;
            ResendIndicator     = resend;
            FileIdModifier      = ' ';
            standardLevel       = FillArray("03",2);
            ImmDestRouteNumber  = FillArray("044000037", 9);
            ImmOriginIdentifier = FillArray(origRoute, 9);
            FileCreationDate    = DateTime.Now.Date.ToString("yyyyMMdd").ToCharArray();
            FileCreationTime    = DateTime.Now.Date.ToString("HHmm").ToCharArray();
            ImmDestName         = FillArray("JPM Chase", 18);            
            ImmOriginName       = FillArray("ICLViaHealth", 18);
            CountryCode         = FillArray("  ",   2);
            UserField           = FillArray("    ", 4);
            Reserved            = ' ';
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generates header information for the specified record BaseRecordType
        /// </summary>
        /// <returns></returns>
        public override String GenerateHeaderString() 
        { 
            String header;
            header = ((int)BaseRecordType).ToString("00")   +
                      new string(standardLevel)         + 
                      testFileIndicator                 +
                      new string(ImmDestRouteNumber)    +
                      new string(ImmOriginIdentifier)   +
                      new string(FileCreationDate)      +
                      new string(FileCreationTime)      +
                      ResendIndicator                   +
                      new string(ImmDestName)           +
                      new string(ImmOriginName)         +
                      FileIdModifier                    +
                      new string(CountryCode)           +
                      new string(UserField)             +
                      Reserved;            
            return header;
        }
        #endregion
    }

    /// <summary>
    /// The Cash Letter Header Record is mandatory. It follows the File Header Record (BaseRecordType 01). The data in these fields will be created by the
    /// depositor.
    /// </summary>
    public class CashLetterHeaderRecord     : Record    
    {
        #region Header Fields
        public char CashLetterRecordType        { get; set; }
        public char CashLetterDocType           { get; set; }
        public char FedWorkType                 { get; set; }
        public char Reserved                    { get; set; }

        public char[] CollectionTypeIndicator   { get; set; }
        public char[] DestinationRoutingNumber  { get; set; }
        public char[] UniqueCustomerID          { get; set; }
        public char[] CashLetterBusinessDate    { get; set; }
        public char[] CashLetterCreationDate    { get; set; }
        public char[] CashLetterCreationTime    { get; set; }
        public char[] CashLetterID              { get; set; }
        public char[] OriginatorContactName     { get; set; }
        public char[] PhoneNumberOrULID         { get; set; }
        public char[] UserField                 { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Generic Constructor. Use Prefilled.
        /// </summary>
        public CashLetterHeaderRecord() 
        {
            BaseRecordType = RecordType.CashLetterHeaderRecord_Type;
            
            CashLetterRecordType        =   new char();    //Position: 08
            CashLetterDocType           =   new char();    //Position: 09
            FedWorkType                 =   new char();    //Position: 13
            Reserved                    =   new char();    //Position: 15

            CollectionTypeIndicator     =   new char[2];    //Position: 02
            DestinationRoutingNumber    =   new char[9];    //Position: 03
            UniqueCustomerID            =   new char[9];    //Position: 04
            CashLetterBusinessDate      =   new char[8];    //Position: 05
            CashLetterCreationDate      =   new char[8];    //Position: 06
            CashLetterCreationTime      =   new char[4];    //Position: 07
            CashLetterID                =   new char[8];    //Position: 10
            OriginatorContactName       =   new char[14];   //Position: 11
            PhoneNumberOrULID           =   new char[10];   //Position: 12        
            UserField                   =   new char[9];    //Position: 14
        }

        /// <summary>
        /// Pre-filled Constructor for Cash Letter Header Record will create a Record following
        /// JP Morgan Chase's specifications.
        /// </summary>
        /// <param name="cashLetterID">8 Char Code that uniquely identifies the 
        /// cash letter (deposit). Must be unique within a Cash Letter Business Date.</param>
        /// <param name="contactName">14 Char Name. Not required; shall be blank filled.</param>
        /// <param name="phone">10 Char Phone Number. REQUIRED</param>
        public CashLetterHeaderRecord(int cashLetterID, string contactName="", string phone="") 
        {
            BaseRecordType                = RecordType.CashLetterHeaderRecord_Type;            
            CashLetterRecordType          =   'I';
            CashLetterDocType             =   'G';
            FedWorkType                   =   ' ';
            Reserved                      =   ' ';

            CollectionTypeIndicator       =   FillArray("01",2);
            DestinationRoutingNumber      =   FillArray("044000037",9);
            UniqueCustomerID              =   FillArray("000000000",9);
            CashLetterBusinessDate        =   FillArray(DateTime.Now.Date.ToString("yyyyMMdd"), 8);
            CashLetterCreationDate        =   FillArray(DateTime.Now.Date.ToString("yyyyMMdd"), 8);
            CashLetterCreationTime        =   FillArray(DateTime.Now.ToString("HHmm"),4);
            CashLetterID                  =   FillArray(cashLetterID.ToString(), 8);
            OriginatorContactName         =   FillArray(contactName, 14);
            PhoneNumberOrULID             =   FillArray(phone, 10);
            UserField                     =   FillArray("  ", 2);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generates header information for the specified record BaseRecordType
        /// </summary>
        /// <returns></returns>
        public override String GenerateHeaderString() 
        {
            String header;
            header =  ((int)BaseRecordType).ToString("00")  +
                      new string(CollectionTypeIndicator)   +
                      new string(DestinationRoutingNumber)  +
                      new string(UniqueCustomerID)          +
                      new string(CashLetterBusinessDate)    +
                      new string(CashLetterCreationDate)    +
                      new string(CashLetterCreationTime)    +
                      CashLetterRecordType                  +
                      CashLetterDocType                     +
                      new string(CashLetterID)              +
                      new string(OriginatorContactName)     +
                      new string(PhoneNumberOrULID)         +
                      FedWorkType                           +
                      new string(UserField)                 +
                      Reserved                  ;
            return header;
        }
        #endregion
    }

    /// <summary>
    /// The Bundle Header Record is mandatory. It follows the File Header Record (BaseRecordType 10). The data in these fields will be created by the
    /// depositor.
    /// </summary>
    public class BundleHeaderRecord         : Record    
    {
        #region Header Fields
        public char[] CollectionTypeIndicator     {get; set;}
        public char[] DestinationRoutingNumber    {get; set;}
        public char[] UniqCustomerIdentifier      {get; set;}
        public char[] BundleBusinessDate          {get; set;}
        public char[] BundleCreationDate          {get; set;}
        public char[] BundleID                    {get; set;}
        public char[] BundleSequenceNumber        {get; set;}
        public char[] CycleNumber                 {get; set;}
        public char[] ReturnLocationRoutingNumber {get; set;}
        public char[] UserField                   {get; set;}
        public char[] Reserved                    {get; set;}
        #endregion

        #region Constructors
        /// <summary>
        /// Generic Constructor. Use Prefilled.
        /// </summary>
        public BundleHeaderRecord() 
        {
            BaseRecordType = RecordType.BundleHeaderRecord_Type;
            CollectionTypeIndicator     = new char[2];      //Position: 01
            DestinationRoutingNumber    = new char[9];      //Position: 02
            UniqCustomerIdentifier      = new char[9];      //Position: 03
            BundleBusinessDate          = new char[8];      //Position: 04
            BundleCreationDate          = new char[8];      //Position: 05
            BundleID                    = new char[10];     //Position: 06
            BundleSequenceNumber        = new char[4];      //Position: 07
            CycleNumber                 = new char[2];      //Position: 08
            ReturnLocationRoutingNumber = new char[9];      //Position: 09
            UserField                   = new char[5];      //Position: 10
            Reserved                    = new char[12];     //Position: 11
        }

        /// <summary>
        /// Pre-Filled Bundle Header Record will create a Record following
        /// JP Morgan Chase's specifications.
        /// </summary>
        /// <param name="customerID">9 Digit Number- Customer Identifier Number. To be provided</param>
        /// <param name="bundleID">10 Digit Number- A number that identifies the bundle, assigned by the
        /// depositor that created the bundle</param>
        /// <param name="bundleSeqNum"> 4 Digit Number- Indicates the relative position of the bundle within the
        /// deposit. This number usually starts with one and is incremented by 
        /// one for each Bundle Header Record in this deposit.</param>
        public BundleHeaderRecord(string customerID, int bundleID, int bundleSeqNum) 
        {
            BaseRecordType              = RecordType.BundleHeaderRecord_Type;
            CollectionTypeIndicator     = FillArray("01", 2);
            DestinationRoutingNumber    = FillArray("044000037",9);
            UniqCustomerIdentifier      = FillArray(customerID, 9);
            BundleBusinessDate          = FillArray(DateTime.Now.Date.ToString("yyyyMMdd"), 8);
            BundleCreationDate          = FillArray(DateTime.Now.Date.ToString("yyyyMMdd"), 8);
            BundleID                    = FillArray(bundleID.ToString(), 10);
            BundleSequenceNumber        = FillArray(bundleSeqNum.ToString(), 4);
            CycleNumber                 = FillArray("  ", 2);   //BLANK
            ReturnLocationRoutingNumber = FillArray("", 9);     //BLANK
            UserField                   = FillArray("", 5);     //BLANK
            Reserved                    = FillArray("", 12);    //BLANK
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generates header information for the specified record BaseRecordType
        /// </summary>
        /// <returns></returns>
        public override String GenerateHeaderString()
        {
            String header;
            header = ((int)BaseRecordType).ToString("00") +
                        new string(CollectionTypeIndicator)     +
                        new string(DestinationRoutingNumber)    +
                        new string(UniqCustomerIdentifier)      +
                        new string(BundleBusinessDate)          +
                        new string(BundleCreationDate)          +
                        new string(BundleID)                    +
                        new string(BundleSequenceNumber)        +
                        new string(CycleNumber)                 +
                        new string(ReturnLocationRoutingNumber) +
                        new string(UserField)                   +
                        new string(Reserved)                    ;
            return header;
        }
        #endregion
    }

    /// <summary>
    /// The Check Detail Record is mandatory. The data in these fields will be created by the depositor. If JPMorgan Chase receives an image
    /// deposit file from a merchant, the Check Detail Records (BaseRecordType 25) included in the image deposit file will be passed on to the paying bank
    /// as they were received by JPMorgan Chase.
    /// </summary>
    public class CheckDetailRecord          : Record    
    {
        #region Header Fields
        public char ExternalProcessingCode                {get; set;}
        public char PayorBankRoutingNumberChkDigit        {get; set;}
        public char DocumentationTypeIndicator            {get; set;}
        public char ElectricReturnAcceptanceIndicator     {get; set;}
        public char MICRValidIndicator                    {get; set;}
        public char BOFDIndicator                         {get; set;}
        public char CorrectionIndicator                   {get; set;}
        public char ArchiveTypeIndicator                  {get; set;}
        
        public char[] AuxiliaryOnUs                       {get; set;}
        public char[] PayorBankRoutingNumber              {get; set;}
        public char[] OnUs                                {get; set;}
        public char[] ItemAmount                          {get; set;}
        public char[] InstitutionItemSeqNumber            {get; set;}
        public char[] CheckDetailRecAddendumCount         {get; set;}
        #endregion

        #region Constructors
        /// <summary>
        /// Generic Constructor. Use Prefilled.
        /// </summary>
        public CheckDetailRecord() 
        {
            BaseRecordType = RecordType.CheckDetailRecord_Type;

            ExternalProcessingCode                = new char();       //Position: 02
            PayorBankRoutingNumberChkDigit        = new char();       //Position: 04
            DocumentationTypeIndicator            = new char();       //Position: 08
            ElectricReturnAcceptanceIndicator     = new char();       //Position: 09
            MICRValidIndicator                    = new char();       //Position: 10
            BOFDIndicator                         = new char();       //Position: 11
            CorrectionIndicator                   = new char();       //Position: 13
            ArchiveTypeIndicator                  = new char();       //Position: 14
        
            AuxiliaryOnUs                       = new char[15];     //Position: 01        
            PayorBankRoutingNumber              = new char[8];      //Position: 03        
            OnUs                                = new char[20];     //Position: 05
            ItemAmount                          = new char[10];     //Position: 06
            InstitutionItemSeqNumber            = new char[15];     //Position: 07        
            CheckDetailRecAddendumCount         = new char[2];      //Position: 12
        }
        
        /// <summary>
        /// Pre-Filled Check Detail Record will create a File Header Record following
        /// JP Morgan Chase's specifications.
        /// </summary>
        /// <param name="routingNum">A number that identifies the institution by or through
        /// which the item is payable. Shall represent the first 8 digits of the routing number</param>
        /// <param name="aux">A code used on commercial checks at the discretion of
        /// the payor bank. Mandatory if present on the MICR line -On-Us symbols on 
        /// MICR line should not be included -Dashes must be retained -Right-justify the data</param>
        /// <param name="onus">Mandatory if present on the MICR Line The On-Us field 
        /// of the MICR document is located between positions 14 and 31 of the MICR line 
        /// of the item. -Translate On-Us symbols to forward slashes "/" -Right-justify the data 
        /// -Retain "dashes" -May omit spaces -Blank-fill any unused positions -Format: NBSMOS 
        /// Numeric blank/special MICR On-Us</param>
        /// <param name="amount">The US dollar value of the check.</param>
        /// <param name="instItemSeqNum">This field is the depositing institution tracer information
        /// and should be supplied when making inquiries. This number should match the number endorsed 
        /// on the check.</param>
        /// <param name="processingCode">A code used for special purposes, also known as Position 44. 
        /// Mandatory if present on the MICR line.</param>
        /// <param name="elecReturn"> A code that indicates whether the institution that creates the
        /// Check Detail Record will or will not support electronic return processing</param>
        /// <param name="micrValid">A code that indicates whether any character in the MICR line 
        /// is unreadable, or, the On-Us field is missing from the Check Detail Record</param>
        /// <param name="corrInd">Indicator to identify whether and how the MICR line was repaired, 
        /// for fields other than Payor Bank RT and Amount 0=no repair 1=repaired</param>
        public CheckDetailRecord(string routingNum, string aux, string onus, string amount, string instItemSeqNum, 
            char processingCode=' ', char elecReturn=' ', char micrValid = ' ', char corrInd ='0') 
        {
            BaseRecordType                        = RecordType.CheckDetailRecord_Type;
            ExternalProcessingCode                = processingCode;
            PayorBankRoutingNumberChkDigit        = '0';       
            DocumentationTypeIndicator            = ' ';              //BLANK
            ElectricReturnAcceptanceIndicator     = elecReturn;
            MICRValidIndicator                    = micrValid;
            BOFDIndicator                         = 'N';
            CorrectionIndicator                   = corrInd;
            ArchiveTypeIndicator                  = ' ';              //BLANK
        
            AuxiliaryOnUs                       = FillArray(aux, 15);
            PayorBankRoutingNumber              = FillArray(routingNum, 8);        
            OnUs                                = FillArray(onus, 20);
            ItemAmount                          = FillArray(amount, 10);
            InstitutionItemSeqNumber            = FillArray(instItemSeqNum, 15);
            CheckDetailRecAddendumCount         = FillArray("0",2);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generates header information for the specified record BaseRecordType
        /// </summary>
        /// <returns></returns>
        public override String GenerateHeaderString()
        {
            String header;
            header = ((int)BaseRecordType).ToString("00") +
                     new string(AuxiliaryOnUs)                      +
                     ExternalProcessingCode                         +
                     new string(PayorBankRoutingNumber)             +
                     PayorBankRoutingNumberChkDigit                 +
                     new string(OnUs)                               +
                     new string(ItemAmount)                         +
                     new string(InstitutionItemSeqNumber)           +
                     DocumentationTypeIndicator                     +
                     ElectricReturnAcceptanceIndicator              +
                     MICRValidIndicator                             +
                     BOFDIndicator                                  +
                     new string(CheckDetailRecAddendumCount)        +
                     CorrectionIndicator                            +
                     ArchiveTypeIndicator                           ;
            return header;
        }
        #endregion
    }

    /// <summary>
    /// The Image View Detail Record is mandatory when the Cash Letter Documentation BaseRecordType Indicator (Field 9) in the Cash Letter Header
    /// Record (BaseRecordType 10) is 'G' or 'H'. The Image View Detail Record is one of two records (BaseRecordType 50 and BaseRecordType 52) that shall be used together to
    /// convey an image view associated with the related Check Detail Record (BaseRecordType 25). If an Image View Detail Record is present, then an
    /// Image View Data Record (BaseRecordType 52) shall be present. JPMorgan Chase requires both the front image and back image of the item. The
    /// front image will be provided first followed by the rear image of the item.
    /// When JPMorgan Chase receives an image deposit, the Image View Detail Records (BaseRecordType 50) included in the image deposit will be passed
    /// on to the paying bank as they were received by JPMorgan Chase.
    /// </summary>
    public class ImageViewDetailRecord      : Record    
    {
        #region Header Fields
        public char    ImageIndicator                    { get; set; }
        public char    ViewSideIndicator                 { get; set; }
        public char    DigitalSignatureIndicator         { get; set; }
        public char    ImageRecreateIndicator            { get; set; }
        public char[]  ImageDepositorsRoutingNumber      { get; set; }
        public char[]  ImagingDepositorsDate             { get; set; }
        public char[]  ImageViewFormatIndicator          { get; set; }
        public char[]  ImageViewCompressionAlgorithmID   { get; set; }
        public char[]  ImageViewDataSize                 { get; set; }
        public char[]  ViewDescriptor                    { get; set; }
        public char[]  DigitalSignatureMethod            { get; set; }
        public char[]  SecurityKeySize                   { get; set; }
        public char[]  StartOfProtectedData              { get; set; }
        public char[]  LengthOfProtectedData             { get; set; }
        public char[]  UserField                         { get; set; }
        public char[]  Reserved                          { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Generic Constructor. Use Prefilled.
        /// </summary>
        public ImageViewDetailRecord() 
        {
            BaseRecordType = RecordType.ImageViewDetailRecord_Type;
            ImageIndicator                    = new char();          //Position: 02
            ViewSideIndicator                 = new char();          //Position: 08
            DigitalSignatureIndicator         = new char();          //Position: 10
            ImageRecreateIndicator            = new char();          //Position: 15        
            ImageDepositorsRoutingNumber      = new char[9];         //Position: 03        
            ImagingDepositorsDate             = new char[8];         //Position: 04
            ImageViewFormatIndicator          = new char[2];         //Position: 05
            ImageViewCompressionAlgorithmID   = new char[2];         //Position: 06
            ImageViewDataSize                 = new char[7];         //Position: 07        
            ViewDescriptor                    = new char[2];         //Position: 09        
            DigitalSignatureMethod            = new char[2];         //Position: 11
            SecurityKeySize                   = new char[5];         //Position: 12
            StartOfProtectedData              = new char[7];         //Position: 13
            LengthOfProtectedData             = new char[7];         //Position: 14       
            UserField                         = new char[8];         //Position: 16
            Reserved                          = new char[5];         //Position: 17
        }

        /// <summary>
        /// Pre-Filled Image View Detail Constructor will create a Record following
        /// JP Morgan Chase's specifications.
        /// </summary>
        /// <param name="checkSide">A code that indicates the image view conveyed in the related 
        /// Image View Data Record 0=front image view 1=rear image view</param>
        /// <param name="checkDate">Check scan date</param>
        public ImageViewDetailRecord(char checkSide, DateTime checkDate) 
        {
            BaseRecordType                    = RecordType.ImageViewDetailRecord_Type;
            ImageIndicator                    = '1';
            ViewSideIndicator                 = checkSide;
            DigitalSignatureIndicator         = '0';          
            ImageRecreateIndicator            = '1';
            ImageDepositorsRoutingNumber      = FillArray("044000037", 9);
            ImagingDepositorsDate             = FillArray(checkDate.Date.ToString("yyyyMMdd"), 8);
            ImageViewFormatIndicator          = FillArray("00",2);         
            ImageViewCompressionAlgorithmID   = FillArray("00",2);         
            ImageViewDataSize                 = FillArray("0",7);         
            ViewDescriptor                    = FillArray("00",2);         
            DigitalSignatureMethod            = FillArray("",2);         
            SecurityKeySize                   = FillArray("",5);         
            StartOfProtectedData              = FillArray("",7);         
            LengthOfProtectedData             = FillArray("",7);         
            UserField                         = FillArray("",8);         
            Reserved                          = FillArray("",15);         
        }
        
        #endregion

        #region Methods
        /// <summary>
        /// Generates header information for the specified record BaseRecordType
        /// </summary>
        /// <returns></returns>
        public override String GenerateHeaderString()
        {
            String header;
            header = ((int)BaseRecordType).ToString("00") +
                     ImageIndicator                                 +
                     new string(ImageDepositorsRoutingNumber)       +
                     new string(ImagingDepositorsDate)              +
                     new string(ImageViewFormatIndicator)           +
                     new string(ImageViewCompressionAlgorithmID)    +
                     new string(ImageViewDataSize)                  +
                     ViewSideIndicator                              +
                     new string(ViewDescriptor)                     +
                     DigitalSignatureIndicator                      +
                     new string(DigitalSignatureMethod)             +
                     new string(SecurityKeySize)                    +
                     new string(StartOfProtectedData)               +
                     new string(LengthOfProtectedData)              +
                     ImageRecreateIndicator                         +
                     new string(UserField)                          +
                     new string(Reserved)                           ;
            return header;
        }
        #endregion
    }

    /// <summary>
    /// The Image View Detail Record is mandatory when the Cash Letter Documentation BaseRecordType Indicator (Field 9) in the Cash Letter Header
    /// Record (BaseRecordType 10) is 'G' or 'H'. The Image View Detail Record is one of two records (BaseRecordType 50 and BaseRecordType 52) that shall be used together to
    /// convey an image view associated with the related Check Detail Record (BaseRecordType 25). If an Image View Detail Record is present, then an
    /// Image View Data Record shall be present.
    /// </summary>
    public class ImageViewDataRecord        : Record    
    {
        #region Header Fields
        public char ClippingOrigin                  { get; set; }
        public char[] DepositorRoutingNumber        { get; set; }
        public char[] BundleBusinessDate            { get; set; }
        public char[] CycleNumber                   { get; set; }
        public char[] InstitutionItemSequenceNumber { get; set; }
        public char[] SecurityOriginatorName        { get; set; }
        public char[] SecurityAuthenticatorName     { get; set; }
        public char[] SecurityKeyName               { get; set; }
        public char[] ClippingCoordinateh1          { get; set; }
        public char[] ClippingCoordinateh2          { get; set; }
        public char[] ClippingCoordinatev1          { get; set; }
        public char[] ClippingCoordinatev2          { get; set; }
        public char[] LengthofImageReferenceKey     { get; set; }
        public char[] ImageReferenceKey             { get; set; }
        public char[] LengthOfDigitalSignature      { get; set; }
        public char[] DigitalSignature              { get; set; }
        public char[] LengthofImageData             { get; set; }        
        public Image  ImageData                     { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Generic Constructor Use Prefilled-Instructor
        /// </summary>
        public ImageViewDataRecord() 
        {
            BaseRecordType = RecordType.ImageViewDataRecord_Type;
            ClippingOrigin                  = new char();            //Position: 09
            DepositorRoutingNumber          = new char[9];           //Position: 02
            BundleBusinessDate              = new char[8];           //Position: 03
            CycleNumber                     = new char[2];           //Position: 04
            InstitutionItemSequenceNumber   = new char[15];          //Position: 05
            SecurityOriginatorName          = new char[16];          //Position: 06
            SecurityAuthenticatorName       = new char[16];          //Position: 07
            SecurityKeyName                 = new char[16];          //Position: 08        
            ClippingCoordinateh1            = new char[4];           //Position: 10
            ClippingCoordinateh2            = new char[4];           //Position: 11
            ClippingCoordinatev1            = new char[4];           //Position: 12
            ClippingCoordinatev2            = new char[4];           //Position: 13
            LengthofImageReferenceKey       = new char[4];          //Position: 14
            ImageReferenceKey               = new char[2];           //Position: 15
            LengthOfDigitalSignature        = new char[5];           //Position: 16
            DigitalSignature                = new char[2];           //Position: 17
            LengthofImageData               = new char[7];           //Position: 18            
        }
        
        /// <summary>
        /// Pre-filled Image View Data Record will create a Record following
        /// JP Morgan Chase's specifications.
        /// </summary>
        /// <param name="instItemSeqNum">15 Char number assigned by the institution that creates the 
        /// Check Detail Record (Type 25)</param>
        /// <param name="imageData">Check Image</param>
        public ImageViewDataRecord(string instItemSeqNum, Image imageData, string imageLocation) 
        {
            FileInfo imgFile = new FileInfo(imageLocation);
            BaseRecordType = RecordType.ImageViewDataRecord_Type;
            ClippingOrigin                  = '0';
            DepositorRoutingNumber          = FillArray("044000037", 9);
            BundleBusinessDate              = FillArray(DateTime.Now.Date.ToString("yyyyMMdd"), 8);
            CycleNumber                     = FillArray("",2);
            InstitutionItemSequenceNumber   = FillArray(instItemSeqNum,15);
            SecurityOriginatorName          = FillArray("",16);
            SecurityAuthenticatorName       = FillArray("",16);
            SecurityKeyName                 = FillArray("",16);
            ClippingCoordinateh1            = FillArray("",4); 
            ClippingCoordinateh2            = FillArray("",4);
            ClippingCoordinatev1            = FillArray("",4);
            ClippingCoordinatev2            = FillArray("",4); 
            LengthofImageReferenceKey       = FillArray("0",4);
            ImageReferenceKey               = FillArray("0",2);
            LengthOfDigitalSignature        = FillArray("0",5);
            DigitalSignature                = FillArray("0",2);
            LengthofImageData               = FillArray(((int)(imgFile.Length)).ToString("0000000"),7);
            ImageData                       = imageData;
        }
        
        #endregion

        #region Methods
        /// <summary>
        /// Generates header information for the specified record BaseRecordType
        /// </summary>
        /// <returns></returns>
        public override String GenerateHeaderString()
        {
            String header;
            header = ((int)BaseRecordType).ToString("00") +
                     new string(DepositorRoutingNumber) +
                     new string(BundleBusinessDate) +
                     new string(CycleNumber) +
                     new string(InstitutionItemSequenceNumber) +
                     new string(SecurityOriginatorName) +
                     new string(SecurityAuthenticatorName) +
                     new string(SecurityKeyName) +
                     ClippingOrigin +
                     new string(ClippingCoordinateh1) +
                     new string(ClippingCoordinateh2) +
                     new string(ClippingCoordinatev1) +
                     new string(ClippingCoordinatev2) +
                     new string(LengthofImageReferenceKey) +
                     //new string(ImageReferenceKey) +
                     new string(LengthOfDigitalSignature) +
                     //new string(DigitalSignature) +
                     new string(LengthofImageData);                     
            return header;
        }

        public MemoryStream GetImageByteStream() 
        {            
            MemoryStream ms = new MemoryStream(20000);
            ImageData.Save(ms, System.Drawing.Imaging.ImageFormat.Tiff);
            return ms;
        }

        public Byte[] GetImageByteArray()
        {
            MemoryStream ms = new MemoryStream(20000);
            ImageData.Save(ms, System.Drawing.Imaging.ImageFormat.Tiff);
            return ms.ToArray();
        }
        #endregion
    }

    /// <summary>
    /// This record is optional. It is highly recommended, when available, and expected to be included in incoming files. When present, there is
    /// one Image View Analysis Record for each image view.
    /// </summary>
    public class ImageViewAnalysisRecord    : Record    
    {
        #region Constructors
        //OPTIONAL RECORD-IGNORING UNTIL NEEDED
        public ImageViewAnalysisRecord() 
        {
            BaseRecordType = RecordType.ImageViewAnalysisRecord_Type;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generates header information for the specified record BaseRecordType
        /// </summary>
        /// <returns></returns>
        public override String GenerateHeaderString()
        {
            //String header;
            //header = ((int)BaseRecordType).ToString();
            return "";
        }
        #endregion
    }

    /// <summary>
    /// This record is mandatory. It shall be present to complete a bundle that began with a Bundle Header Record (BaseRecordType 20).
    /// </summary>
    public class BundleControlRecord        : Record    
    {
        #region Header Fields
        public char[] ItemsWithinBundleCount  { get; set; }
        public char[] BundleTotalAmount       { get; set; }
        public char[] MicrValidTotalAmount    { get; set; }
        public char[] ImagesWithinBundleCount { get; set; }
        public char[] UserField               { get; set; }
        public char[] Reserved                { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Generic Constructor Use Prefilled-Instructor
        /// </summary>
        public BundleControlRecord() 
        {
            BaseRecordType = RecordType.BundleControlRecord_Type;
            ItemsWithinBundleCount  = new char[4];      //Position: 02
            BundleTotalAmount       = new char[12];      //Position: 03
            MicrValidTotalAmount    = new char[12];     //Position: 04
            ImagesWithinBundleCount = new char[5];     //Position: 05
            UserField               = new char[20];      //Position: 06
            Reserved                = new char[25];     //Position: 07
        }

        /// <summary>
        /// Pre-filled Bundle Control Record will create a Record following
        /// JP Morgan Chase's specifications.
        /// </summary>
        /// <param name="itemCount">Maximum of 300 per bundle</param>
        /// <param name="totalAmount">Total dollar amount of the bundle</param>
        /// <param name="checkCount">The total number of Image View Detail record pairs
        /// within a bundle regardless of whether image data is actually present. Each 
        /// image view is represented by an Image View Detail Record (Type 50) and an Image
        /// View Data Record (Type 52) pair</param>
        public BundleControlRecord(int itemCount, int totalAmount,int checkCount) 
        {
            BaseRecordType          = RecordType.BundleControlRecord_Type;
            ItemsWithinBundleCount  = FillArrayBackEnd(itemCount.ToString("0000"), 4);
            BundleTotalAmount       = FillArray(totalAmount.ToString("000000000000"), 12);
            MicrValidTotalAmount    = FillArray("000000000000", 12);
            ImagesWithinBundleCount = FillArrayBackEnd(checkCount.ToString("00000"), 5);
            UserField               = FillArray("", 20);
            Reserved                = FillArray("", 25);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generates header information for the specified record BaseRecordType
        /// </summary>
        /// <returns></returns>
        public override String GenerateHeaderString()
        {
            String header;
            header = ((int)BaseRecordType).ToString("00") +
                     new string(ItemsWithinBundleCount)     +
                     new string(BundleTotalAmount)          +
                     new string(MicrValidTotalAmount)       +
                     new string(ImagesWithinBundleCount)    +
                     new string(UserField)                  +
                     new string(Reserved)                   ;
            return header;
        }
        #endregion
    }

    /// <summary>
    /// This record is mandatory. There must be one Cash Letter Record (BaseRecordType 90) for each Cash Letter Header Record (BaseRecordType 10). This record
    /// must be the last record in the cash letter. The data in the fields is generated by the depositor that created the corresponding Cash Letter
    /// Header Record.
    /// </summary>
    public class CashLetterControlRecord    : Record    
    {
        #region Header Fields
        public char[] BundleCount                 { get; set; }
        public char[] ItemsWithinCashLetterCount  { get; set; }
        public char[] CashLetterTotalAmount       { get; set; }
        public char[] ImagesWithinCashLetterCount { get; set; }
        public char[] ECEInstitutionName          { get; set; }
        public char[] SettlementDate              { get; set; }
        public char[] Reserved                    { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Generic Constructor Use Prefilled-Instructor
        /// </summary>
        public CashLetterControlRecord() 
        {
            BaseRecordType = RecordType.CashLetterControlRecord_Type;
            BundleCount                 = new char[6];      //Position: 02
            ItemsWithinCashLetterCount  = new char[8];      //Position: 03
            CashLetterTotalAmount       = new char[14];      //Position: 04
            ImagesWithinCashLetterCount = new char[9];     //Position: 05
            ECEInstitutionName          = new char[18];      //Position: 06
            SettlementDate              = new char[8];     //Position: 07
            Reserved                    = new char[15];     //Position: 08
        }

        /// <summary>
        /// Pre-filled Cash Letter Control Record will create a Record following
        /// JP Morgan Chase's specifications.
        /// </summary>
        /// <param name="bundleCount">The total number of bundles within the cash letter</param>
        /// <param name="itemCount">The total number of items sent within the cash letter, all
        /// Check Detail Records (Type 25) or all Return Records (Type 31).</param>
        /// <param name="total">The total US dollar value of the cash letter, all Check Detail 
        /// Records (Type 25) or all Return Records (Type31).</param>
        /// <param name="imageCount">The total number of image view record pairs within a cash 
        /// letter regardless of whether image data is actually present. Each image is represented
        /// by an Image ViewDetail Record (Type 50) and an Image View Data Record (Type 52) pair.</param>
        public CashLetterControlRecord(int bundleCount, int itemCount, int total, int imageCount) 
        {
            BaseRecordType              = RecordType.CashLetterControlRecord_Type;
            BundleCount                 = FillArrayBackEnd(bundleCount.ToString("000000"), 6);
            ItemsWithinCashLetterCount  = FillArrayBackEnd(itemCount.ToString("00000000"), 8);
            CashLetterTotalAmount       = FillArrayBackEnd(total.ToString("00000000000000"), 14);
            ImagesWithinCashLetterCount = FillArrayBackEnd(imageCount.ToString("000000000"), 9);
            ECEInstitutionName          = FillArray("NO INSTITUTE", 18);
            SettlementDate              = FillArray("20131029", 8);
            Reserved                    = FillArray("", 15);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generates header information for the specified record BaseRecordType
        /// </summary>
        /// <returns></returns>
        public override String GenerateHeaderString()
        {
            String header;
            header = ((int)BaseRecordType).ToString("00") +
                     new string(BundleCount)                    +
                     new string(ItemsWithinCashLetterCount)     +
                     new string(CashLetterTotalAmount)          +
                     new string(ImagesWithinCashLetterCount)    +
                     new string(ECEInstitutionName)             +
                     new string(SettlementDate)                 +
                     new string(Reserved)                       ;
            return header;
        }
        #endregion
    }

    /// <summary>
    /// The File Control Record is mandatory. It is the final record of an electronic exchange file. The data in the fields is created by the depositor
    /// sending the file – the immediate-origin depositor.
    /// </summary>
    public class FileControlRecord          : Record    
    {
        #region Header Fields
        public char[] CashLetterCount             { get; set; }
        public char[] TotalRecordCount            { get; set; }
        public char[] TotalItemCount              { get; set; }
        public char[] FileTotalAmount             { get; set; }
        public char[] ImmediateOriginContact      { get; set; }
        public char[] ImmediateOrigContactPhone   { get; set; }
        public char[] Reserved                    { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Generic Constructor Use Prefilled-Instructor
        /// </summary>
        public FileControlRecord() 
        {
            BaseRecordType = RecordType.FileControlRecord_Type;
            CashLetterCount             = new char[6];      //Position: 02
            TotalRecordCount            = new char[8];      //Position: 03
            TotalItemCount              = new char[8];      //Position: 04
            FileTotalAmount             = new char[16];     //Position: 05
            ImmediateOriginContact      = new char[14];     //Position: 06
            ImmediateOrigContactPhone   = new char[10];     //Position: 07
            Reserved                    = new char[16];     //Position: 08
        }

        /// <summary>
        /// Pre-filled File Control Record will create a Record following
        /// JP Morgan Chase's specifications.
        /// </summary>
        /// <param name="cashLetterCount">The total number of cash letters within the file.</param>
        /// <param name="recordCount">The total number of records of all types sent in the file,
        /// including the File Control Record.</param>
        /// <param name="totalItemCount">The total number of items sent within the file, all Check
        /// Detail Records (Type 25) and all Return Records (Type31).</param>
        /// <param name="fileTotalAmount">The total US dollar value of the complete file, all Check
        /// Detail Records (Type 25) and all Return Records (Type31).</param>
        public FileControlRecord(int cashLetterCount, int recordCount, int totalItemCount, int fileTotalAmount) 
        {
            BaseRecordType = RecordType.FileControlRecord_Type;
            CashLetterCount             = FillArray(cashLetterCount.ToString(), 6);
            TotalRecordCount            = FillArray(recordCount.ToString(), 8);
            TotalItemCount              = FillArray(totalItemCount.ToString(), 8);
            FileTotalAmount             = FillArray(fileTotalAmount.ToString(), 16);
            ImmediateOriginContact      = FillArray("", 14);
            ImmediateOrigContactPhone   = FillArray("", 10);
            Reserved                    = FillArray("", 16);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generates header information for the specified record BaseRecordType
        /// </summary>
        /// <returns></returns>
        public override String GenerateHeaderString()
        {
            String header;
            header = ((int)BaseRecordType).ToString("00") + 
                     new string(CashLetterCount)            +
                     new string(TotalRecordCount)           +
                     new string(TotalItemCount)             +
                     new string(FileTotalAmount)            +
                     new string(ImmediateOriginContact)     +
                     new string(ImmediateOrigContactPhone)  +
                     new string(Reserved)                   ;
            return header;
        }
        #endregion
    }

}
