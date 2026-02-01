using AutoMapper;
using CommunityCar.Application.Features.Community.QA.ViewModels;
using CommunityCar.Domain.Entities.Community.QA;

namespace CommunityCar.Application.Features.Community.QA.Mappings;

public class QAMappingProfile : AutoMapper.Profile
{
    public QAMappingProfile()
    {
        CreateMap<Question, QuestionVM>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList()))
            .ForMember(dest => dest.AuthorName, opt => opt.Ignore()) // Will be populated by service
            .ForMember(dest => dest.AnswerCount, opt => opt.MapFrom(src => src.AnswerCount));

        CreateMap<Answer, AnswerVM>()
            .ForMember(dest => dest.AuthorName, opt => opt.Ignore()); // Will be populated by service
    }
}



