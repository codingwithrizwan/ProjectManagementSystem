namespace PMS.Domain.Entities
{
    public class Employee
    {
        public long Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Name { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public ApplicationUser? User { get; private set; }

        // EF ctor
        //only entity framework can create this object 
        //normal code can not create instance directly
        private Employee() { }

        public Employee(Guid userId, string name, string email)
        {
            UserId = userId;
            Name = name;
            Email = email;
        }
    }
}
