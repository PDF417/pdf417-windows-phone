# Table of contents

* [Windows Phone _PDF417_ integration instructions](#intro)
* [Quick Start](#quickStart)
  * [Quick start with demo app](#quickDemo)
  * [Quick integration of _PDF417_ into your app](#quickIntegration)
  * [Performing your first scan](#quickScan)
* [Advanced _PDF417_ integration instructions](#advancedIntegration)
  * [Checking if _PDF417_ is supported](#supportCheck)
  * [Embedding `RecognizerControl` into custom application page](#recognizerControl)
  * [`RecognizerControl` reference](#recognizerControlReference)
* [Recognition settings and results](#recognitionSettingsAndResults)
  * [Generic settings](#genericSettings)
  * [Scanning PDF417 barcodes](#pdf417Recognizer)
  * [Scanning one dimensional barcodes with _PDF417_'s implementation](#custom1DBarDecoder)
  * [Scanning barcodes with ZXing implementation](#zxing)
  * [Scanning US Driver's licence barcodes](#usdl)
* [Troubleshooting](#troubleshoot)
  * [Integration problems](#integrationTroubleshoot)
  * [SDK problems](#sdkTroubleshoot)
* [Additional info](#info)


# <a name="intro"></a> Windows Phone _PDF417_ integration instructions

The package contains Visual Studio 2012 solution(can open in VS 2013) that contains everything you need to get you started with _PDF417_ library. Demo project _PDF417Demo_ for Windows Phone 8.0 is included in solution containing the example use of _PDF417_ library.
 
_PDF417_ is supported on Windows Phone 8.0. Windows Phone 8.1 can be supported with minor changes and Windows Phone 10 is expected to be supported soon.

# <a name="quickStart"></a> Quick Start

## <a name="quickDemo"></a> Quick start with demo app

### In Visual Studio 2012

1. In _FILE_ menu choose _Open Project..._.
2. In _Open project_ dialog select _PDF417Demo.sln_ in _PDF417Demo_ folder.
3. Wait for project to load.

### In Visual Studio 2013

1. In _FILE_ menu choose _Open_ then _Project/Solution_.
2. In _Open project_ dialog select _PDF417Demo.sln_ in _PDF417Demo_ folder.
3. Wait for project to load.

## <a name="quickIntegration"></a> Quick integration of _PDF417_ into your app

This works the same in both _Visual Studio 2012_ or _Visual Studio 2013_

1. In File Explorer (**NOT** in VS) copy the _WPLibDebug_ and _WPLibRelease_ folders from _PDF417_ SDK to your project's folder
2. Copy (**INSIDE** VS) the _WPLibAssets_ folder from _PDF417_ SDK into your project (it is important to preserve folder structure)
3. Set the properties _Build Action_ to _None_ and _Copy to Output Directory_ to _Copy if newer_ to **all** the files in _WPLibAssets_ folder
4. Right click to your project's _References_ and choose _Add Reference..._
5. Click the _Browse..._ button on the bottom
6. In _Select the files to reference..._ dialog select _PDF417.dll_ and _Microblink.winmd_ from _WPLibDebug_ folder (you created in first step) if you want to link to debug version of _PDF417_ library (select files from _WPLibRelease_ if you want to link to release version of _PDF417_ library)

## <a name="quickScan"></a> Performing your first scan
1. Add `RecognizerControl` to main page of your application. Your .xaml file should contain something like these lines:

	```xml
	xmlns:UserControls="clr-namespace:Microblink.UserControls;assembly=PDF417"
	```
	and `RecognizerControl` tag itself:
	```xml
	<UserControls:RecognizerControl x:Name="mRecognizer" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" />
	```
2. You should setup `RecognizerControl` in containing page constructor like this:

	```csharp
	// sets license key
	// obtain your licence key at http://microblink.com/login or
	// contact us at http://help.microblink.com            
	mRecognizer.LicenseKey = "Add your licence key here";
	
	// setup array of recognizer settings
	Microblink.PDF417RecognizerSettings pdf417Settings = new Microblink.PDF417RecognizerSettings();
    Microblink.ZXingRecognizerSettings zxingSettings = new Microblink.ZXingRecognizerSettings() { QRCode = true };
    Microblink.BarDecoderRecognizerSettings bardecSettings = new Microblink.BarDecoderRecognizerSettings() { ScanCode128 = true };
    Microblink.USDLRecognizerSettings usdlSettings = new Microblink.USDLRecognizerSettings();
    mRecognizer.RecognizerSettings = new Microblink.IRecognizerSettings[] { pdf417Settings, zxingSettings, bardecSettings, usdlSettings };        
	
	// these three events must be handled
	mRecognizer.OnCameraError += mRecognizer_OnCameraError;            
	mRecognizer.OnScanningDone += mRecognizer_OnScanningDone;
	mRecognizer.OnInitializationError += mRecognizer_OnInitializationError;
	```
3. You should implement `OnNavigatedTo` and `OnNavigatedFrom` of your main page to initialize and terminate `RecognizerControl` respectively so `RecognizerControl` will be initialized when the user activates the page and will terminate when the user navigates away from the page. You should do it like this:
	
	```csharp
	protected override void OnNavigatedTo(NavigationEventArgs e) {
        // call default behaviour
        base.OnNavigatedTo(e);
        // initialize the recognition process
        mRecognizer.InitializeControl(this.Orientation);
    }
        
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
        // terminate the recognizer
        mRecognizer.TerminateControl();
        // call default behaviour
        base.OnNavigatingFrom(e);
    }
	```
4. You should also implement `OnOrientationChanged` of your main page and forward the orientation change info to `RecognizerControl` like this:
	
	```csharp
	protected override void OnOrientationChanged(OrientationChangedEventArgs e) {
        // call default behaviour
        base.OnOrientationChanged(e);            
        // orientation change events MUST be forwarded to RecognizerControl
        mRecognizer.OnOrientationChanged(e);            
    }
	```
5. After scan finishes it will trigger `OnScanningDone` event. You can obtain the scanning results by implementing the event handler something like this:
	
	```csharp
	void mRecognizer_OnScanningDone(IList<Microblink.IRecognitionResult> resultList, RecognitionType recognitionType) {
        // display results if scan was successful
        if (recognitionType == RecognitionType.SUCCESSFUL) {
            StringBuilder b = new StringBuilder();
            // process all results in result list
            foreach (var result in resultList) {
                // process all result elements
                if (result.Valid && !result.Empty) {
	                foreach (var key in result.Elements.Keys) {
	                    // append key-value pairs to StringBuilder
	                    b.Append(key);
	                    b.Append(" = ");
	                    b.Append(result.Elements[key]);
	                    b.Append("\n");
	                }
                }
            }
            // display message box with scanned results
            MessageBox.Show("Results:\n" + b.ToString());
        }
        // resume scanning
        mRecognizer.ResumeScanning();
    }
	```
	
	For more information about defining recognition settings and obtaining scan results see [Recognition settings and results](#recognitionSettingsAndResults).

# <a name="advancedIntegration"></a> Advanced _PDF417_ integration instructions
This section will cover more advanced details in _PDF417_ integration. First part will discuss the methods for checking whether _PDF417_ is supported on current device. Second part will show how to embed `RecognizerControl` into custom application page. Third part is a brief `RecognizerControl` reference.

## <a name="supportCheck"></a> Checking if _PDF417_ is supported

### _PDF417_ requirements
Even before starting the scanning process, you should check if _PDF417_ is supported on current device. In order to be supported, device needs to have a camera.

Windows Phone 8.0 is the minimum version on which _PDF417_ is supported. It is supported on Windows Phone 8.1 with minor adjustments and we expect Windows 10.0 (Mobile) to be supported in 2016.

Camera video preview resolution also matters. In order to perform successful scans, camera preview resolution cannot be too low. _PDF417_ requires minimum 480p camera preview resolution in order to perform scan. It must be noted that camera preview resolution is not the same as the video record resolution, although on most devices those are the same. However, there are some devices that allow recording of HD video (720p resolution), but do not allow high enough camera preview resolution. _PDF417_ does not work on those devices.

However, some recognizers require camera with autofocus. If you try to start recognition with those recognizers on a device that does not have camera with autofocus, you will get an error. To determine whether does recognizer require camera with autofocus or not you can call `bool requiresAutofocus()` method of `Microblink.IRecognitionSettings` interface.

## <a name="recognizerControl"></a> Embedding `RecognizerControl` into custom application page
This section will discuss how to embed `RecognizerControl` into your windows phone application page and perform scan.
Note that this example is for Windows Phone 8.0.

Here is the minimum example of integration of `RecognizerControl` as the only control in your page:

**.xaml file**

```xml
<phone:PhoneApplicationPage
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:phone="clr-namespace:Microsoft.Phone.Controls;assembly=Microsoft.Phone"
    xmlns:shell="clr-namespace:Microsoft.Phone.Shell;assembly=Microsoft.Phone"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:UserControls="clr-namespace:Microblink.UserControls;assembly=PDF417"
    xmlns:local="clr-namespace:MyApp"
    x:Class="MyApp.MyPage"
    mc:Ignorable="d"
    FontFamily="{StaticResource PhoneFontFamilyNormal}"
    FontSize="{StaticResource PhoneFontSizeNormal}"
    Foreground="{StaticResource PhoneForegroundBrush}"
    SupportedOrientations="PortraitOrLandscape" Orientation="Portrait"
    shell:SystemTray.IsVisible="True">
    
    <Grid Background="Transparent">        
                        
        <UserControls:RecognizerControl x:Name="mRecognizer" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" />
                
    </Grid>

</phone:PhoneApplicationPage>
```

**.xaml.cs file**

```csharp
public partial class MyPage : PhoneApplicationPage
    {
    
    private void InitializeRecognizer() {            
        // sets license key
		// obtain your licence key at http://microblink.com/login or
		// contact us at http://help.microblink.com            
		mRecognizer.LicenseKey = "Add your licence key here";
		
		// setup array of recognizer settings
		Microblink.PDF417RecognizerSettings pdf417Settings = new Microblink.PDF417RecognizerSettings();
        Microblink.ZXingRecognizerSettings zxingSettings = new Microblink.ZXingRecognizerSettings() { QRCode = true };
        Microblink.BarDecoderRecognizerSettings bardecSettings = new Microblink.BarDecoderRecognizerSettings() { ScanCode128 = true };
        Microblink.USDLRecognizerSettings usdlSettings = new Microblink.USDLRecognizerSettings();
        mRecognizer.RecognizerSettings = new Microblink.IRecognizerSettings[] { pdf417Settings, zxingSettings, bardecSettings, usdlSettings };        
		
		// these three events must be handled
		mRecognizer.OnCameraError += mRecognizer_OnCameraError;            
		mRecognizer.OnScanningDone += mRecognizer_OnScanningDone;
		mRecognizer.OnInitializationError += mRecognizer_OnInitializationError;
    }
    
    void mRecognizer_OnInitializationError(InitializationErrorType errorType) {
        // handle licensing error by displaying error message and terminating the application
        if (errorType == InitializationErrorType.INVALID_LICENSE_KEY) {
            MessageBox.Show("Could not unlock API! Invalid license key!\nThe application will now terminate!");
            Application.Current.Terminate();
        } else {
            // there are no other error types
            throw new NotImplementedException();
        }           
    }
    
    void mRecognizer_OnScanningDone(IList<Microblink.IRecognitionResult> resultList, RecognitionType recognitionType) {
        // display results if scan was successful
        if (recognitionType == RecognitionType.SUCCESSFUL) {
            StringBuilder b = new StringBuilder();
            // process all results in result list
            foreach (var result in resultList) {
                // process all result elements
                if (result.Valid && !result.Empty) {
	                foreach (var key in result.Elements.Keys) {
	                    // append key-value pairs to StringBuilder
	                    b.Append(key);
	                    b.Append(" = ");
	                    b.Append(result.Elements[key]);
	                    b.Append("\n");
	                }
            	}
            }
            // display message box with scanned results
            MessageBox.Show("Results:\n" + b.ToString());
        }
        // resume scanning
        mRecognizer.ResumeScanning();
    }
    
    void mRecognizer_OnCameraError(Microblink.UserControls.CameraError error) {
        MessageBox.Show("Could not initialize the camera!\nThe application will now terminate!");
        Application.Current.Terminate();
    }        
    
    public MyPage()
    {
        InitializeComponent();
        // set up recognizer
        InitializeRecognizer();
    }
   
    protected override void OnNavigatedTo(NavigationEventArgs e) {
        // call default behaviour
        base.OnNavigatedTo(e);
        // initialize the recognition process
        mRecognizer.InitializeControl(this.Orientation);
    }
    
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
        // terminate the recognizer
        mRecognizer.TerminateControl();
        // call default behaviour
        base.OnNavigatingFrom(e);
    }    

    protected override void OnOrientationChanged(OrientationChangedEventArgs e) {
        // call default behaviour
        base.OnOrientationChanged(e);        
        // orientation change events MUST be forwarded to RecognizerControl
        mRecognizer.OnOrientationChanged(e);            
    }    
               
}
```

## <a name="recognizerControlReference"></a> `RecognizerControl` reference
The usage example for `RecognizerControl` is provided in `PDF417Demo` demo app provided with SDK. This section just gives a quick overview of `RecognizerControl's` most important features.

### <a name="recognizerControlMethods"></a> **Methods**

##### <a name="recognizerControl_InitializeControl"></a> `InitializeControl(PageOrientation)`
This method will initialize `RecognizerControl's` internal fields and will initialize camera control thread. This method must be called after all other settings are already defined, such as event handlers and recognition settings.

##### <a name="recognizerControl_ResumeScanning"></a> `ResumeScanning()`
This method resumes scanning loop. Scanning loop is usually paused when results have arrived and OnScanningDone event is raised. You can also pause scanning loop by yourself by calling PauseScanning.

##### <a name="recognizerControl_PauseScanning"></a> `PauseScanning()`
This method pauses scanning loop. Scanning loop is resumed by calling ResumeScanning.

##### <a name="recognizerControl_TerminateControl"></a> `TerminateControl()`
This method terminates `RecognizerControl` internal state, stops the recognizers and shuts down camera control thread. Call this method when you are finished with scanning and want to free resources used up by _PDF417_ library. You can reinitialize `RecognizerControl` by calling `InitializeControl` method.

##### <a name="recognizerControl_OnOrientationChanged"></a> `OnOrientationChanged(OrientationChangedEventArgs)`
This method should be called to forward the `OnOrientationChanged` event to `RecognizerControl` so it can handle the orientation change.

##### <a name="recognizerControl_SetROI"></a> `SetROI(Windows.Foundation.Rect)`
You can use this method to define the scanning region of interest.

Region of interest is defined as `Windows.Foundation.Rect`. First parameter of rectangle is x-coordinate represented as percentage of view width, second parameter is y-coordinate represented as percentage of view height, third parameter is region width represented as percentage of view width and fourth parameter is region height represented as percentage of view height.

View width and height are defined in current context, i.e. they depend on screen orientation.

Note that scanning region of interest only reflects to native code - it does not have any impact on user interface. You are required to create a matching user interface that will visualize the same scanning region you set here.

##### <a name="recognizerControl_TransformPointCoordinateRelative"></a> `TransformPointCoordinateRelative(Point)`
Method to transform a point from a position in image to a position in a control relative to current preview size.

##### <a name="recognizerControl_TransformPointCoordinateAbsolute"></a> `TransformPointCoordinateAbsolute(Point)`
Method to transform a point from a position in image to absolute position in a control.

##### <a name="recognizerControl_TransformPointCoordinateROI"></a> `TransformPointCoordinateROI(Point)`
Method to transform a point from a position in image to a position in a control relative to Region of Interest.

### <a name="recognizerControlProperties"></a> **Properties**

##### <a name="recognizerControl_GenericRecognizerSettings"></a> `GenericRecognizerSettings`
With this property you can set the generic settings that will affect all enabled recognizers or the whole recognition process. For more information about generic settings, see [Generic settings](#genericSettings). This property must be set before `InitializeControl()`.

##### <a name="recognizerControl_RecognizerSettings"></a> `RecognizerSettings`
Array of `IRecognizerSettings` objects. Those objects will contain information about what will be scanned and how will scan be performed. For more information about recognition settings and results see [Recognition settings and results](#recognitionSettingsAndResults). This property must be set before `InitializeControl()`.

##### <a name="recognizerControl_PreviewScale"></a> `PreviewScale`
Defines the aspect mode of camera. If set to `Uniform` (default), then camera preview will be fit inside available view space. If set to `UniformToFill`, camera preview will be zoomed and cropped to use the entire view space.

##### <a name="recognizerControl_MacroMode"></a> `MacroMode`
When set to `true`, camera will be optimized for near object scanning. It will focus on near objects more easily and will not be able to focus on far objects. Use this only if you plan to hold your device very near to the object you are scanning. Also, feel free to experiment with both `true` and `false` to see which better suits your use case.

##### <a name="recognizerControl_IsTorchSupported"></a> `IsTorchSupported` 
This property is set to `true` if camera supports torch flash mode. Note that if camera is not loaded it will be set to `false`.

##### <a name="recognizerControl_TorchOn"></a> `TorchOn` 
If torch flash mode is supported on camera, this property can be used to enable/disable torch flash mode. When set to `true` torch is turned on. Note that camera has to be loaded for this to work.

##### <a name="recognizerControl_LicenseKey"></a> `Licensee` and `LicenseKey`
With these properties you can set the license key for _PDF417_. You can obtain your license key from [Microblink website](http://microblink.com/login) or you can contact us at [http://help.microblink.com](http://help.microblink.com).
Once you obtain a license key, you can set it with following snippet:

```csharp
// set the license key
mRecognizer.LicenseKey = "Enter_License_Key_Here";
```

License key is bound to your application ID. For example, if you have license key that is bound to `PDF417Demo` application ID, you cannot use the same key in other applications. However, if you purchase Premium license, you will get license key that can be used in multiple applications. This license key will then not be bound to application ID. Instead, it will be bound to the licensee string that needs to be provided to the library together with the license key. To provide licensee string, use something like this:

```csharp
// set the license key
mRecognizer.Licensee = "Enter_Licensee_Here";
mRecognizer.LicenseKey = "Enter_License_Key_Here";
```

### <a name="recognizerControlEvents"></a> **Events**

##### <a name="recognizerControl_OnAutofocusFailed"></a> `OnAutofocusFailed` 
This event is raised when camera fails to perform autofocus even after multiple attempts. You should inform the user to try using camera under different light.

##### <a name="recognizerControl_OnCameraError"></a> `OnCameraError`
This event is triggered on camera related errors. This event **must** be handled or the _PDF417_ library will throw an exception. Camera errors come in four different types:

* `NoCameraAtSelectedSensorLocation` - There is no camera at selected location(front or right)
* `CameraNotReady` - Camera is not ready
* `PreviewSizeTooSmall` - Camera preview size is smaller than required
* `NotSupported` - Required camera is not supported

##### <a name="recognizerControl_OnScanningDone"></a> `OnScanningDone` 
This event is raised when scanning finishes and scan data is ready. This event **must** be handled or the _PDF417_ library will throw an exception. After recognition completes, `RecognizerControl` will pause its scanning loop and to continue the scanning you will have to call `ResumeScanning` method.

##### <a name="recognizerControl_OnInitializationError"></a> `OnInitializationError` 
This event is raised when an error occurs during RecognizerControl initialization. At the moment only licensing errors trigger this event. This event **must** be handled or the _PDF417_ library will throw an exception.
        
##### <a name="recognizerControl_OnControlInitialized"></a> `OnControlInitialized` 
Triggered after canvas is initialized and camera is ready for receiving frames.

##### <a name="recognizerControl_OnSuccessfulScanImage"></a> `OnSuccessfulScanImage` 
Handle this event to obtain images that are currently being processed by the native library. This event will return images that resulted in a successful scan. Please take notice that installing this event handler introduces a large performance penalty.

##### <a name="recognizerControl_OnOriginalImage"></a> `OnOriginalImage` 
Handle this event to obtain images that are currently being processed by the native library. This event will return original images passed to recognition process. Please take notice that installing this event handler introduces a large performance penalty.

##### <a name="recognizerControl_OnDewarpedImage"></a> `OnDewarpedImage` 
Handle this event to obtain images that are currently being processed by the native library. This event will return dewarped images from the recognition process. Please take notice that installing this event handler introduces a large performance penalty.

##### <a name="recognizerControl_OnDisplayDefaultTarget"></a> `OnDisplayDefaultTarget` 
This event is raised when recognizer wants to put viewfinder in its default position (for example if detection has failed).

##### <a name="recognizerControl_OnDisplayNewTarget"></a> `OnDisplayNewTarget` 
This event is raised when recognizer detects an object and wants to put viewfinder in position of detected object.

##### <a name="recognizerControl_OnDisplayOcrResult"></a> `OnDisplayOcrResult` 
This event is raised when recognizer has OCR support enabled and has OCR result ready for displaying / using. You can handle this event to display real-time OCR results in preview.

##### <a name="recognizerControl_OnDisplayPointSet"></a> `OnDisplayPointSet` 
This event is raised when recognizer detects a non-rectangular object (e.g. 1D barcode, QR code, etc), instead of raising `OnDisplayNewTarget`. Handler will be provided with a list of detected object's feature points (in image coordinates). You can handle this event to display real-time detected object's feature points in preview.
        
Image coordinates refer to coordinates in video frame that has been analyzed. Usually the video frame is in landscape right mode, i.e. (0,0) represents the upper right corner and x coordinate rises downwards and y coordinate rises leftward.
        
##### <a name="recognizerControl_OnShakingStartedEvent"></a> `OnShakingStartedEvent` 
Event is triggered when device shaking starts.

# <a name="recognitionSettingsAndResults"></a> Recognition settings and results

This chapter will discuss various recognition settings used to configure different recognizers and scan results generated by them.

## <a name="genericSettings"></a> Generic settings

Generic settings affect all enabled recognizers and the whole recognition process. Here is the list of properties that are most relevant:

##### `bool AllowMultipleScanResults`
Sets whether or not outputting of multiple scan results from same image is allowed. If that is `true`, it is possible to return multiple recognition results from same image. By default, this option is `false`, i.e. the array of `IRecognitionResult`s will contain at most 1 element. The upside of setting that option to `false` is the speed - if you enable lots of recognizers, as soon as the first recognizer succeeds in scanning, recognition chain will be terminated and other recognizers will not get a chance to analyze the image. The downside is that you are then unable to obtain multiple results from single image.

##### `int RecognitionTimeout`
Number of miliseconds _PDF417_ will attempt to perform the scan before it exits with timeout error. On timeout returned array of `IRecognitionResult`s might be null, empty or may contain only elements that are not valid (`Valid` is `false`) or are empty (`Empty` is `true`).

## <a name="pdf417Recognizer"></a> Scanning PDF417 barcodes

This section discusses the settings for setting up PDF417 recognizer and explains how to obtain results from PDF417 recognizer.

### Setting up PDF417 recognizer

To activate PDF417 recognizer, you need to create a `PDF417RecognizerSettings` and add it to `IRecognizerSettings` array. You can do this using following code snippet:

```csharp
using Microblink;

private IRecognizerSettings[] setupSettingsArray() {
	PDF417RecognizerSettings sett = new PDF417RecognizerSettings();
	
	// disable inverse scan mode
	sett.InverseScanMode = false;
	// enable null quiet zone
	sett.NullQuietZoneAllowed = true;
	// enable uncertain scan mode
    sett.UncertainScanMode = true	
	
	// now add sett to recognizer settings array that is used to configure
	// recognition
	return new IRecognizerSettings[] { sett };
}            
```

As can be seen from example, you can tweak PDF417 recognition parameters with properties of `PDF417RecognizerSettings`.

##### `UncertainScanMode`
By setting this property to `true`, you will enable scanning of non-standard elements, but there is no guarantee that all data will be read. This option is used when multiple rows are missing (e.g. not whole barcode is printed). Default is `false`.

##### `NullQuietZoneAllowed`
By setting this property to `true`, you will allow scanning barcodes which don't have quiet zone surrounding it (e.g. text concatenated with barcode). This option can significantly increase recognition time. Default is `false`.

##### `InverseScanMode`
By setting this to `true`, you will enable scanning of barcodes with inverse intensity values (i.e. white barcodes on dark background). This option can significantly increase recognition time. Default is `false`.

### Obtaining results from PDF417 recognizer
PDF417 recognizer produces `PDF417RecognitionResult`. You can use `is` operator to check if element in results array is instance of `PDF417RecognitionResult` class. See the following snippet for an example:

```csharp
using Microblink;

public void OnScanningDone(IList<IRecognitionResult> resultList, RecognitionType recognitionType) {   
    if (recognitionType == RecognitionType.SUCCESSFUL) {        
        foreach (var result in resultList) {
        	if (result is PDF417RecognitionResult) {			    
			    PDF417RecognitionResult pdf417Result = (PDF417RecognitionResult)result;
			    // you can use properties of PDF417RecognitionResult class to 
		        // obtain scanned information
		        if(result.Valid && !result.Empty) {
		        	bool uncertain = pdf417Result.Uncertain;
		        	string stringData = pdf417Result.StringData;
		        	BarcodeDetailedData rawData = pdf417Result.RawData;				    				    				    
			    } else {
		        	// not all relevant data was scanned, ask user
		        	// to try again
		        }   
			}            
        }                 
    }
}
```

Available properties are:

##### `bool Valid`
Set to `true` if scan result is valid, i.e. if all required elements were scanned with good confidence and can be used. If `false` is returned that indicates that some crucial data fields are missing. You should ask user to try scanning again. If you keep getting `false` (i.e. invalid data) for certain document, please report that as a bug to [help.microblink.com](http://help.microblink.com). Please include high resolution photographs of problematic documents.

##### `bool Empty`
Set to `true` if scan result is empty, i.e. nothing was scanned. All getters should return `null` for empty result.

##### `bool Uncertain`
Indicates if scanned barcode is uncertain. This can be `true` only if scanning of uncertain barcodes is allowed, as explained earlier.

##### `string StringData`
This property holds string representation of barcode contents. Note that PDF417 barcode can contain binary data so sometimes it makes little sense to obtain only string representation of barcode data.

##### `BarcodeDetailedData RawData`
This property contains information about barcode's binary layout. If you only need to access containing byte array, you can call method `GetAllData` of `BarcodeDetailedData` object.

## <a name="custom1DBarDecoder"></a> Scanning one dimensional barcodes with _PDF417_'s implementation

This section discusses the settings for setting up 1D barcode recognizer that uses _PDF417_'s implementation of scanning algorithms and explains how to obtain results from that recognizer. Henceforth, the 1D barcode recognizer that uses _PDF417_'s implementation of scanning algorithms will be refered as "Bardecoder recognizer".

### Setting up Bardecoder recognizer

To activate Bardecoder recognizer, you need to create a `BarDecoderRecognizerSettings` and add it to `IRecognizerSettings` array. You can do this using following code snippet:

```csharp
using Microblink;

private IRecognizerSettings[] setupSettingsArray() {
	BarDecoderRecognizerSettings sett = new BarDecoderRecognizerSettings();
		
	// activate scanning of Code39 barcodes
	sett.ScanCode39 = true;
	// activate scanning of Code128 barcodes
	sett.ScanCode128 = true;
	// disable inverse scan mode
	sett.InverseScanMode = false;
	// disable slower algorithm for low resolution barcodes
	sett.TryHarder = false;
	// disable autodetection of image scale
	sett.AutoScaleDetection = false;
		
	// now add sett to recognizer settings array that is used to configure
	// recognition
	return new IRecognizerSettings[] { sett };
}            
```

As can be seen from example, you can tweak Bardecoder recognition parameters with properties of `BarDecoderRecognizerSettings`.

##### `ScanCode128`
Activates or deactivates the scanning of Code128 1D barcodes. Default (initial) value is `false`.

##### `ScanCode39`
Activates or deactivates the scanning of Code39 1D barcodes. Default (initial) value is `false`.

##### `InverseScanMode`
By setting this to `true`, you will enable scanning of barcodes with inverse intensity values (i.e. white barcodes on dark background). This option can significantly increase recognition time. Default is `false`.

##### `AutoScaleDetection`
Setting this property to `true` will enable autodetection of image scale when scanning barcodes. This enables correct reading of barcodes on high resolution images but slows down the recognition process.

##### `TryHarder`
By setting this to `true`, you will enabled scanning of lower resolution barcodes at cost of additional processing time. This option can significantly increase recognition time. Default is `false`.

### Obtaining results from Bardecoder recognizer

Bardecoder recognizer produces `BarDecoderRecognitionResult`. You can use `is` operator to check if element in results array is instance of `BarDecoderRecognitionResult` class. See the following snippet for example:

```csharp
using Microblink;

public void OnScanningDone(IList<IRecognitionResult> resultList, RecognitionType recognitionType) {   
    if (recognitionType == RecognitionType.SUCCESSFUL) {        
        foreach (var result in resultList) {
        	if (result is BarDecoderRecognitionResult) {			    
			    BarDecoderRecognitionResult bardecoderResult = (BarDecoderRecognitionResult)result;
			    // you can use properties of BarDecoderRecognitionResult class to 
		        // obtain scanned information
		        if(result.Valid && !result.Empty) {
		        	BarcodeType type = bardecoderResult.BarcodeType		        	
		        	string stringData = pdf417Result.StringData;
		        	BarcodeDetailedData rawData = pdf417Result.RawData;
		        	if (BarcodeType.CODE39.Equals(type)) {
			        	string stringDataExt = pdf417Result.ExtendedStringData;
			        	BarcodeDetailedData rawDataExt = pdf417Result.ExtendedRawData;
		        	}
			    } else {
		        	// not all relevant data was scanned, ask user
		        	// to try again
		        }   
			}            
        }                 
    }
}
```

As you can see from the example, obtaining data is rather simple. You just need to call several methods of the `BarDecoderScanResult` object:

##### `bool Valid`
Set to `true` if scan result is valid, i.e. if all required elements were scanned with good confidence and can be used. If `false` is returned that indicates that some crucial data fields are missing. You should ask user to try scanning again. If you keep getting `false` (i.e. invalid data) for certain document, please report that as a bug to [help.microblink.com](http://help.microblink.com). Please include high resolution photographs of problematic documents.

##### `bool Empty`
Set to `true` if scan result is empty, i.e. nothing was scanned. All getters should return `null` for empty result.

##### `string StringData`
This property holds string representation of barcode contents.

##### `BarcodeDetailedData RawData`
This property contains information about barcode's binary layout. If you only need to access containing byte array, you can call method `GetAllData` of `BarcodeDetailedData` object.

##### `string ExtendedStringData`
This property holds string representation of extended barcode contents. This is available only if barcode that supports extended encoding mode was scanned (e.g. code39).

##### `BarcodeDetailedData ExtendedRawData`
This property contains information about barcode's binary layout when decoded in extended mode. If you only need to access containing byte array, you can call method `GetAllData` of `BarcodeDetailedData` object. This is available only if barcode that supports extended encoding mode was scanned (e.g. code39).

##### `BarcodeType`
This property is an enum that defines the type of barcode scanned.

## <a name="zxing"></a> Scanning barcodes with ZXing implementation

This section discusses the settings for setting up barcode recognizer that use ZXing's implementation of scanning algorithms and explains how to obtain results from it. _PDF417_ uses ZXing's [c++ port](https://github.com/zxing/zxing/tree/00f634024ceeee591f54e6984ea7dd666fab22ae/cpp) to support barcodes for which we still do not have our own scanning algorithms. Also, since ZXing's c++ port is not maintained anymore, we also provide updates and bugfixes to it inside our codebase.

### Setting up ZXing recognizer

To activate ZXing recognizer, you need to create a `ZXingRecognizerSettings` and add it to `IRecognizerSettings` array. You can do this using the following code snippet:

```csharp
using Microblink;

private IRecognizerSettings[] setupSettingsArray() {
	ZXingRecognizerSettings sett = new ZXingRecognizerSettings();
	
	// disable inverse scan mode
	sett.InverseScanMode = false;
	// enable scanning of QR code
	sett.QRCode = true;
	// enable scanning of ITF barcode
	sett.ITFCode = true;
	
	// now add sett to recognizer settings array that is used to configure
	// recognition
	return new IRecognizerSettings[] { sett };
}            
```

As can be seen from example, you can tweak ZXing recognition parameters with properties of `ZXingRecognizerSettings`. Note that some barcodes, such as Code 39 are available for scanning with [_PDF417_'s implementation](#custom1DBarDecoder). You can choose to use only one implementation or both (just put both settings objects into `IRecognizerSettings` array). Using both implementations increases the chance of correct barcode recognition, but requires more processing time. Of course, we recommend using the _PDF417_'s implementation for supported barcodes.

##### `AztecCode`
Activates or deactivates the scanning of Aztec 2D barcodes. Default (initial) value is `false`.

##### `Code128`
Activates or deactivates the scanning of Code128 1D barcodes. Default (initial) value is `false`.

##### `Code39`
Activates or deactivates the scanning of Code39 1D barcodes. Default (initial) value is `false`.

##### `DataMatrixCode`
Activates or deactivates the scanning of Data Matrix 2D barcodes. Default (initial) value is `false`.

##### `EAN13Code`
Activates or deactivates the scanning of EAN 13 1D barcodes. Default (initial) value is `false`.

##### `EAN8Code`
Activates or deactivates the scanning of EAN 8 1D barcodes. Default (initial) value is `false`.

##### `ITFCode`
Activates or deactivates the scanning of ITF 1D barcodes. Default (initial) value is `false`.

##### `QRCode`
Activates or deactivates the scanning of QR 2D barcodes. Default (initial) value is `false`.

##### `UPCACode`
Activates or deactivates the scanning of UPC A 1D barcodes. Default (initial) value is `false`.

##### `UPCECode`
Activates or deactivates the scanning of UPC E 1D barcodes. Default (initial) value is `false`.

##### `InverseScanMode`
By setting this to `true`, you will enable scanning of barcodes with inverse intensity values (i.e. white barcodes on dark background). This option can significantly increase recognition time. Default is `false`.

### Obtaining results from ZXing recognizer

Bardecoder recognizer produces `ZXingRecognitionResult`. You can use `is` operator to check if element in results array is instance of `ZXingRecognitionResult` class. See the following snippet for example:

```csharp
using Microblink;

public void OnScanningDone(IList<IRecognitionResult> resultList, RecognitionType recognitionType) {   
    if (recognitionType == RecognitionType.SUCCESSFUL) {        
        foreach (var result in resultList) {
        	if (result is ZXingRecognitionResult) {			    
			    ZXingRecognitionResult zxingResult = (ZXingRecognitionResult)result;
			    // you can use properties of ZXingRecognitionResult class to 
		        // obtain scanned information
		        if(result.Valid && !result.Empty) {
		        	bool uncertain = pdf417Result.Uncertain;
		        	string stringData = pdf417Result.StringData;
		        	BarcodeDetailedData rawData = pdf417Result.RawData;				    				    				    
			    } else {
		        	// not all relevant data was scanned, ask user
		        	// to try again
		        }   
			}            
        }                 
    }
}
```

As you can see from the example, obtaining data is rather simple. You just need to call several methods of the `ZXingScanResult` object:

##### `bool Valid`
Set to `true` if scan result is valid, i.e. if all required elements were scanned with good confidence and can be used. If `false` is returned that indicates that some crucial data fields are missing. You should ask user to try scanning again. If you keep getting `false` (i.e. invalid data) for certain document, please report that as a bug to [help.microblink.com](http://help.microblink.com). Please include high resolution photographs of problematic documents.

##### `bool Empty`
Set to `true` if scan result is empty, i.e. nothing was scanned. All getters should return `null` for empty result.

##### `string StringData`
This property holds string representation of barcode contents.

##### `BarcodeDetailedData RawData`
This property contains information about barcode's binary layout. If you only need to access containing byte array, you can call method `GetAllData` of `BarcodeDetailedData` object.

##### `string ExtendedStringData`
This property holds string representation of extended barcode contents. This is available only if barcode that supports extended encoding mode was scanned (e.g. code39).

##### `BarcodeDetailedData ExtendedRawData`
This property contains information about barcode's binary layout when decoded in extended mode. If you only need to access containing byte array, you can call method `GetAllData` of `BarcodeDetailedData` object. This is available only if barcode that supports extended encoding mode was scanned (e.g. code39).

##### `BarcodeType`
This property is an enum that defines the type of barcode scanned.

## <a name="usdl"></a> Scanning US Driver's licence barcodes

This section discusses the settings for setting up USDL recognizer and explains how to obtain results from it.

### Setting up USDL recognizer
To activate USDL recognizer, you need to create a `USDLRecognizerSettings` and add it to `IRecognizerSettings` array. You can do this using following code snippet:

```csharp
using Microblink;

private IRecognizerSettings[] setupSettingsArray() {
	USDLRecognizerSettings sett = new USDLRecognizerSettings();
	
	// enable null quiet zone
	sett.NullQuietZoneAllowed = true;
	// enable uncertain scan mode
    sett.UncertainScanMode = true	
	
	// now add sett to recognizer settings array that is used to configure
	// recognition
	return new IRecognizerSettings[] { sett };
}            
```

As can be seen from example, you can tweak USDL recognition parameters with properties of `USDLRecognizerSettings`.

##### `UncertainScanMode`
By setting this property to `true`, you will enable scanning of non-standard elements, but there is no guarantee that all data will be read. This option is used when multiple rows are missing (e.g. not whole barcode is printed). Default is `false`.

##### `NullQuietZoneAllowed`
By setting this property to `true`, you will allow scanning barcodes which don't have quiet zone surrounding it (e.g. text concatenated with barcode). This option can significantly increase recognition time. Default is `false`.

### Obtaining results from USDL recognizer

USDL recognizer produces `USDLRecognitionResult`. You can use `is` operator to check if element in results array is instance of `USDLRecognitionResult`. See the following snippet for an example:

```csharp
using Microblink;

public void OnScanningDone(IList<IRecognitionResult> resultList, RecognitionType recognitionType) {   
    if (recognitionType == RecognitionType.SUCCESSFUL) {        
        foreach (var result in resultList) {
        	if (result is USDLRecognitionResult) {			    
			    USDLRecognitionResult usdlResult = (USDLRecognitionResult)result;
			    // you can use properties of USDLRecognitionResult class to 
		        // obtain scanned information
		        if(result.Valid && !result.Empty) {
		        	bool uncertain = usdlResult.Uncertain;
		        	string stringData = usdlResult.StringData;
		        	BarcodeDetailedData rawData = usdlResult.RawData;

		        	// if you need specific parsed driver's license element, you can
					// use GetField method
					// for example, to obtain AAMVA version, you should use:
					string aamvaVersion = usdlResult.GetField(USDLRecognitionResult.kAamvaVersionNumber);				    				    				    
			    } else {
		        	// not all relevant data was scanned, ask user
		        	// to try again
		        }   
			}            
        }                 
    }
}
```

Available properties are:

##### `bool Valid`
Set to `true` if scan result is valid, i.e. if all required elements were scanned with good confidence and can be used. If `false` is returned that indicates that some crucial data fields are missing. You should ask user to try scanning again. If you keep getting `false` (i.e. invalid data) for certain document, please report that as a bug to [help.microblink.com](http://help.microblink.com). Please include high resolution photographs of problematic documents.

##### `bool Empty`
Set to `true` if scan result is empty, i.e. nothing was scanned. All getters should return `null` for empty result.

##### `bool Uncertain`
Indicates if scanned barcode is uncertain. This can be `true` only if scanning of uncertain barcodes is allowed, as explained earlier.

##### `string StringData`
This property holds string representation of barcode contents. Note that PDF417 barcode can contain binary data so sometimes it makes little sense to obtain only string representation of barcode data.

##### `BarcodeDetailedData RawData`
This property contains information about barcode's binary layout. If you only need to access containing byte array, you can call method `GetAllData` of `BarcodeDetailedData` object.

Method for retrieving specific driver's license element is:

##### `string GetField(string)`
This method will return a parsed US Driver's licence element. The method requires a key that defines which element should be returned and returns either a string representation of that element or `null` if that element does not exist in barcode. To see a list of available keys, refer to [Keys for obtaining US Driver's license data](DriversLicenseKeys.md)

# <a name="troubleshoot"></a> Troubleshooting

## <a name="integrationTroubleshoot"></a> Integration problems

If you have followed [Windows Phone integration instructions](#quickIntegration) and are still having integration problems, please contact us at [help.microblink.com](http://help.microblink.com).

## <a name="sdkTroubleshoot"></a> SDK problems

In case of problems with using the SDK, you should do as follows:

### Licensing problems

If you are getting "invalid license key" error or having other license-related problems (e.g. some feature is not enabled that should be or there is a watermark on top of camera), first check the log. All license-related problems are logged to error log so it is easy to determine what went wrong.

When you have determined what is the license-related problem or you simply do not understand the log, you should contact us [help.microblink.com](http://help.microblink.com). When contacting us, please make sure you provide following information:

* exact ID of your app (`Product ID` from your `WMAppManifest.xml` file)
* license key that is causing problems
* please stress out that you are reporting problem related to Windows Phone version of _PDF417_ SDK
* if unsure about the problem, you should also provide excerpt from log containing license error

### Other problems

If you are having problems with scanning certain items, undesired behaviour on specific device(s), crashes inside _PDF417_ or anything unmentioned, please do as follows:

* enable logging to get the ability to see what is library doing. To enable logging, put this line in your application:

	```csharp
	Microblink.Log.Level = Microblink.Log.LogLevel.Verbose;	
	```

	After this line, library will display as much information about its work as possible. Please save the entire log of scanning session to a file that you will send to us. It is important to send the entire log, not just the part where crash occured, because crashes are sometimes caused by unexpected behaviour in the early stage of the library initialization.
	
* Contact us at [help.microblink.com](http://help.microblink.com) describing your problem and provide following information:
	* log file obtained in previous step
	* high resolution scan/photo of the item that you are trying to scan
	* information about device that you are using - we need exact model name of the device. 
	* please stress out that you are reporting problem related to Windows Phone version of _PDF417_ SDK


# <a name="info"></a> Additional info
For any other questions, feel free to contact us at [help.microblink.com](http://help.microblink.com).
