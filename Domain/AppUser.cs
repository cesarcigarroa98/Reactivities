using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

//Using Identity user from microsoft to create an user with specific parameters.
namespace Domain
{
    public class AppUser : IdentityUser
    {
        //These are extra properties that will be added to DB when running migration.
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public ICollection<ActivityAttendee> Activities  {get; set;}
    }
}