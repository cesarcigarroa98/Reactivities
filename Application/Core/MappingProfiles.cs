using System.Linq;
using Application.Activities;
using AutoMapper;
using Domain;

namespace Application.Core
{
    //This class is necessary to use Automapper.
    //A folder was created to separate concerns. This class will be easily accesible from the classes that required Automapper.
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            //First parameter is from which class we need to map.
            //Second parameter is target class.
            CreateMap<Activity, Activity>();
            CreateMap<Activity, ActivityDto>()
                //Fill ActivityDto.HostUsername with username from Appuser object contained inside Attendees list from activity
                .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Attendees
                    .FirstOrDefault(x => x.IsHost).AppUser.UserName));
            //Map between Attendees list property in Activity to Profile list in ActivityDto
            CreateMap<ActivityAttendee, Profiles.Profile>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
                .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio));
        }
    }
}