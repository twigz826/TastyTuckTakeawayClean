namespace TastyTuckTakeaway.Core.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool ValidPaymentDetails { get; set; }
        public int PaymentDetails { get; set; }
        public ulong CardNumber { get; set; }
        public int BlockAttemps { get; set; }
        public int Age { get; set; }

        public AppUser(int id, string name, string email, string password, bool isActive)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            IsActive = isActive;
            ValidPaymentDetails = true;
            PaymentDetails = 100;
            CardNumber = 12345678901234567878;
            BlockAttemps = 0;
            Age = 30;
        }

        public bool CheckIfActive()
        {
            return IsActive;
        }

        public bool HasValidPaymentDetails()
        {
            return ValidPaymentDetails;
        }
    }
}