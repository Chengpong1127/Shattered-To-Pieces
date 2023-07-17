using System.Collections.Generic;
using System.Collections;
using System.IO;


public class FileHandler
{
    /// <summary>
    /// Save a string to a file
    /// </summary>
    /// <param name="path"> The path of the file</param>
    /// <param name="content"> The content of the file</param>
    public static void SaveString(string path, string content)
    {
        if (!File.Exists(path))
        {
            File.Create(path);
        }
        File.WriteAllText(path, content);
    }
    /// <summary>
    /// Load a file as a string
    /// </summary>
    /// <param name="path"> The path of the file</param>
    /// <returns> The content of the file</returns>
    public static string LoadString(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }
        return File.ReadAllText(path);
    }

    /// <summary>
    /// Delete a file or a directory
    /// </summary>
    /// <param name="path"> The path of the file or directory</param>
    public static void Delete(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        else
        {
            throw new FileNotFoundException();
        }
    }

    /// <summary>
    /// Rename a file or a directory
    /// </summary>
    /// <param name="path"> The path of the file or directory</param>
    /// <param name="newPath"> The new path of the file or directory</param>
    public static void Rename(string path, string newPath)
    {
        if(File.Exists(path))
        {
            File.Move(path, newPath);
        }else if(Directory.Exists(path))
        {
            Directory.Move(path, newPath);
        }else
        {
            throw new FileNotFoundException();
        }
    }
    /// <summary>
    /// Get all files in a directory
    /// </summary>
    /// <param name="path"> The path of the directory</param>
    /// <returns> A list of files in the directory</returns>
    public static List<string> GetFiles(string path)
    {
        
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException();
        }
        return new List<string>(Directory.GetFiles(path));
    }

    /// <summary>
    /// Make a directory
    /// </summary>
    /// <param name="path"> The path of the new directory</param>
    public static void Mkdir(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
