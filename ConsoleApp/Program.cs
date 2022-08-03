namespace ConsoleApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var reader = new DataReader();
            reader.ImportAndPrintData("data.csv");
        }
    }
}
