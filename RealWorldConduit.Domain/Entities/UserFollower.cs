namespace RealWorldConduit.Domain.Entities
{
    public class UserFollower : BaseEntity
    {
        public virtual User FollowedUser { get; set; }
        public Guid FollowedUserId { get; set; }
        public virtual User Follower { get; set; }
        public Guid FollowerId { get; set; }
    }
}
