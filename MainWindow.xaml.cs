using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Desktop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace MsalWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string CommonAuthorityUri = "https://login.microsoftonline.com/common";
        private const string ClientId = "d3590ed6-52b3-4102-aeff-aad2292ab01c";
        private const string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";
        private const string GraphResourceUri = "https://graph.microsoft.com";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AuthenticationResult result = await AcquireTokenAsync(new[] { $"{GraphResourceUri}/.default" });
                SignInResult.Text = $"Signed in as {result.Account.Username}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(GetWindow(this), ex.ToString(), $"Error: {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task<AuthenticationResult> AcquireTokenAsync(IEnumerable<string> scopes)
        {
            PublicClientApplicationBuilder builder =
                PublicClientApplicationBuilder.Create(ClientId)
                .WithAuthority(CommonAuthorityUri)
                .WithRedirectUri(RedirectUri)
                .WithBroker(new BrokerOptions(BrokerOptions.OperatingSystems.Windows)); //WAM broker fails when process is elevated.

            IPublicClientApplication pca = builder.Build();
            IntPtr parentHwnd = new WindowInteropHelper(GetWindow(this)).Handle;
            return await pca.AcquireTokenInteractive(scopes).WithParentActivityOrWindow(parentHwnd).ExecuteAsync();
        }
    }
}
