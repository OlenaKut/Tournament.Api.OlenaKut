using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Data.Data;

namespace Tournament.Data.Data
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<TournamentDetails, TournamentDto>().ReverseMap();
            CreateMap<TournamentCreateDto, TournamentDetails>();
            CreateMap<TournamentUpdateDto, TournamentDetails>().ReverseMap();
            CreateMap<Game, GameDto>().ReverseMap();
            CreateMap<GameCreateDto, Game>().ReverseMap();
            CreateMap<GameUpdateDto, Game>().ReverseMap();

        }
    }
}
