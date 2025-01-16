using AutoMapper;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;

namespace lms_server.Mappings
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<Client, ClientDto>().ReverseMap();
        }
    }
}
