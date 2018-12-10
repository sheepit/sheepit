using LiteDB;

namespace SheepIt.Domain
{
    public static class Database
    {
        public static LiteDatabase Open()
        {
            return new LiteDatabase(@"C:\sheep-it\poc-database.db");
        }
    }
}