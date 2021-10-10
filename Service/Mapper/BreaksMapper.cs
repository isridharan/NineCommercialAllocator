using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Model;
using Service.ViewModel;

namespace Service.Mapper
{
    public class BreaksProfile : Profile
    {
        public BreaksProfile()
        {
            CreateMap<Break, BreakViewModel>()
              .ForMember(x => x.Name, o => o.MapFrom(x => x.Name));              
            CreateMap<KeyValuePair<int,Commercial>, CommercialViewModel>()
             .ForMember(x => x.Name, o => o.MapFrom(x => x.Value.Name))
             .ForMember(x => x.Type, o => o.MapFrom(x => x.Value.Type))
             .ForMember(x => x.Demographic, o => o.MapFrom(x => x.Value.Demographic))
             .ForMember(x => x.Rating, o => o.MapFrom(x => x.Value.Rating));
        }
    }
}
