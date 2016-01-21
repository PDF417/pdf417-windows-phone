using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PDF417DirectAPIDemo.Resources;
using System.Text;
using Microblink.UserControls;
using System.Windows.Media.Imaging;
using Microblink;
using Microsoft.Phone.Tasks;
using Microblink.Recognition;

namespace PDF417DirectAPIDemo
{
    /// <summary>
    /// Main application page containing RecognizerControl
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {

        private PhotoChooserTask photoChooserTask;        

        /// <summary>
        /// Handles completed scanning events.
        /// Navigates to results page if scanning was successful.
        /// </summary>
        /// <param name="resultList">list of recognition results</param>
        /// <param name="recognitionType">type of recognition</param>
        void mRecognizer_OnScanningDone(IList<Microblink.IRecognitionResult> resultList, RecognitionType recognitionType) {            
            // terminate direct API
            Recognizer.GetSingletonInstance().Terminate();
            // navigate to results page
            bool resultFound = false;
            if (recognitionType == RecognitionType.SUCCESSFUL) {
                // Find croatian payslip results in list of results.                
                foreach (var result in resultList) {                   
                    if (result.Valid && !result.Empty) {
                        // check if result is a PDF417 result
                        if (result is Microblink.PDF417RecognitionResult) {
                            // obtain the PDF417 result
                            Microblink.PDF417RecognitionResult pdf417Result = (Microblink.PDF417RecognitionResult)result;
                            // set it as input for results page
                            ResultsPage.results = pdf417Result.Elements;
                            // mark as found
                            resultFound = true;
                            break;
                        }
                            // check if result is a ZXing result
                        else if (result is Microblink.ZXingRecognitionResult) {
                            // obtain the ZXing result
                            Microblink.ZXingRecognitionResult zxingResult = (Microblink.ZXingRecognitionResult)result;
                            // set it as input for results page
                            ResultsPage.results = zxingResult.Elements;
                            resultFound = true;
                            break;
                        }
                    }
                }                
            }
            // send scan status to results page
            ResultsPage.resultFound = resultFound;
            // navigate to results page                                    
            NavigationService.Navigate(new Uri("/ResultsPage.xaml", UriKind.Relative));
            // reenable photo choosing
            ReenableButton();
        }        

        /// <summary>
        /// Default construtor.
        /// Initializes photo chooser task
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += photoChooserTask_Completed;            
        }

        /// <summary>
        /// When user chooses a photo starts the recognition process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void photoChooserTask_Completed(object sender, PhotoResult e) {
            if (e.TaskResult == TaskResult.OK) {
                SetScanInProgress();
                BitmapSource image = new BitmapImage();
                image.SetSource(e.ChosenPhoto);

                // setup direct API
                Recognizer directRecognizer = Recognizer.GetSingletonInstance();               
                // register event handlers
                if (directRecognizer.CurrentState == RecognizerDirectAPIState.OFFLINE) { 
                    directRecognizer.OnScanningDone += mRecognizer_OnScanningDone;
                    // unlock direct API
                    try {
                        directRecognizer.LicenseKey = "Add license key here";
                    }
                    catch (InvalidLicenseKeyException exception) {
                        MessageBox.Show("Could not unlock API! Invalid license key!\nThe application will now terminate!");
                        Application.Current.Terminate();
                    }
                }
                // add MRTD recognizer settings
                if (directRecognizer.CurrentState == RecognizerDirectAPIState.UNLOCKED) {
                    // add PDF417 & ZXing recognizer settings
                    Microblink.PDF417RecognizerSettings pdf417Settings = new Microblink.PDF417RecognizerSettings() {
                        InverseScanMode = false,
                        NullQuietZoneAllowed = true,
                        UncertainScanMode = true
                    };
                    Microblink.ZXingRecognizerSettings zxingSettings = new Microblink.ZXingRecognizerSettings() {
                        QRCode = true
                    };
                    directRecognizer.Initialize(new GenericRecognizerSettings(), new Microblink.IRecognizerSettings[] { pdf417Settings, zxingSettings });
                }
                // start recognition
                directRecognizer.Recognize(image);             
            }
        }        

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        /// <summary>
        /// Called when this page is navigated to.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            // call default behaviour
            base.OnNavigatedTo(e);            
        }

        /// <summary>
        /// Called when the user leaves this page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {            
            // call default behaviour
            base.OnNavigatingFrom(e);
        }               

        /// <summary>
        /// Opens a photo chooser page on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) {
            photoChooserTask.Show();
        }

        /// <summary>
        /// displays the "in progress" message and disables the button
        /// </summary>
        private void SetScanInProgress() {
            mButton.Visibility = System.Windows.Visibility.Collapsed;
            mMessage.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// hides the "in progress" message and reenables the button
        /// </summary>
        private void ReenableButton() {
            mButton.Visibility = System.Windows.Visibility.Visible;
            mMessage.Visibility = System.Windows.Visibility.Collapsed;
        }
                    
    }
}