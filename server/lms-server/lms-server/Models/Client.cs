namespace lms_server.Models
{
    public class Client
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string? Address { get; set; }

        public string PhoneNumber { get; set; }
    }
}
