using FromLocalsToLocals.Database;
using SendGrid;
using SendGrid.Helpers.Mail;
using SuppLocals;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromLocalsToLocals.Utilities
{
    public class SendAllSubscribers
    {

            

        private readonly AppDbContext _context;


        public SendAllSubscribers(AppDbContext context)
        {

            _context = context;

        }

        public async void SendingAll()
        {
            var users = _context.Users.ToList();
            var vendoriai = _context.Vendors;
            string emaily;
            var list = _context.Vendors.OrderByDescending(v => v.DateCreated).Take(3);
            StringBuilder msg = new StringBuilder();

            if (vendoriai == null)
            {
                msg.Append("At this time there is not new vendors");
            }

            else
            {
                foreach (var u in users)
                {

                    if (u.Subscribe is true)
                    {
                        emaily = u.Email;
                        foreach (var vendory in list)
                        {
                            msg.Append(vendory.Title.ToString());
                            msg.Append("  ");
                            msg.Append(vendory.DateCreated);
                            msg.Append(" <br>");
                        }

                        await SendEmail.NewsLetterSender(msg.ToString(), emaily);
                    }

                }
            }
        }



    }
}
