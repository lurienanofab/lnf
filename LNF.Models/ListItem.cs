namespace LNF.Models
{
    public struct ListItem
    {
        public string Value { get; }
        public string Text { get; }

        public ListItem(string value, string text)
        {
            Value = value;
            Text = text;
        }
    }
}
