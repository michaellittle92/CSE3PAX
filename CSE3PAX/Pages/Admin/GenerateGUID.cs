namespace CSE3PAX.Pages.Admin
{
    public class GenerateGUID
    {
        public static string CreateNewGuid() {
            Guid guid = Guid.NewGuid();
            string guidString = guid.ToString();
            return guidString;
        }
    }
}
