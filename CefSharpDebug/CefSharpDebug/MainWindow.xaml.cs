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
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using CefSharp;
    using CefSharp.Wpf;

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
                //Debugger.Launch();
                this.InitializeCefSharpWebControl();
                this.InitializeComponent();
                this.Loaded += this.MainWindowLoaded;
                this.GotFocus += this.MainWindow_GotFocus;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Error");
            }
        }

        /// <summary>
        /// The main window_ got focus.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        void MainWindow_GotFocus(object sender, RoutedEventArgs e)
        {
            this.queryResultsWebBrowser.Focus();
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
            this.Url = "http://10.78.15.29:8040/Provisioning/Query/reportPage.aspx?id=a7c12572-c829-456f-98e5-f4a4527c27c9";
            this.SetupCefSharpWebControl();
        }

        /// <summary>
        /// The setup CEF sharp web control.
        /// </summary>
        /// <param name="chromiumWebBrowser">
        /// The chromium web browser.
        /// </param>
        private void SetupCefSharpWebControl()
        {
            try
            {
                //this.jscallbacksManager = new JavaScriptCallbacksManager(this.queryResultsWebBrowser);

                //// Register the callback object
                //this.queryResultsWebBrowser.RegisterJsObject("callbackObj", this.reportShareCallbackObject);

                this.queryResultsWebBrowser.FrameLoadEnd += this.ChromiumWebBrowserFrameLoadEnd;
                this.queryResultsWebBrowser.KeyDown += this.QueryResultsWebBrowserOnPreviewKeyDown;
                //IRequestHandler requestHandler = new WebRequestHandler();
                //this.queryResultsWebBrowser.RequestHandler = requestHandler;

                this.queryResultsWebBrowser.Loaded += this.ChromiumWebBrowserLoaded;
                this.queryResultsWebBrowser.PreviewTextInput += this.QueryResultsWebBrowserPreviewTextInput;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Error");
            }
        }

        private void QueryResultsWebBrowserOnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            var param = KeyInterop.VirtualKeyFromKey(keyEventArgs.Key);

            this.queryResultsWebBrowser.SendKeyEvent((int)WM.KEYDOWN, param, 0);
        }

        void QueryResultsWebBrowserPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (var character in e.Text)
            {
                this.queryResultsWebBrowser.SendKeyEvent((int)WM.CHAR, character, 0);
            }

            e.Handled = true;
        }

        private void ChromiumWebBrowserFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            // JavaScript Callback Manager injects the callback object into Chromium Web browser.
            Application.Current.Dispatcher.Invoke(new Action(this.Target));
        }

        /// <summary>
        /// The target.
        /// </summary>
        private void Target()
        {
            this.queryResultsWebBrowser.Focus();
        }

        /// <summary>
        /// The chromium web browser loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        private void ChromiumWebBrowserLoaded(object sender, RoutedEventArgs e)
        {
            this.queryResultsWebBrowser.Focus();
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
            //popup.IsOpen = true;
        }


    }
}
