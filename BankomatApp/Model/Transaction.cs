using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BankomatApp.Model
{
    internal class Transaction
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string CardNumber { get; set; }
        public DateTime DateTime { get; set; }
        public string BankomatNumber { get; set; }
        public double Amount { get; set; }
        public double Remains { get; set; }
        public Transaction(int type, string cardNumber, string bankomatNumber, double amount, double remains)
        {
            Type = type;
            CardNumber = Regex.Replace(cardNumber, @"\d(?=.*\d{4})", "*");
            DateTime = DateTime.Now;
            BankomatNumber = bankomatNumber;
            Amount = amount;
            Remains = remains;
        }
    }
}
