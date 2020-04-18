namespace SqlStreamStore.Demo.Persistence.Entities
{
    public class User : Entity
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
    }
}
