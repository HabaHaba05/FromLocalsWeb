using FromLocalsToLocals.Models;
using Microsoft.AspNetCore.Http;
using SendGrid;
using SendGrid.Helpers.Mail;
using SuppLocals;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FromLocalsToLocals.Utilities
{
    public static class ExtensionMethods
    {
        public static T ParseEnum<T>(this string value) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static bool ValidImage(this IFormFile image)
        {
            if (image != null)
            {
                if (image.Length > 0)
                {
                    var ext = Path.GetExtension(image.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(ext) || Config.permittedExtensions.Contains(ext))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static byte[] ConvertToBytes(this IFormFile image)
        {
            if (image != null)
            {
                if (image.Length > 0)
                {
                    var ext = Path.GetExtension(image.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(ext) || Config.permittedExtensions.Contains(ext))
                    {
                        using (var target = new MemoryStream())
                        {
                            image.CopyTo(target);
                            return target.ToArray();
                        
                        }
                    }
                }
            }
            return null;
        }

        public static async Task ExceptionSender(this Exception e)
        {
            var key = Config.Send_Grid_Key;
            var client = new SendGridClient(key);

            var from = new EmailAddress("fromlocalstolocals@gmail.com", "Exception message");
            var subject = "Exception receiver";
            var to = new EmailAddress("lukasstc223@gmail.com", "Dear User");
            var plainTextContent = "We have this exception:";


            var htmlContent = "<!DOCTYPE html><html><head><meta charset=\"UTF-8\"></head><body> " + e.ToString();
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

    }
}
