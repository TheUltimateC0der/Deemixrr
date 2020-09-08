using AutoMapper;

using Deemixrr.Data;
using Deemixrr.Jobs.Models;
using Deemixrr.Models;

using E.Deezer.Api;

namespace Deemixrr.AutoMapper
{
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<ArtistCreateInputModel, CreateArtistBackgroundJobData>();
            CreateMap<PlaylistCreateInputModel, CreatePlaylistBackgroundJobData>();

            CreateMap<IArtist, Artist>()
                .ForMember(x => x.DeezerId, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(x => x.Folder, opt => opt.Ignore())
                .ForMember(x => x.FolderId, opt => opt.Ignore())
                .ForMember(x => x.NumberOfTracks, opt => opt.Ignore())
                .ForMember(x => x.NumberOfAlbums, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<IPlaylist, Playlist>()
                .ForMember(x => x.DeezerId, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Title))
                .ForMember(x => x.Folder, opt => opt.Ignore())
                .ForMember(x => x.FolderId, opt => opt.Ignore())
                .ForMember(x => x.NumberOfTracks, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<IGenre, Genre>()
                .ForMember(x => x.DeezerId, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}