using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareDtos
{
    public class TransactionSender
    {
        public int TransactionId { get; set; }
        public int TutionId { get; set; }
        public double Amount { get; set; }
        public bool IsSuccess { get; set; }
        public string Content { get; set; }
        public DateTime StartTransactionTime { get; set; }
        public DateTime EndTransactionTime { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
    }
}
