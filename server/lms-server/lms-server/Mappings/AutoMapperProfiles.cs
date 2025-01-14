using AutoMapper;
using lms_server.Models;
using lms_server.Models.DTO;

namespace lms_server.Mappings
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Book, BookDto>().ReverseMap();
        }
    }
}
