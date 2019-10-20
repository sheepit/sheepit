using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Infrastructure.ErrorHandling;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.DeploymentProcesses
{
    public class DeploymentProcessStorage
    {
        private readonly SheepItDatabase _database;
        private readonly IdentityProvider _identityProvider;

        public DeploymentProcessStorage(
            SheepItDatabase database,
            IdentityProvider identityProvider)
        {
            _database = database;
            _identityProvider = identityProvider;
        }

        public void ValidateZipFile(byte[] zipFileBytes)
        {
            using (var memoryStream = new MemoryStream(zipFileBytes))
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
            {
                var processFileEntry = GetProcessFileEntry(zipArchive);

                ValidateProcessFileCanBeDeserialized(processFileEntry);
            }
        }

        private static ZipArchiveEntry GetProcessFileEntry(ZipArchive zipArchive)
        {
            var processFileEntryOrNull = zipArchive.Entries
                .FirstOrDefault(entry => entry.FullName == DeploymentProcessDirectory.ProcessFileName);

            if (processFileEntryOrNull == null)
            {
                throw new CustomException(
                    "CREATE_DEPLOYMENT_STORAGE_ZIP_DOES_NOT_CONTAIN_PROCESS_YAML",
                    "The provided zip file does not contain process.yaml in its root."
                );
            }

            return processFileEntryOrNull;
        }

        private static void ValidateProcessFileCanBeDeserialized(ZipArchiveEntry processFileEntry)
        {
            using (var processFileEntryStream = processFileEntry.Open())
            using (var processFileEntryStreamReader = new StreamReader(processFileEntryStream))
            {
                try
                {
                    DeploymentProcessFile.OpenFomStream(processFileEntryStreamReader);
                }
                catch (Exception exception)
                {
                    throw new CustomException(
                        errorCode: "CREATE_DEPLOYMENT_STORAGE_CANNOT_DESERIALIZE_PROCESS_YAML",
                        humanReadableMessage: "Deserializing provided process.yaml file failed - please check its formatting.",
                        innerException: exception
                    );
                }
            }
        }

        public async Task<int> Add(string projectId, byte[] zipFileBytes)
        {
            var objectId = ObjectId.GenerateNewId();
            var id = await _identityProvider.GetNextId("DeploymentProcess");
            
            await _database.DeploymentProcesses.InsertOneAsync(new DeploymentProcess
            {
                ObjectId = objectId,
                Id = id,
                ProjectId = projectId,
                File = zipFileBytes
            });

            return id;
        }
    }
}