using EAGetMail;
using Limilabs.Client.IMAP;
using Limilabs.Mail;
using Limilabs.Mail.MIME;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace EmailAttach
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables
        NewMailWatcher _objMailWatcher = null;
        static MainWindow _objMainWindow = null;
        Action HandlNewMail;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Init()
        {
            try
            {
                HandlNewMail = NewMailHandler;
                _objMainWindow = this;
                _objMailWatcher = new NewMailWatcher();
                _objMailWatcher.NewMail += _objMailWatcher_NewMail;
                _objMailWatcher.StartReceiving(txtEmail.Text,txtPassword.Password);
            }
            catch (Exception ex)
            {
            }
        }

        private void NewMailHandler()
        {
            btnGetAllAttach_Click(null, null);
        }
        private void _objMailWatcher_NewMail()
        {
            _objMainWindow.Dispatcher.Invoke(HandlNewMail);
        }

        private void btnGetAllAttach_Click(object sender, RoutedEventArgs e)
        {
            // Create a folder named "inbox" under current directory
            // to save the email retrieved.
            int j = 0;
            string curpath = Directory.GetCurrentDirectory();
            string mailbox = String.Format("{0}\\inbox", curpath);

            // If the folder is not existed, create it.
            if (!Directory.Exists(mailbox))
            {
                Directory.CreateDirectory(mailbox);
            }

            // Gmail IMAP4 server is "imap.gmail.com"
            MailServer oServer = new MailServer("imap.gmail.com",
                        txtEmail.Text, txtPassword.Password, ServerProtocol.Imap4);
            MailClient oClient = new MailClient("TryIt");

            // Set SSL connection,
            oServer.SSLConnection = true;

            // Set 993 IMAP4 port
            oServer.Port = 993;

            try
            {
                oClient.Connect(oServer);

                List<MailInfo> lstInfo = oClient.GetMailInfos().Where(info=>oClient.GetMail(info).ReceivedDate.Date==DateTime.Now.Date).ToList();

                foreach(MailInfo info in lstInfo)
                {
                    Mail thismail = oClient.GetMail(info);
                    foreach (var item in thismail.Attachments)
                    {
                        item.SaveAs(item.Name, true);
                    }
                }
                // Quit and purge emails marked as deleted from Gmail IMAP4 server.
                oClient.Quit();

                
            }
            catch (Exception ep)
            {
                Console.WriteLine(ep.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtEmail.Text = "targaryian@gmail.com";
            txtPassword.Password = "asdfghjklmnbvcxz";
            Init();
        }
    }
}
