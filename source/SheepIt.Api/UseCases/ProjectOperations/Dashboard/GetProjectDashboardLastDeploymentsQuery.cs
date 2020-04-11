using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;

namespace SheepIt.Api.UseCases.ProjectOperations.DeploymentDetails
{
	public class GetProjectDashboardLastDeploymentsQueryResult
	{
		public int ComponentId { get; set; }
		public int EnvironmentId { get; set; }
		public int DeploymentId { get; set; }
		public int PackageId { get; set; }
		public string PackageDescription { get; set; }
		public DateTime StartedAt { get; set; }
	}

	public class GetProjectDashboardLastDeploymentsQuery
    {
	    private readonly SheepItDbContext _dbContext;

	    private const string SqlQueryTemplate = @"
SELECT
	*
FROM (
	SELECT 
		ROW_NUMBER() OVER(
			PARTITION BY
				package.""ComponentId"",
				deployment.""EnvironmentId""
			ORDER BY
				deployment.""StartedAt"" DESC) as ""RowNumber"",
		package.""ComponentId"",
		deployment.""EnvironmentId"",
		deployment.""Id"" as ""DeploymentId"",
		package.""Id"" as ""PackageId"",
		package.""Description"" as ""PackageDescription"",
		deployment.""StartedAt""
	FROM
		public.""Package"" package
		JOIN public.""Deployment"" deployment ON deployment.""PackageId"" = package.""Id""
	WHERE
		package.""ProjectId"" = '{0}' AND
		deployment.""Status"" = 'Succeeded'
	) as lastDeployments
WHERE lastDeployments.""RowNumber"" = 1";

        public GetProjectDashboardLastDeploymentsQuery(SheepItDbContext dbContext)
        {
	        _dbContext = dbContext;
        }
        
        public async Task<List<GetProjectDashboardLastDeploymentsQueryResult>> Execute(string projectId)
        {
	        var connection = _dbContext.Database.GetDbConnection();
	        
	        if (connection.State != System.Data.ConnectionState.Open)
	        {
		        await connection.OpenAsync();
	        }

	        var sqlQuery = GetLastDeploymentsSqlQuery(projectId);

	        var results = new List<GetProjectDashboardLastDeploymentsQueryResult>();
	        
	        using (var tx = connection.BeginTransaction())
	        {
		        using (var command = connection.CreateCommand())
		        {
			        command.Transaction = tx;
			        command.CommandText = sqlQuery;

			        using (var reader = await command.ExecuteReaderAsync())
			        {
				        while (await reader.ReadAsync())
				        {
					        results.Add(new GetProjectDashboardLastDeploymentsQueryResult
					        {
						        ComponentId = Convert.ToInt32(reader["ComponentId"]),
						        EnvironmentId = Convert.ToInt32(reader["EnvironmentId"]),
						        DeploymentId = Convert.ToInt32(reader["DeploymentId"]),
						        PackageId = Convert.ToInt32(reader["PackageId"]),
						        PackageDescription = reader["PackageDescription"].ToString(),
						        StartedAt = Convert.ToDateTime(reader["StartedAt"])
					        });
				        }
			        }
		        }
	        }

	        return results;
        }
        
        private string GetLastDeploymentsSqlQuery(string projectId)
        {
	        return string.Format(SqlQueryTemplate, projectId);
        }
    }
}