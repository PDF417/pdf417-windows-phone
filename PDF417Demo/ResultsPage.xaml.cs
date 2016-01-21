using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PDF417Demo
{
    /// <summary>
    /// Page that displays recognition results
    /// </summary>
    public partial class ResultsPage : PhoneApplicationPage
    {

        /// <summary>
        /// static fields for sending result data to the page
        /// </summary>
        public static string dataType;
        public static Microblink.BarcodeDetailedData raw;
        public static Microblink.BarcodeDetailedData rawExt;
        public static string stringData;
        public static string stringDataExt;
        public static string uncertain;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ResultsPage() {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the button click event by
        /// navigating back to the main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) {
            // navigate back to main page
            NavigationService.GoBack();
        }

        /// <summary>
        /// Called when this page is navigated to.
        /// Fills out form fields with recognition results.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            // call default behaviour
            base.OnNavigatedTo(e);
            // if results have been passed copy them to form fields
            if (dataType != null) {
                mDataType.Text = dataType;
                dataType = null;
            }
            if (uncertain != null) {
                mUncertainPanel.Visibility = System.Windows.Visibility.Visible;
                mUncertain.Text = uncertain;
                uncertain = null;
            } else {
                mUncertainPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (stringData != null && !stringData.Trim().Equals("")) {
                mRawPanel.Visibility = System.Windows.Visibility.Visible;
                mRawBox.Text = stringData;
                stringData = null;
            } else {
                mRawPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (raw != null && raw.Elements.Count > 0) {
                mDetailsPanel.Children.Clear();
                TextBlock header = new TextBlock() { Text = "Data Details", HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, TextWrapping = TextWrapping.Wrap };
                mDetailsPanel.Children.Add(header);
                foreach (var elem in raw.Elements) {
                    string txt = "<no data>";
                    if (elem.Type == Microblink.BarcodeElementType.TEXT_DATA) {
                        txt = elem.Text;                        
                    } else if (elem.Type == Microblink.BarcodeElementType.BYTE_DATA) {
                        txt = "{ ";
                        foreach (var b in elem.Bytes) {
                            txt += b;
                            txt += " ";
                        }
                        txt += "}";
                    }
                    TextBox txtBox = new TextBox() { Text = txt, HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, TextWrapping = TextWrapping.Wrap, Height = 111.0, FontSize = 20.0 };
                    mDetailsPanel.Children.Add(txtBox);
                }                
                mDetailsPanel.Visibility = System.Windows.Visibility.Visible;
                raw = null;
            } else {
                mDetailsPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (stringDataExt != null && !stringDataExt.Trim().Equals("")) {
                mRawExtPanel.Visibility = System.Windows.Visibility.Visible;
                mRawExtBox.Text = stringDataExt;
                stringDataExt = null;
            } else {
                mRawExtPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (rawExt != null && rawExt.Elements.Count > 0) {
                mDetailsExtPanel.Children.Clear();
                TextBlock header = new TextBlock() { Text = "Extended Data Details", HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, TextWrapping = TextWrapping.Wrap };
                mDetailsExtPanel.Children.Add(header);
                foreach (var elem in rawExt.Elements) {
                    string txt = "<no data>";
                    if (elem.Type == Microblink.BarcodeElementType.TEXT_DATA) {
                        txt = elem.Text;
                    } else if (elem.Type == Microblink.BarcodeElementType.BYTE_DATA) {
                        txt = "{ ";
                        foreach (var b in elem.Bytes) {
                            txt += b;
                            txt += " ";
                        }
                        txt += "}";
                    }
                    TextBox txtBox = new TextBox() { Text = txt, HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, TextWrapping = TextWrapping.Wrap, Height = 111.0, FontSize = 20.0 };
                    mDetailsExtPanel.Children.Add(txtBox);
                }
                mDetailsExtPanel.Visibility = System.Windows.Visibility.Visible;
                rawExt = null;
            } else {
                mDetailsExtPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

    }
}