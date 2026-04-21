using System;
using System.IO;
using System.Windows.Forms;
using Atalasoft.Twain;
using Atalasoft.Imaging;
using Atalasoft.Imaging.Codec;
using Atalasoft.Imaging.ImageProcessing.Document;
using Atalasoft.Barcoding.Reading;

namespace TWAINBarCodeDemo
{
	/// <summary>
	/// This class preforms the function of scanning in multipule documents using an automated document
	/// feeder (ADF).  Currently, 2 commands are able to be prefomed, AutoDeskew and AutoDespeckle.
	/// This class also used the built in Barcode Reader functionality of the dotImage 2.1 toolkit.
	/// A cover page with 2 barcodes on it will tell where the following documents should be saved
	/// (in which directory) and what the file format should be.  A ScanMsg Event is raised to signal
	/// the parent class of a message.  This message should then be displayed as text for the GUI.
	/// </summary>
	public class BatchScan
	{
		// Params
		private bool despeckle;			// turn on/off commands
		private bool deskew;
		private String outputpath;		// base filepath to save documents to
		private String filename_format;	
		private Acquisition acquisition = new Acquisition();	// Acquisition to handle all scanning
		private Device device = null;		// object representing the scanner
        private ReadOpts options;		// Barcoding options
        private BarCodeReader readEngin;	//barcode reading object
		
#region EventHandler Code
		public class BatchScanEventArgs : System.EventArgs 
		{	
			public String msg;
			public BatchScanEventArgs(String m){msg = m;}
		}

		public delegate void BatchScanEventHandler(object sender, BatchScanEventArgs e);
		public event BatchScanEventHandler ScanMsg;

		protected virtual void OnScanMsg(BatchScanEventArgs e)
		{
			if (ScanMsg != null)
			{
				ScanMsg(this, e);
			}
		}

#endregion
		
		// Constructor
		public BatchScan()
		{
			init();
			LoadDevice();
		}

		//destructor
		~BatchScan()
		{
			// dispose of the device here
		}
		//Sets values when a new group of documents are to be scanned in the same session.
		
		private void setDefaults()
		{
			//set paramater defaults
			outputpath = @"..\\..\\TWAINoutput\";
			filename_format = @"untitled";
		}
		
		// initialize params
		private void init()
		{
			despeckle = false;
			deskew    = false;
			
			setDefaults();

			//Setup AcquisitionEvents
			this.acquisition.ApplicationIdentity.Info = "My Acquisition";
			this.acquisition.ApplicationIdentity.Manufacturer = "Atalasoft";
			//this.acquisition.Parent = this;
			this.acquisition.ApplicationIdentity.ProductFamily = "Imaging";
			this.acquisition.ApplicationIdentity.ProductName = "Twain";
			this.acquisition.ApplicationIdentity.VersionMajor = 1;
			this.acquisition.ApplicationIdentity.VersionMinor = 0;

			// Register the events to do with Acquireing images from device
			// Only use these when needed
			this.acquisition.AcquireFinished += new System.EventHandler(this.acquisition_AcquireFinished);
			this.acquisition.DeviceEvent += new Atalasoft.Twain.DeviceEventHandler(this.acquisition_DeviceEvent);
			this.acquisition.ImageAcquired += new Atalasoft.Twain.ImageAcquiredEventHandler(this.acquisition_ImageAcquired);
			this.acquisition.AcquireCanceled += new System.EventHandler(this.acquisition_AcquireCanceled);
			//this.acquisition.FileTransfer += new Atalasoft.Twain.FileTransferEventHandler(this.acquisition_FileTransfer);
		}


		// Checks to make sure a scanner is recognized by dotTWAIN, and loads the default device,
		// if there are more than one found.
		private void LoadDevice()
		{
			// Never assume that a system has any acquisition devices.
			if (this.acquisition.Devices.Count == 0)
			{
				OnScanMsg(new BatchScanEventArgs("Devices.Count = 0 -- No devices Found!"));
				return;
			}
				
			string def = this.acquisition.Devices.Default.Identity.ProductName;
			
			// Find the default device
			foreach (Device dev in this.acquisition.Devices)
			{
				if (dev.Identity.ProductName == def)
				{
					this.device = dev;
					return;
				}
			}
			OnScanMsg(new BatchScanEventArgs("Could Not Find Default device"));
		}
		
		#region Accessor Methods

		// Accessor Methods
		public bool Despeckle 
		{
			get { return despeckle; }
			set { despeckle = value; }
		}

		public bool Deskew
		{
			get { return deskew; }
			set { deskew = value; }
		}

		public String OutputPath
		{
			get { return outputpath; }
			set { outputpath = value; }
		}
		#endregion

		// Starts scanning and processing from default device
		public void Process()
		{
			if (this.device == null) 
			{
				OnScanMsg(new BatchScanEventArgs("Acquisition was canceled -- Device Error."));
				return;
			}
			//raise an event
			OnScanMsg(new BatchScanEventArgs("Opening Device ..."));
			this.device.Open();
			
			// acquire an image with (showDeviceScrean, SavetoFile)
			// The ADF will automaticaly read in all documents in scanner
			OnScanMsg(new BatchScanEventArgs("Scanning ..."));
			// acquire image from scannar without displaying its interface and without saving directly
			// to file
			this.device.HideInterface = true;
			this.device.Acquire();

		}

		#region Acquisition Events

		private void acquisition_AcquireCanceled(object sender, System.EventArgs e)
		{
			OnScanMsg(new BatchScanEventArgs("Acquisition was canceled. -- Acquisition Event"));
		}

		private void acquisition_AcquireFinished(object sender, System.EventArgs e)
		{
			// AcquireFinished fires after all images have been acquired.
			// dispose of scanner device object
			this.device.Close();
			OnScanMsg(new BatchScanEventArgs("Closing Device ...\n\rAcquisition Finished."));
		}
		
		// This event is fired every time an page is scanned in
		private void acquisition_ImageAcquired(object sender, Atalasoft.Twain.AcquireEventArgs e)
		{

			if (e.Image != null)
			{
				try			
				{
					AtalaImage tempImage = AtalaImage.FromBitmap(e.Image);
					
					// Start a new workspace to apply comands to images.
					// make global workspace?
					Workspace theworkspace = new Workspace();
					theworkspace.Image = tempImage;

					//apply Comands
					if (this.deskew)
					{
						OnScanMsg(new BatchScanEventArgs("Applying Deskew Command ... "));
						theworkspace.ApplyCommand(new AutoDeskewCommand());
						OnScanMsg(new BatchScanEventArgs("......Done"));
					}
					if (this.despeckle)
					{
						OnScanMsg(new BatchScanEventArgs("Applying Despeckle Command ... "));
						theworkspace.ApplyCommand(new DocumentDespeckleCommand());
						OnScanMsg(new BatchScanEventArgs("......Done"));
					}

					OnScanMsg(new BatchScanEventArgs("Image Acquired, Scanning for Barcodes ..."));
					// check if page scanned has barcodes.
					// isCoverSheet() ?
			
					// Check every page for barcodes
					Barcoder_Load(); // load options
					// load the current image for barcode reading
                    readEngin = new BarCodeReader(theworkspace.Image);
					// Read the barcodes into the collection results
                    BarCode[] results = recognizeBarcodes(readEngin, options);
					// Decode the barcode, if none are found, nothing will happen
					decodeResults(results);
					// Display results
					OnScanMsg(new BatchScanEventArgs(results.Length + " total barcode" +
						(results.Length == 1 ? "" : "s") + " found."));
					
					// Create directory if it does not already exist.  If it does, this
					// call does nothing.
					DirectoryInfo newpath = new DirectoryInfo(this.outputpath);
					newpath.Create();
				
					// Save the transformed image
					// Default is saveing to C:\TWAINoutput\untitled_xx.Tif
					String filename = this.outputpath + filename_format + "_"
														+ (getNumFiles()).ToString() +  ".Tif";
					OnScanMsg(new BatchScanEventArgs("Saving file to: " + filename));
					theworkspace.Save(filename, new TiffEncoder());

					// dispose after we are done
					theworkspace.Dispose();
					tempImage.Dispose();
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}finally{}
			}
		}
		
//		private void acquisition_FileTransfer(object sender, Atalasoft.Twain.FileTransferEventArgs e)
//		{
//			// This will fire before a file transfer takes place.
//			// Unused in this class.
//
//		}

		
		private void acquisition_DeviceEvent(object sender, Atalasoft.Twain.DeviceEventArgs e)
		{
			// One of the many device events has fired.
			// You will only receive the events you have set using
			// the Device.DeviceEvents property.

			// Currently,  none have been set, but just in case ...
			OnScanMsg(new BatchScanEventArgs("Device Event:  " + e.Event.ToString()));
		}
		
		// an unexpected exception has occured, display it
		private void acquisition_AsynchronousException(object sender, Atalasoft.Twain.AsynchronousExceptionEventArgs e)
		{
			OnScanMsg(new BatchScanEventArgs("Asynchronous Exception: " + e.Exception.Message));
		}
		
		#endregion

		// helper methods

		// Method to find number of untitled files already in directory
		// returns number of files.  Next file should be named this number
		// because numbering starts at 0;
		private Int32 getNumFiles()
		{
			DirectoryInfo path = new DirectoryInfo(outputpath);
			FileInfo[] f = path.GetFiles(filename_format + "*.Tif");
			return f.Length;
		}

		//Loads the Barcode reader
		private void Barcoder_Load()
		{
			// set a few reasonable default options
            options = new ReadOpts();
			// set the barcode font to code39.  This could be changed to account for letters, etc.
            options.Symbology = Symbologies.Code39;
			// read barcodes left to right and right to left
            options.Direction = Directions.East |
                                Directions.West;
			// counter-intuitive - these defaults get pulled from the UI instead
			// of being pushed into the UI
			options.ScanInterval = 5;  //default value
			options.ScanBarsToRead = 2; // 2 barcodes expected per page for this example
		}

		// Read a set of barcodes from an image. 
        private BarCode[] recognizeBarcodes(BarCodeReader reader, ReadOpts optionsIn) 
		{

            BarCode[] results = null;
            ReadOpts options = new ReadOpts(optionsIn);

			if (options.Symbology == 0)  // a symbology must be specified
			{
				return null;
			}

			try 
			{
				// This is where the barcodes are read
				results = reader.ReadBars(options);
			}
			catch (ArgumentOutOfRangeException ex)  // a few errors
			{
			OnScanMsg(new BatchScanEventArgs("Range error in options: " + ex.Message ));
			}
			catch (System.Exception ex) 
			{
			OnScanMsg(new BatchScanEventArgs("General error: " + ex.Message ));
			}
			
			return results;
		}

		//Parses the barcode data into the expected params
		// if there are no barcodes on this page, it returns (not a cover page),
		// otherwise the new filepath and filename format are saved. An incorrect number
		// of barcodes on the page displays an error message, and the page is treated like
		// it does not have any barcodes.
        bool decodeResults(BarCode[] bcInfo)
		{
			//bcInfo is a collection containing the information of each barcode found 
			if (bcInfo.Length == 0)
				return false;
			if (bcInfo.Length == 1)
			{
				OnScanMsg(new BatchScanEventArgs("Error in decodeResults -- missing barcode."));
				return false;
			}
			if (bcInfo.Length > 2)
			{
				//error
				OnScanMsg(new BatchScanEventArgs("Error in decodeResults -- more than 1 barcode on a page"));
				return false;
			}
			// Current Image is a cover Sheet, deal with acordignly

			// if no barcodes are found on the first page, all pages will be saved to the default 
			// location (c:\TWAINoutput) with filenames untitled_x.tif	
			setDefaults();
			
			int directory = 1;
			int fname = 0;

			if (bcInfo[0].DataString.StartsWith("%")) // finds out which barcode is the filepath
			{
				directory = 0; 
				fname = 1;
			}

			String info = bcInfo[directory].DataString;
			// set filename_format to the specified format (no special chars to parse out)
			filename_format = bcInfo[fname].DataString;
			
				// set outputpath to the directory structure indicated by the barcode
				foreach( Char c in (info.Substring(1)).ToCharArray())
				{
					if (c.Equals('%'))
						this.outputpath = this.outputpath + @"\";
					else
						this.outputpath = this.outputpath + c;
				}
				
				// Create directory if it does not already exist.  If it does, this
				// call does nothing.
				DirectoryInfo newpath = new DirectoryInfo(this.outputpath);
				newpath.Create();
			
			return true;
		}


	} // end BatchScan class
}// namespace myTWAINdemo
