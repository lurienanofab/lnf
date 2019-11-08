namespace LNF.Models.Mail
{
    public class InvalidEmailItem : IInvalidEmail
    {
        public int EmailID { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
    }
}
