using AutoMapper;
using SDC.data.Entity.Books;
using SDC.data.Entity.Location;
using SDC.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.AutoMapperConfig
{
    public class StripAuthorBooksResolver : ValueResolver<Book, List<Author>>
    {
        protected override List<Author> ResolveCore(Book source)
        {
            return source.Authors.AsEnumerable()
                .Select(a =>
                {
                    var author_dest = Mapper.Map<Author>(a);
                    author_dest.Books.Clear();
                    return author_dest;
                })
                .ToList();
        }
    }

    public class StripGenreBooksResolver:ValueResolver<Book, List<Genre>>
    {
        protected override List<Genre> ResolveCore(Book source)
        {
            return source.Genres.AsEnumerable()
                .Select(g =>
                {
                    var genre_dest = Mapper.Map<Genre>(g);
                    genre_dest.Books.Clear();
                    return genre_dest;
                })
                .ToList();
        }
    }

    public class MappingsConfig
    {
        public static void RegisterMappings()
        {
            AutoMapper.Mapper.CreateMap<Author, AuthorViewModel>()
                .ForMember(vm => vm.AddedBy,
                opts => opts.MapFrom(src => (src.AddedBy == null ? "-" : src.AddedBy.UserName)))
                .ForMember(vm => vm.LastModifiedBy,
                opts => opts.MapFrom(src => (src.LastModifiedBy == null ? "-" : src.AddedBy.UserName)));

            AutoMapper.Mapper.CreateMap<AuthorViewModel, Author>();

            AutoMapper.Mapper.CreateMap<Book, Book>();

            //Author <-> Author, to remove books.
            AutoMapper.Mapper.CreateMap<Author, Author>()
                .ForMember(dest => dest.Books, opts => opts.Ignore());

            //Genre <-> Genre, to remove books.
            AutoMapper.Mapper.CreateMap<Genre, Genre>()
                .ForMember(dest => dest.Books, opts => opts.Ignore());

            AutoMapper.Mapper.CreateMap<Book, BookViewModel>()
                .ForMember(vm=>vm.OwnerId,
                opts => opts.MapFrom(src => src.Shelf.Owner.UserId))
                .ForMember(vm => vm.OwnerName, 
                opts => opts.MapFrom(src => src.Shelf.Owner.UserName));


            AutoMapper.Mapper.CreateMap<BookViewModel, Book>()
                .ForMember(vm => vm.Authors, opt => opt.Ignore())
                .ForMember(vm => vm.Genres, opt => opt.Ignore())
                .ForMember(vm => vm.Publisher, opt => opt.Ignore());
        }
    }
}