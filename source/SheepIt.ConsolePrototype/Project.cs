namespace SheepIt.ConsolePrototype
{
    public class Project
    {
        public string Id { get; set; }
        public string RepositoryUrl { get; set; }
    }

    public static class Projects
    {
        public static Project Get(string projectId)
        {
            using (var database = Database.Open())
            {
                var projectCollection = database.GetCollection<Project>();

                return projectCollection.FindById(projectId);
            }
        }
    }
}