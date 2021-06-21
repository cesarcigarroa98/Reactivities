using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    //Inheriting from IdentityDbContext will add necessary tables to DB related to SignIn and LogIn.
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //Not necessary to add AppUser because it is done automatically by base class.
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityAttendee> ActivityAttendees {get; set;}
        public DbSet<Photo> Photos {get; set;}
        public DbSet<Comment> Comments {get; set;}
        public DbSet<UserFollowing> UserFollowings {get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ActivityAttendee>(b => 
            {
                //Primary key will be a combionation of userId and activityId
                b.HasKey(k => new {k.AppUserId, k.ActivityId});

                //Many to many configuration relationship
                b.HasOne(u => u.AppUser)
                    .WithMany(a => a.Activities)
                    .HasForeignKey(aa => aa.AppUserId);

                b.HasOne(u => u.Activity)
                    .WithMany(a => a.Attendees)
                    .HasForeignKey(aa => aa.ActivityId);
            });

            builder.Entity<Comment>()
                .HasOne(a => a.Activity)
                .WithMany(c => c.Comments)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<UserFollowing>(b => 
            {
                b.HasKey(k => new {k.ObserverId, k.TargetId});

                b.HasOne(o => o.Observer)
                    .WithMany(f => f.Followings)
                    .HasForeignKey(o => o.ObserverId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(o => o.Target)
                    .WithMany(f => f.Followers)
                    .HasForeignKey(o => o.TargetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}