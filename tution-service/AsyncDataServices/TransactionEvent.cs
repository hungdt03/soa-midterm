namespace tution_service.AsyncDataServices
{
    public class TransactionEvent
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string Content { get; set; }
        public DateTime StartTransactionTime { get; set; }
        public DateTime EndTransactionTime { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
    }
}
