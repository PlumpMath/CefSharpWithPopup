// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Test">
//   Test
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CefSharpDebug
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows;

    using CefSharp;

    /// <summary>
    /// Interaction logic for 
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The CEFSharp logs path.
        /// </summary>
        private const string CefLogs = "Logs\\cef-{0}.log";

        /// <summary>
        /// The CEFSharp locales path.
        /// </summary>
        private const string CefLocales = "locales\\";

        /// <summary>
        /// The error message if CEF not initialized.
        /// </summary>
        private const string ErrorCefNotInitialized = "Error occurred while initializing CefSharp Web-Control.";

        /// <summary>
        /// The CEFSharp Browser Process application executable.
        /// </summary>
        private const string CefSharpBrowserProcessExe = "CefSharp.BrowserSubprocess.exe";

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            try
            {
                this.InitializeCefSharpWebControl();
                this.InitializeComponent();
                this.Loaded += this.MainWindowLoaded;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Error");
            }
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }
        

        /// <summary>
        /// The main window loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;
            this.Url = "www.google.com";
        }

        /// <summary>
        /// Initializes CEFSharp web control.
        /// </summary>
        private void InitializeCefSharpWebControl()
        {
            var assemblyDir = this.GetAssemblyDirectory();

            var cefSettings = new CefSettings
            {
                PackLoadingDisabled = false,
                LogFile = Path.Combine(assemblyDir, string.Format(CefLogs, DateTime.Now.Millisecond)),
                LocalesDirPath = Path.Combine(assemblyDir, CefLocales),
                BrowserSubprocessPath = Path.Combine(assemblyDir, CefSharpBrowserProcessExe)
            };
            Cef.Initialize(cefSettings);
            var isCefInitialized = Cef.IsInitialized;


            if (!isCefInitialized)
            {
                MessageBox.Show(
                    this,
                    ErrorCefNotInitialized,
                    "CEFSharp Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Gets current assembly directory path
        /// </summary>
        /// <returns>Returns current executing assembly directory.</returns>
        private string GetAssemblyDirectory()
        {
            var assemblyDir = Assembly.GetAssembly(typeof(MainWindow)).Location;
            return Path.GetDirectoryName(assemblyDir);
        }

        /// <summary>
        /// The query results web browser_ on frame load start.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void QueryResultsWebBrowser_OnFrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            this.Dispatcher.Invoke(
                delegate
                {
                    this.queryResultsWebBrowser.Visibility = Visibility.Collapsed;
                    LoadingTextBlock.Visibility = Visibility.Visible;
                });
        }

        /// <summary>
        /// The query results web browser_ on frame load end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void QueryResultsWebBrowser_OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            this.Dispatcher.Invoke(
                delegate
                {
                    this.queryResultsWebBrowser.Visibility = Visibility.Visible;
                    LoadingTextBlock.Visibility = Visibility.Collapsed;
                });
        }

        /// <summary>
        /// The popup button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PopupButton_OnClick(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = true;
        }
    }
}
