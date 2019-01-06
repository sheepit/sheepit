namespace SheepIt.Domain
{
    public static class Database
    {
        public static TempMongoDatabase Open()
        {
            return new TempMongoDatabase();
        }
    }
}