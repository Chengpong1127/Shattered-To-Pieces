using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;

public class FileTest
{
    [Test]
    public void FileHandlerTest()
    {
        string path = Path.Combine(Application.persistentDataPath, "FileHandlerTest.txt");
        string content = "Hello World!";
        FileHandler.SaveString(path, content);
        string loadedContent = FileHandler.LoadString(path);
        Assert.AreEqual(content, loadedContent);

        FileHandler.Delete(path);
    }
    [Test]
    public void OverwriteTest(){
        string path = Path.Combine(Application.persistentDataPath, "OverwriteTest.txt");
        string content = "Hello World!";
        FileHandler.SaveString(path, content);
        string loadedContent = FileHandler.LoadString(path);
        Assert.AreEqual(content, loadedContent);
        string newContent = "Hello World! 2";
        FileHandler.SaveString(path, newContent);
        string loadedNewContent = FileHandler.LoadString(path);
        Assert.AreEqual(newContent, loadedNewContent);

        FileHandler.Delete(path);
    }

    [Test]
    public void FileNotFoundExceptionTest(){
        string path = Path.Combine(Application.persistentDataPath, "FileNotFoundExceptionTest.txt");
        Assert.Throws<FileNotFoundException>(() => FileHandler.LoadString(path));
    }

    [Test]
    public void DeleteFileTest(){
        string path = Path.Combine(Application.persistentDataPath, "DeleteFileTest.txt");
        string content = "Hello World!";
        FileHandler.SaveString(path, content);
        string loadedContent = FileHandler.LoadString(path);
        Assert.AreEqual(content, loadedContent);
        FileHandler.Delete(path);
        Assert.Throws<FileNotFoundException>(() => FileHandler.LoadString(path));
    }

    [Test]
    public void DeleteDirectoryTest(){
        string path = Path.Combine(Application.persistentDataPath, "DeleteDirectoryTest");
        Directory.CreateDirectory(path);
        Assert.IsTrue(Directory.Exists(path));
        FileHandler.Delete(path);
        Assert.Throws<FileNotFoundException>(() => FileHandler.LoadString(path));
    }

    [Test]
    public void RenameFileTest(){
        string path = Path.Combine(Application.persistentDataPath, "RenameFileTest.txt");
        string newPath = Path.Combine(Application.persistentDataPath, "RenameFileTest2.txt");
        string content = "Hello World!";
        FileHandler.SaveString(path, content);
        string loadedContent = FileHandler.LoadString(path);
        Assert.AreEqual(content, loadedContent);
        FileHandler.Rename(path, newPath);
        Assert.Throws<FileNotFoundException>(() => FileHandler.LoadString(path));
        string loadedNewContent = FileHandler.LoadString(newPath);
        Assert.AreEqual(content, loadedNewContent);
        FileHandler.Delete(newPath);
    }

    [Test]
    public void RenameDirectoryTest(){
        string path = Path.Combine(Application.persistentDataPath, "RenameDirectoryTest");
        string newPath = Path.Combine(Application.persistentDataPath, "RenameDirectoryTest2");
        Directory.CreateDirectory(path);
        Assert.IsTrue(Directory.Exists(path));
        FileHandler.Rename(path, newPath);
        Assert.Throws<FileNotFoundException>(() => FileHandler.LoadString(path));
        Assert.IsTrue(Directory.Exists(newPath));
        FileHandler.Delete(newPath);
    }


}
