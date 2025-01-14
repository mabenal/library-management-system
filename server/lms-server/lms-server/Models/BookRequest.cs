namespace lms_server.Models
{
    public class BookRequest
    {
        public  Guid Id { get; set; }

        public required string Status { get; set; }

        public required DateTime DateRequested { get; set; }

        public DateTime DateApproved { get; set; }

        public DateTime DateReturned { get; set; }

        public Guid ClientId { get; set; }

        public Guid BookId { get; set; }

        //navigation properties

        public required Client Client { get; set; }

        public required Book Book { get; set; }





    }
}
