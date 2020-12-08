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

        public SendAllSubscribers(AppDbContext context){
            _context = context;      
        }

        public async Task SendingAll()
        {
            var users = _context.Users.ToList();
            var vendors = _context.Vendors;
            var list = _context.Vendors.OrderByDescending(v => v.DateCreated).Take(3);

            StringBuilder msg = new StringBuilder();

            if (vendors == null)
            {
                msg.Append("At this time there is not new vendors");
            }

            else
            {
                foreach (var u in users)
                {

                    if (u.Subscribe is true)
                    {
                        foreach (var v in list)
                        {
                            msg.Append(v.Title.ToString());
                            msg.Append("    Vendor type: ");
                            msg.Append(v.VendorType);
                            msg.Append(" <br>");
                        }
                    }
                    await SendEmail.NewsLetterSender(msg.ToString(), u.Email);
                }
            }
        }



    }
}
