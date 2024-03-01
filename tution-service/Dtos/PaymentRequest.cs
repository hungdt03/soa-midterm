namespace tution_service.Dtos
{
    public class PaymentRequest
    {
        public int StudentId { get; set; }
        public int TutionId { get; set; }
        public string Content { get; set; }
    }
}
