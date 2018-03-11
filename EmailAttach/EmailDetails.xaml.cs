using EAGetMail;
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
    /// Interaction logic for EmailDetails.xaml
    /// </summary>
    public partial class EmailDetails : Page
    {
        int j = 0;
        string LoggedInUser = string.Empty; 

        public EmailDetails(string user)
        {
            LoggedInUser = user;
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txtLoggedInUser.Text = LoggedInUser;
            }
            catch (Exception ex)
            {

            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("Login.xaml", UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex)
            {

            }
        }

        private void btnGetAttach_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
            
            string currentPath = Directory.GetCurrentDirectory();
            string attachment = String.Format("{0}\\Attachments", currentPath);
            if (!Directory.Exists(attachment))
            {
                Directory.CreateDirectory(attachment);
            }

            // Gmail IMAP4 server is "imap.gmail.com"
            MailServer oServer = new MailServer("imap.gmail.com", txtEmail.Text, txtPassword.Password, ServerProtocol.Imap4);
            MailClient oClient = new MailClient("TryIt");

            // Set SSL connection,
            oServer.SSLConnection = true;

            // Set 993 IMAP4 port
            oServer.Port = 993;

            try
            {
                oClient.Connect(oServer);

                MailInfo[] infos = oClient.GetMailInfos();
                for (int i = 0; i < infos.Length; i++)
                {
                    MailInfo info = infos[i];
                    //Console.WriteLine("Index: {0}; Size: {1}; UIDL: {2}", info.Index, info.Size, info.UIDL);

                    // Download email from GMail IMAP4 server
                    Mail oMail = oClient.GetMail(info);

                    // Console.WriteLine("From: {0}", oMail.From.ToString());
                    // Console.WriteLine("Subject: {0}\r\n", oMail.Subject);                  

                    foreach (var item in oMail.Attachments)
                    {
                        item.SaveAs(item.Name, true);
                    }
                    // Generate an email file name based on date time.
                    //System.DateTime d = System.DateTime.Now;
                    //System.Globalization.CultureInfo cur = new System.Globalization.CultureInfo("en-US");
                    //string sdate = d.ToString("yyyyMMddHHmmss", cur);
                    //string fileName = String.Format("{0}\\{1}{2}{3}.eml", mailbox, sdate, d.Millisecond.ToString("d3"), i);

                    // Save email to local disk
                    //oMail.SaveAs(fileName, true);

                    // Mark email as deleted in GMail account.
                    //oClient.Delete(info);

                    j++;
                }

                // Quit and purge emails marked as deleted from Gmail IMAP4 server.
                oClient.Quit();


            }
            catch (Exception ep)
            {
                Console.WriteLine(ep.Message);
            }
        }         
    }
}
