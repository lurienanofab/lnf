namespace LNF.Models.Authorization.Credentials
{
    public interface ICredentials
    {
        void ApplyParameters(IRequest req);
    }
}
