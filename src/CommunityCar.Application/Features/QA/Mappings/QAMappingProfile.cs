using AutoMapper;
using CommunityCar.Application.Features.QA.ViewModels;
using CommunityCar.Domain.Entities.Community.QA;

namespace CommunityCar.Application.Features.QA.Mappings;

public class QAMappingProfile : Profile
{
    public QAMappingProfile()
    {
        CreateMap<Question, QuestionVM>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => "Anonymous")) // TODO: Map actual user name
            .ForMember(dest => dest.AnswerCount, opt => opt.Ignore());

        CreateMap<Answer, AnswerVM>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => "Anonymous")); // TODO: Map actual user name
    }
}
