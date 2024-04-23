namespace Enhancing_ECommerce_Security_A_Passwordless_Approach.Services
{
    public class EmailSettings
    {
        public required string MailServer { get; set; }
        public required int MailPort { get; set; }
        public required string SenderName { get; set; }
        public required string Sender { get; set; }
        public required string Password { get; set; }
    }
}
