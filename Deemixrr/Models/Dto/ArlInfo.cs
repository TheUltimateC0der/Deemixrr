using AutoMapper;

using System;

namespace Deemixrr.Models.Dto
{
    public class ArlInfo
    {
        public string Content { get; set; }

        public DateTime LastWrite { get; set; }
    }

    public class ArlInfoAutoMapperConfiguration : Profile
    {
        public ArlInfoAutoMapperConfiguration()
        {
            CreateMap<ArlInfo, SettingsArlInputViewModel>()
                .ForMember(x => x.Arl, opt => opt.MapFrom(x => x.Content))
                .ForMember(x => x.LastWrite, opt => opt.MapFrom(x => x.LastWrite))
                .ReverseMap();
        }
    }
}