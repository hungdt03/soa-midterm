namespace tution_service.Dtos.ClientDtos
{
    public class TransactionRequest
    {
        public double Amount { get; set; }
        public string Content { get; set; }
        public int TutionId { get; set; }
    }
}
