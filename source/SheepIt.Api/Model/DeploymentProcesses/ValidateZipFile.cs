using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using SheepIt.Api.Infrastructure.ErrorHandling;
using SheepIt.Api.Runner.DeploymentProcessRunning.DeploymentProcessAccess;

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
                throw new ZipArchiveCannotBeOpenedException(exception);
            }
        }

        private static ZipArchiveEntry GetProcessFileEntry(ZipArchive zipArchive)
        {
            var processFileEntryOrNull = zipArchive.Entries
                .FirstOrDefault(entry => entry.FullName == DeploymentProcessDirectory.ProcessFileName);

            if (processFileEntryOrNull == null)
            {
                throw new ZipArchiveDoesNotContainProcessYamlException();
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
                throw new ZipArchiveDeserializingFailedException(exception);
            }
        }
    }
    
    public class ZipArchiveCannotBeOpenedException : CustomException
    {
        public ZipArchiveCannotBeOpenedException(Exception innerException)
            : base(
                "CREATE_DEPLOYMENT_STORAGE_ZIP_CANNOT_BE_OPENED",
                "The provided file cannot be opened - please check if it's a valid zip file.",
                innerException)
        {
        }
    }
        
        
    public class ZipArchiveDoesNotContainProcessYamlException : CustomException
    {
        public ZipArchiveDoesNotContainProcessYamlException() 
            : base(
                "CREATE_DEPLOYMENT_STORAGE_ZIP_DOES_NOT_CONTAIN_PROCESS_YAML",
                "Project The provided zip file does not contain process.yaml in its root.")
        {
        }
    }
        
    public class ZipArchiveDeserializingFailedException : CustomException
    {
        public ZipArchiveDeserializingFailedException(Exception innerException) 
            : base(
                "CREATE_DEPLOYMENT_STORAGE_CANNOT_DESERIALIZE_PROCESS_YAML",
                "Deserializing provided process.yaml file failed - please check its formatting.",
                innerException)
        {
                
        }
    }
}