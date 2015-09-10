using SDC.web.Models.Books;
using SDC.web.Models.Common;
using SDC.web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.App_Start
{
    public class MappingsConfig
    {
        public static void RegisterMappings()
        {
            int i = 1;
            AutoMapper.Mapper.CreateMap<Author, AuthorViewModel>()
                .ForMember(vm => vm.AddedBy,
                opts => opts.MapFrom(src => (src.AddedBy == null ? "-" : src.AddedBy.UserName)))
                .ForMember(vm => vm.LastModifiedBy,
                opts => opts.MapFrom(src => (src.LastModifiedBy == null ? "-" : src.AddedBy.UserName)));

            AutoMapper.Mapper.CreateMap<AuthorViewModel, Author>();

            AutoMapper.Mapper.CreateMap<Book, BookViewModel>()
                .ForMember(vm => vm.Language,
                opts => opts.MapFrom(src => new Language { Code = src.Language, Name = src.Language }));
        }
    }
}