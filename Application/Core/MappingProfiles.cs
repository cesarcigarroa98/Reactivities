using System.Linq;
using Application.Activities;
using Application.Comments;
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
            string currentUsername = null;

            //First parameter is from which class we need to map.
            //Second parameter is target class.
            CreateMap<Activity, Activity>();
            CreateMap<Activity, ActivityDto>()
                //Fill ActivityDto.HostUsername with username from Appuser object contained inside Attendees list from activity
                .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Attendees
                    .FirstOrDefault(x => x.IsHost).AppUser.UserName));
            //Map between Attendees list property in Activity to Profile list in ActivityDto
            CreateMap<ActivityAttendee, AttendeeDto>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
                .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio))
                .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.AppUser.Followers.Count))
                .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.AppUser.Followings.Count))
                .ForMember(d => d.Following, 
                    o => o.MapFrom(s => s.AppUser.Followers.Any(x => x.Observer.UserName == currentUsername)));
            CreateMap<AppUser, Profiles.Profile>()
                .ForMember(d => d.Image, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.Followers.Count))
                .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.Followings.Count))
                .ForMember(d => d.Following, 
                    o => o.MapFrom(s => s.Followers.Any(x => x.Observer.UserName == currentUsername)));
            CreateMap<Comment, CommentDto>()
                .ForMember(d => d.Image, o => o.MapFrom(s => s.Author.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.Author.UserName))
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.Author.DisplayName));
        }
    }
}