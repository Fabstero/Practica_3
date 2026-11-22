using AutoMapper;
using Practica_2.Application.Dtos;
using Practica_2.Domain.Entidades;

namespace Practica_2.Application.Mapping;

public class MapeoClases : Profile
{
    public MapeoClases()
    {
        CreateMap<Votante, VotanteDto>().ReverseMap();

        CreateMap<PartidoPolitico, PartidoPoliticoDto>()
            .ForMember(dest => dest.CantidadVotos, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.Votos, opt => opt.Ignore());

        CreateMap<Voto, VotoDto>()
            .ForMember(dest => dest.CedulaVotante, opt => opt.MapFrom(src => src.Votante != null ? src.Votante.Cedula : string.Empty))
            .ForMember(dest => dest.NombreVotante, opt => opt.MapFrom(src => src.Votante != null ? src.Votante.NombreCompleto : string.Empty))
            .ForMember(dest => dest.ProvinciaVotante, opt => opt.MapFrom(src => src.Votante != null ? src.Votante.Provincia : string.Empty))
            .ForMember(dest => dest.NombrePartido, opt => opt.MapFrom(src => src.PartidoPolitico != null ? src.PartidoPolitico.Nombre : string.Empty))
            .ForMember(dest => dest.SiglasPartido, opt => opt.MapFrom(src => src.PartidoPolitico != null ? src.PartidoPolitico.Siglas : string.Empty))
            .ForMember(dest => dest.NombreCandidato, opt => opt.MapFrom(src => src.PartidoPolitico != null ? src.PartidoPolitico.NombreCandidato : string.Empty))
            .ForMember(dest => dest.BanderaUrl, opt => opt.MapFrom(src => src.PartidoPolitico != null ? src.PartidoPolitico.BanderaUrl : null));
    }
}
