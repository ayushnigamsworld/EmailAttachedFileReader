using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GmailAPI
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials 
        // at ~/.credentials/gmail-dotnet-quickstart.json 
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Gmail API .NET Quickstart";

        //static void Main(string[] args)
        //{
        //    UserCredential credential;

        //    using (var stream =
        //        new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
        //    {
        //        string credPath = System.Environment.GetFolderPath(
        //            System.Environment.SpecialFolder.Personal);
        //        credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart.json");

        //        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //            GoogleClientSecrets.Load(stream).Secrets,
        //            Scopes,
        //            "user",
        //            CancellationToken.None,
        //            new FileDataStore(credPath, true)).Result;
        //        Console.WriteLine("Credential file saved to: " + credPath);
        //    }

        //    // Create Gmail API service.
        //    var service = new GmailService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = ApplicationName,
        //    });

        //    // Define parameters of request.
        //    UsersResource.LabelsResource.ListRequest request = service.Users.Labels.List("me");

        //    // List labels.
        //    IList<Label> labels = request.Execute().Labels;
        //    Console.WriteLine("Labels:");
        //    if (labels != null && labels.Count > 0)
        //    {
        //        foreach (var labelItem in labels)
        //        {
        //            Console.WriteLine("{0}", labelItem.Name);
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("No labels found.");
        //    }
        //    Console.Read();

        //}


        static void Main(string[] args)
        {

            UserCredential credential;

            using (var stream =
              new FileStream("Targaryian_Client_Id.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                  System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,Scopes,"user",CancellationToken.None,new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service. 
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });


            var inboxlistRequest = service.Users.Messages.List("targaryian@gmail.com");
            inboxlistRequest.LabelIds = "INBOX";
            inboxlistRequest.IncludeSpamTrash = false;

            //get our emails 
            var emailListResponse = inboxlistRequest.Execute();

            if (emailListResponse != null && emailListResponse.Messages != null)
            {
                //loop through each email and get what fields you want... 
                foreach (var email in emailListResponse.Messages)
                {

                    var emailInfoRequest = service.Users.Messages.Get("targaryian@gmail.com", email.Id);
                    var emailInfoResponse = emailInfoRequest.Execute();

                    //if (emailInfoResponse != null)
                    //{
                    //    String from = "";
                    //    String date = "";
                    //    String subject = "";

                    //    //loop through the headers to get from,date,subject, body  
                    //    foreach (var mParts in emailInfoResponse.Payload.Headers)
                    //    {
                    //        if (mParts.Name == "Date")
                    //        {
                    //            date = mParts.Value;
                    //        }
                    //        else if (mParts.Name == "From")
                    //        {
                    //            from = mParts.Value;
                    //        }
                    //        else if (mParts.Name == "Subject")
                    //        {
                    //            subject = mParts.Value;
                    //        }

                    //        if (date != "" && from != "")
                    //        {

                    //            foreach (MessagePart p in emailInfoResponse.Payload.Parts)
                    //            {
                    //                if (p.MimeType == "text/html")
                    //                {
                    //                    byte[] data = FromBase64ForUrlString(p.Body.Data);
                    //                    string decodedString = Encoding.UTF8.GetString(data);

                    //                }
                    //            }



                    //        }

                    //    }
                    //}
                    string savePath = @"C:\Users\amit\Desktop\Attachments";
                    GetAttachments(service, "targaryian@gmail.com", emailInfoResponse.Id, savePath);
                }

            }
            Console.ReadLine();

        }

        public static byte[] FromBase64ForUrlString(string base64ForUrlInput)
        {
            int padChars = (base64ForUrlInput.Length % 4) == 0 ? 0 : (4 - (base64ForUrlInput.Length % 4));
            StringBuilder result = new StringBuilder(base64ForUrlInput, base64ForUrlInput.Length + padChars);
            result.Append(String.Empty.PadRight(padChars, '='));
            result.Replace('-', '+');
            result.Replace('_', '/');
            string temp = result.ToString();
            return Convert.FromBase64String(result.ToString());
        }

        // ...

        /// <summary>
        /// Get and store attachment from Message with given ID.
        /// </summary>
        /// <param name="service">Gmail API service instance.</param>
        /// <param name="userId">User's email address. The special value "me"
        /// can be used to indicate the authenticated user.</param>
        /// <param name="messageId">ID of Message containing attachment.</param>
        /// <param name="outputDir">Directory used to store attachments.</param>
        public static void GetAttachments(GmailService service, String userId, String messageId, String outputDir)
        {
            try
            {
                Message message = service.Users.Messages.Get(userId, messageId).Execute();
                IList<MessagePart> parts = message.Payload.Parts;
                foreach (MessagePart part in parts)
                {
                    if (!String.IsNullOrEmpty(part.Filename))
                    {
                        String attId = part.Body.AttachmentId;
                        MessagePartBody attachPart = service.Users.Messages.Attachments.Get(userId, messageId, attId).Execute();

                        // Converting from RFC 4648 base64 to base64url encoding
                        // see http://en.wikipedia.org/wiki/Base64#Implementations_and_history
                        String attachData = attachPart.Data.Replace('-', '+');
                        attachData = attachData.Replace('_', '/');

                        byte[] data = Convert.FromBase64String(attachData);
                        File.WriteAllBytes(Path.Combine(outputDir, part.Filename), data);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }
    }
}
