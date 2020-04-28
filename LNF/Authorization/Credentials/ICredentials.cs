namespace LNF.Authorization.Credentials
{
    public interface ICredentials
    {
        void ApplyParameters(IRequest req);
    }
}
