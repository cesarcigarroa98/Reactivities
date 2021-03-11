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
        }
    }
}