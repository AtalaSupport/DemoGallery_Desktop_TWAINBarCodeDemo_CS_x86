# TWAIN BarCode Demo
This demo shows how to scan multiple pages (using DotTwain) and file them based on a barcode.  

Each group of pages have their own cover sheet with a barcode that holds information about where to save the following scanned images.  
		

This example uses the Code39 symbology, which consists of numbers, some letters, and some symbols.  The dotImage bar code reader can be set to recognize all kinds of symbologies, but for simplicity we are only using numbers here.  

Now, lets assume a scenario of the following type:  Company X wants to convert a whole bunch of paper documents into digital documents.  The documents are text documents written by two employees.  Company X has come up with a file system that uses only numbers.  The directory for text documents is “1234”, the number of employee 1 is “4444” and the number of employee 2 is “888”.  Company also wants each filename to have the format of “111_x.tif”.  Using this system, the pages can be scanned and stored in the proper directory structure.  

This is a C# application. We also offer a [VB.NET version](https://github.(mailto:sales@atalasoft.com)/AtalaSupport/DemoGallery_Desktop_TWAINBarCodeDemo_VB_x86).


### Document Prep
The format is that each set of text documents written by on employee is feed into the 
scanner together.  The first of these pages contains bar codes that tell that computer what 
the set of documents is and who wrote them, it will then store all the following 
documents to the appropriate directory.  When a separator page (a page with bar codes on 
it) is encountered, the computer assumes that the current group of documents has ended 
and the next group’s information is taken from the bar codes on the separator page.

## NOTE
This scenario is demonstrated on the 4 pages following this one, which are intended to be 
printed out and scanned through your scanner.  They only demonstrate the capability of 
document imaging using bar coded indexing, and this example can scan in an arbitrary 
number of groups of any size.

## Instructions
1.	Print the following pages.
2.	Load the pages into a TWAIN scanner with an ADF.
3.	Run the myTWAINdemo application.
4.	Click on “Start Batch Scan” To start the scanning the documents.
5.	Look in the directory “C:\TWAINoutput\1234” to find the document images.
6.	Using a Code39 Symbology font, you can create your own bar codes to be read by the demo program.

Requires DotImage, DotImage BarcodeReader Code39, and DotTwain.


## Licensing
This application requires a license for DotImage Document Imaging as well as our Barcode Reader addon. (In theory, you could use DotTwain DotImage Photo or DotImage Photo Pro in place of DotImage Document Imaging. You may also request a 30 day evaulation if youre evaluating if DotImage / our OCR is right for you.


## SDK Dependencies
This app was built based on 2026.2.0.0. It targets .NET Framework 4.6.2 and was created in Visual Studio 2019. You must have our SDK installed (and licesed per above)

[Download DotImage](https://www.atalasoft.(mailto:sales@atalasoft.com)/BeginDownload/DotImageDownloadPage)


### Using NuGet for SDK Dependencies
We do publish our SDK components to NuGet. We have chosen to base the demo on local installed SDK because this leads to much smaller applications (NuGet packages add a lot of overhead due to the way they're packaged and deployed, and many of our demos -- including this one -- are often used to reproduce issues that need to be submitted to support. Apps that use NuGet are often significantly larger and run up against our maximum support case upload size)

Still, if you wish to use NuGet for the dependencies instead of relying on locally installed SDK, you can.

- Take note of each of the references we've included:
    - Atalasoft.DotImage.dll
    - Atalasoft.DotImage.AdvancedDocClean.dll
    - Atalasoft.DotImage.Barcoding.Reading.dll
    - Atalasoft.DotImage.Lib.dll
    - Atalasoft.DotImage.PdfDoc.Bridge.dll
    - Atalasoft.DotImage.WinControls.dll
    - Atalasoft.PdfDoc.dll
    - Atalasoft.Shared.dll
- Remove those referneces
- Open the NuGet Package Manger from `Tools -> NuGet Package Manager -> Manage NuGet Packages for this Solution`
- Browse for and install  Atalasoft.DotImage.WinControls.x64 - It will pull in DotImage Document Imaging (the base SDK) and our windows controls and shared dll
- Browse for and install Atalasoft.Barcoding.Readingt.x64 - brings in the Barcode Reading engine


## Downloading source
The sources can be downloaded for [c#](https://github.(mailto:sales@atalasoft.com)/AtalaSupport/DemoGallery_Desktop_TWAINBarCodeDemo_CS_x86/archive/refs/heads/main.zip) and [VB.NET](https://github.(mailto:sales@atalasoft.com)/AtalaSupport/DemoGallery_Desktop_TWAINBarCodeDemo_VB_x86/archive/refs/heads/main.zip)


## Cloning
We recommend the following if you wisht to clone our repository:

Example: git for windows
```bash
git clone https://github.(mailto:sales@atalasoft.com)/AtalaSupport/DemoGallery_Desktop_TWAINBarCodeDemo_CS_x86.git TWAINBarCodeDemo 
```


## Related documentation
In addition to this README, the Atalasoft documentation set includes the following:  
- [AtalaSupport Github](https://github.(mailto:sales@atalasoft.com)/AtalaSupport/) For an extensive set of sample apps.  
- [Atalasoft's APIs & Developer Guides page](https://www.atalasoft.(mailto:sales@atalasoft.com)/Support/APIs-Dev-Guides) for our Developers guide and API references.  
- [Atalasoft Support](http://www.atalasoft.(mailto:sales@atalasoft.com)/support/) for our main support portal.
- [Atalasoft Knowledgebase](http://www.atalasoft.(mailto:sales@atalasoft.com)/kb2) where you can find answers to common questions / issues.  


## Getting Help for Atalasoft products
Atalasoft regularly updates our support [Knowledgebase](http://www.atalasoft.(mailto:sales@atalasoft.com)/kb2) with the latest information about our products. To access some resources, you must have a valid Support Agreement with an authorized Atalasoft Reseller/Partner or with Atalasoft directly. Use the tools that Atalasoft provides for researching and identifying issues. 

Customers with an active evaluation, or those with active support / maintenance may [create a support case](https://www.atalasoft.(mailto:sales@atalasoft.com)/Support/my-portal/Cases/Create-Case) 24/7, or call in to support ([+1 949 236-6510](tel:19492366510) ) during our normal support hours (Monday - Friday 8:00am to 5:00PM Eastern (New York) time).  

Customers who are unable to create a case or call in may [email our Sales Team](mailto:sales@atalasoft.com).  

