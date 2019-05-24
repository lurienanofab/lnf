namespace LNF.Models.Data
{
    public struct AccountNumber : IAccountNumber
    {
        public string Value { get; }
        public string Account { get { return TryGetSubstring(0, 6); } }
        public string Fund { get { return TryGetSubstring(6, 5); } }
        public string Department { get { return TryGetSubstring(11, 6); } }
        public string Program { get { return TryGetSubstring(17, 5); } }
        public string Class { get { return TryGetSubstring(22, 5); } }
        public string Project { get { return TryGetSubstring(27, 7); } }

        private AccountNumber(string value)
        {
            Value = value;
        }

        public static IAccountNumber Parse(string value)
        {
            return new AccountNumber(value);
        }

        private string TryGetSubstring(int start, int length)
        {
            if (Value.Length >= start + length)
                return Value.Substring(start, length);
            else
                return new string('?', length);
        }
    }
}
