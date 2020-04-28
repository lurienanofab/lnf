namespace LNF.Ordering
{
    public static class Purchasers
    {
        public static bool IsPurchaser(int clientId)
        {
            return ServiceProvider.Current.Ordering.Purchaser.IsPurchaser(clientId);
        }
    }
}
