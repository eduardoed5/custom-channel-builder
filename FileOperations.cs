using CreatorChannelsXrmToolbox.Model;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;

namespace CreatorChannelsXrmToolbox
{
    /// <summary>
    /// This class allows the handling of file operations
    /// </summary>
    public class FileOperations
    {
        /// <summary>
        /// Allows you to write a file to the specified path and file name.
        /// </summary>
        /// <param name="path">Location of the file on the disk where it will be written.</param>
        /// <param name="binary">Binary file data</param>
        /// <param name="fileName">File name to write</param>
        public static void WriteFile(string path, byte[] binary, string fileName)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllBytes(Path.Combine(path, fileName), binary);
        }

        /// <summary>
        /// Allows you to write a file to the specified path
        /// </summary>
        /// <param name="path">Location of the file on the disk where it will be written.</param>
        /// <param name="binary">Binary file data</param>
        public static void WriteFile(string path, byte[] binary)
        {
            File.WriteAllBytes(path, binary);
        }

        /// <summary>
        /// Allows you to read a file from the specified path.
        /// </summary>
        /// <param name="path">Location of the file on the disk where it will be read.</param>
        /// <param name="fileName">File Name to read</param>
        /// <returns>Binary file data</returns>
        public static byte[] ReadFile(string path, string fileName)
        {
            byte[] _binary = File.ReadAllBytes(Path.Combine(path, fileName));
            return _binary;
        }

        /// <summary>
        /// Extract the solution from the original zip
        /// </summary>
        /// <param name="path">Location of the file on the disk</param>
        /// <param name="solutionName">Solution name</param>
        public static void ExtractSolution(string path, string solutionName)
        {
            string _sourceFile = Path.Combine(path, solutionName);
            string _extractionPath = path + "\\" + Path.GetFileNameWithoutExtension(solutionName);
            if (Directory.Exists(_extractionPath))
                Directory.Delete(_extractionPath, true);
            ZipFile.ExtractToDirectory(_sourceFile, _extractionPath);
        }

        /// <summary>
        /// Generate the solution in a zip file
        /// </summary>
        /// <param name="path">Location of the new file on disk</param>
        /// <param name="solutionName">Solution Name</param>
        /// <param name="solutionNameNew">Name of the new solution file</param>
        public static void ZipSolution(string path, string solutionName, string solutionNameNew)
        {
            string _folderNameSolution = Path.GetFileNameWithoutExtension(solutionName);
            string _sourceDirectory = Path.Combine(path, _folderNameSolution);
            string _outputFile = path + "\\" + solutionNameNew;
            if (File.Exists(_outputFile))
                File.Delete(_outputFile);
            ZipFile.CreateFromDirectory(_sourceDirectory, _outputFile);
        }

        /// <summary>
        /// Save an object to a JSON file on disk
        /// </summary>
        /// <param name="channel">Channel information</param>
        /// <param name="path">Location of the file on the disk</param>
        public static void SaveObjectAsJson(ChannelData channel, string path)
        {
            JsonSerializerSettings _options = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string _json = JsonConvert.SerializeObject(channel, _options);
            File.WriteAllText(path, _json);
        }

        /// <summary>
        /// Read a JSON file from disk
        /// </summary>
        /// <param name="path">Location of the file on the disk</param>
        /// <returns>Channel information</returns>
        /// <exception cref="FileNotFoundException">Exception if the file does not exist</exception>
        public static ChannelData ReadObjectFromJson(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("JSON file not found", path);

            string _json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<ChannelData>(_json);
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="path">Location of the file on the disk</param>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        /// <summary>
        /// Delete a directory
        /// </summary>
        /// <param name="path">Location of the directory on the disk</param>
        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        /// <summary>
        /// Copy a file from one directory to another
        /// </summary>
        /// <param name="sourcePath">Source path</param>
        /// <param name="sourceFileName">Source file name</param>
        /// <param name="destinationPath">Destination path</param>
        public static void CopyFile(string sourcePath, string sourceFileName, string destinationPath)
        {
            string _sourceFile = sourcePath + "\\" + sourceFileName;
            string _destinationFile = destinationPath + "\\" + sourceFileName.Replace("_edit", "");
            File.Copy(_sourceFile, destinationPath, overwrite: true);
        }

    }
}
