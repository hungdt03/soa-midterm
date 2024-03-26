using System.ComponentModel.DataAnnotations;

namespace ibanking_server.Dtos
{
    public class TransactionRequest
    {
        public double Amount { get; set; }
        public string Content { get; set; }
        public int TutionId { get; set; }
    }
}
