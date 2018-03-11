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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGetAllAttach_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    string emailAdd = txtEmail.Text;
            //    string password = txtPassword.Password;
            //    using (Imap imap = new Imap())
            //    {
            //        imap.Connect("imap.gmail.com");   // or ConnectSSL for SSL
            //        imap.UseBestLogin(emailAdd, password);
            //        imap.SelectInbox();
            //        List<long> uids = imap.Search(Flag.All);
            //        foreach (long uid in uids)
            //        {
            //            IMail email = new MailBuilder()
            //                .CreateFromEml(imap.GetMessageByUID(uid));

            //            Console.WriteLine(email.Subject);

            //            // save all attachments to disk
            //            foreach (MimeData mime in email.Attachments)
            //            {
            //                mime.Save(mime.SafeFileName);
            //            }
            //        }
            //        imap.Close();
            //    }

            //}
            //catch (Exception ex)
            //{
            //aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
            //}


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
                MailInfo[] infos = oClient.GetMailInfos();
                for (int i = 0; i < infos.Length; i++)
                {
                    MailInfo info = infos[i];
                    Console.WriteLine("Index: {0}; Size: {1}; UIDL: {2}",
                        info.Index, info.Size, info.UIDL);

                    // Download email from GMail IMAP4 server
                    Mail oMail = oClient.GetMail(info);

                    Console.WriteLine("From: {0}", oMail.From.ToString());
                    Console.WriteLine("Subject: {0}\r\n", oMail.Subject);

                    string[] attachmentName = new string[10];
                    foreach (var item in oMail.Attachments)
                    {                        
                        attachmentName[j] = item.Name;                        
                    }

                    foreach (var item in oMail.Attachments)
                    {

                        item.SaveAs(item.Name,true);
                    }

                    // Generate an email file name based on date time.
                    System.DateTime d = System.DateTime.Now;
                    System.Globalization.CultureInfo cur = new
                        System.Globalization.CultureInfo("en-US");
                    string sdate = d.ToString("yyyyMMddHHmmss", cur);
                    string fileName = String.Format("{0}\\{1}{2}{3}.eml",
                        mailbox, sdate, d.Millisecond.ToString("d3"), i);

                    // Save email to local disk
                    oMail.SaveAs(fileName, true);

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
