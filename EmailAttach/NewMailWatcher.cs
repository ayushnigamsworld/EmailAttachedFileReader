using S22.Imap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EmailAttach
{
    public class NewMailWatcher
    {
        public event Action NewMail;
        public void StartReceiving(string mailid,string password)
        {
            try
            {
                Task.Run(() =>
                {
                    using (ImapClient client = new ImapClient("imap.gmail.com", 993, mailid, password,
                        AuthMethod.Login, true))
                    {
                        if (client.Supports("IDLE") == false)
                        {
                            return;
                        }
                        client.NewMessage += Client_NewMessage;
                        while (true) ;
                    }
                });
            }
            catch (Exception ex)
            {
            }
        }

        private void Client_NewMessage(object sender, IdleMessageEventArgs e)
        {
            NewMail?.Invoke();
        }
    }
}
