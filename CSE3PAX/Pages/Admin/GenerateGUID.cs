namespace CSE3PAX.Pages.Admin
{
    /*
     The GenerateGUID class provides functionality to generate new GUID (Globally Unique Identifier) strings.
    */
    public class GenerateGUID
    {

        // Generates a new GUID and returns it as a string
        public static string CreateNewGuid() {
            Guid guid = Guid.NewGuid();
            string guidString = guid.ToString();
            return guidString;
        }
    }
}
