using AutoMapper;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;

namespace lms.Abstractions.Mappings
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<Client, ClientDto>().ReverseMap();
            CreateMap<BookRequest, BookRequestDto>().ReverseMap();
        }
    }
}
