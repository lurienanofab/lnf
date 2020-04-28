namespace LNF.Mail
{
    public interface IMailGroupMember
    {
        string Name { get; set; }
        string Email { get; set; }
        IMailGroup Group { get; }
    }
}
