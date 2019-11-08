namespace LNF.Models.Mail
{
    public interface IInvalidEmail
    {
        int EmailID { get; set; }
        string DisplayName { get; set; }
        string EmailAddress { get; set; }
        bool IsActive { get; set; }
    }
}
