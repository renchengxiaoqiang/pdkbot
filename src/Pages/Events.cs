using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PdkBot.Pages
{
    public class ReceieveNewMessageEventArgs
    {
        public ReceieveNewMessageEventArgs(string message)
        {
            Message = message;
        }
        public string Message { get; set; }
    }
    public class PageSelectionChangedEventArgs
    {
        public PageSelectionChangedEventArgs(Page page,TabItem tab)
        {
            this.Page = page;
            this.Tab = tab;
        }
        public Page Page { get; set; }
        public TabItem Tab { get; set; }
    }

    //public class BuyerSwitchedEventArgs
    //{
    //    public Conversation Buyer;
    //}
    //public class Conversation
    //{
    //    public string Buyer;
    //    public string Seller;
    //}
}
