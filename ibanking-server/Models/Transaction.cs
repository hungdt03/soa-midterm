using ibanking_server.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ibanking_server.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double Amount { get; set; }
        public string Content { get; set; }
        public DateTime StartTransactionTime { get; set; }
        public DateTime EndTransactionTime { get; set; }
        public TransactionType TransactionType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
