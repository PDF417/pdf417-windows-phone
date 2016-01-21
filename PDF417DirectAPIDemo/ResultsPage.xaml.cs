using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PDF417DirectAPIDemo
{
    /// <summary>
    /// Page that displays recognition results
    /// </summary>
    public partial class ResultsPage : PhoneApplicationPage
    {

        /// <summary>
        /// static fields for sending result data to the page
        /// </summary>
        public static IReadOnlyDictionary<string, object> results;
        public static bool resultFound;

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
            // if scan successful try to display results
            if (resultFound) {            
                // if results have been passed copy them to form fields
                if (results != null) {
                    mMainPanel.Children.Clear();
                    foreach (string key in results.Keys) {
                        if (results[key] != null) {
                            string value = results[key].ToString();
                            if (!value.Trim().Equals("")) { 
                            StackPanel panel = new StackPanel();
                            TextBlock title = new TextBlock() {
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                                TextWrapping = TextWrapping.Wrap,
                                Text = key
                            };
                            TextBox text = new TextBox() {
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                                TextWrapping = TextWrapping.Wrap,
                                Text = value
                            };                        
                            panel.Children.Add(title);
                            panel.Children.Add(text);
                            mMainPanel.Children.Add(panel);
                        }
                        }
                    }
                    results = null;
                }
            } else {
                // display results not found message
                mMainPanel.Children.Clear();
                TextBlock msg = new TextBlock() {
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                    TextAlignment = System.Windows.TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Text = "Image scan was unsuccessful"
                };
                mMainPanel.Children.Add(msg);
            }
        }

    }
}