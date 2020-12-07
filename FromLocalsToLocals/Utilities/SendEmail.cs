using FromLocalsToLocals.Database;
using SendGrid;
using SendGrid.Helpers.Mail;
using SuppLocals;
using System;
using System.Threading.Tasks;

namespace FromLocalsToLocals.Utilities
{
    public static class SendEmail
    {

        public static async Task ExceptionSender()
        {
            var key = Config.Send_Grid_Key;
            var client = new SendGridClient(key);

            var from = new EmailAddress("fromlocalstolocals@gmail.com", "Message");
            var subject = "Newsletter";
            var to = new EmailAddress("lukasstc223@gmail.com", "Dear User");
            var plainTextContent = "We have this exception:";


            var htmlContent = "<!DOCTYPE html><html><head><meta charset=\"UTF-8\"></head><body> Laiskas issiustas";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

    }
}