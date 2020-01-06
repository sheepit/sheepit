using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Infrastructure.ErrorHandling;

namespace SheepIt.Api.Model.DeploymentProcesses
{
    public class ValidateZipFile
    {
        public void Validate(byte[] zipFileBytes)
        {
            using var memoryStream = new MemoryStream(zipFileBytes);
            using var zipArchive = OpenZipArchive(memoryStream);
            
            var processFileEntry = GetProcessFileEntry(zipArchive);

            ValidateProcessFileCanBeDeserialized(processFileEntry);
        }

        private static ZipArchive OpenZipArchive(MemoryStream memoryStream)
        {
            try
            {
                return new ZipArchive(memoryStream, ZipArchiveMode.Read);
            }
            catch (Exception exception)
            {
                throw new CustomException(
                    errorCode: "CREATE_DEPLOYMENT_STORAGE_ZIP_CANNOT_BE_OPENED",
                    humanReadableMessage: "The provided file cannot be opened - please check if it's a valid zip file.",
                    innerException: exception
                );
            }
        }

        private static ZipArchiveEntry GetProcessFileEntry(ZipArchive zipArchive)
        {
            var processFileEntryOrNull = zipArchive.Entries
                .FirstOrDefault(entry => entry.FullName == DeploymentProcessDirectory.ProcessFileName);

            if (processFileEntryOrNull == null)
            {
                throw new CustomException(
                    errorCode: "CREATE_DEPLOYMENT_STORAGE_ZIP_DOES_NOT_CONTAIN_PROCESS_YAML",
                    humanReadableMessage: "The provided zip file does not contain process.yaml in its root."
                );
            }

            return processFileEntryOrNull;
        }

        private static void ValidateProcessFileCanBeDeserialized(ZipArchiveEntry processFileEntry)
        {
            using var processFileEntryStream = processFileEntry.Open();
            using var processFileEntryStreamReader = new StreamReader(processFileEntryStream);
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
}