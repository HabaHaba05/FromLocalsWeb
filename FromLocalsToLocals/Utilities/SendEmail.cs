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

        public static async Task NewsLetterSender(string msge, string email)
        {
            var key = Config.Send_Grid_Key;
            var client = new SendGridClient(key);

            var from = new EmailAddress("fromlocalstolocals@gmail.com", "Forgot password");
            var subject = "Forgot Password Confirmation";
            var to = new EmailAddress(email, "Dear User");
            var plainTextContent = "";


            var htmlContent = "<!DOCTYPE html><html><head></head><body style=\"background-color: #CCBA8B;\">" +
            "<table class=\"body-wrap\"  style=\"background-color: #CCBA8B;\" ><tr><td class=\"container\">" +
            "<table><tr><td align=\"center\" class=\"masthead\">" +
            "<a href=\"https://ibb.co/6HHkSvm\"><img src=\"https://i.ibb.co/0CCxLBc/appLogo.png\" alt=\"appLogo\" border=\"0\" /></a>" +
            "<h1>From Locals to Locals</h1></td></tr><tr><td class=\"content\"><b><div style=\"text-align: center;\">"
            + msge + "</div></b></body></html>" +
            "</td></tr></table></td></tr></table></body></html>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }



    }
}