using System.Linq;

namespace LNF.Data
{
    public static class Clients
    {
        public static string GetDisplayName(string lname, string fname)
        {
            return string.Join(", ", new[] { lname, fname }.Where(x => !string.IsNullOrEmpty(x))).Trim();
        }
    }
}
