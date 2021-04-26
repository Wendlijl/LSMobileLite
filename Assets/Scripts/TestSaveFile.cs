using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;

public class TestSaveFile : MonoBehaviour
{
    public void SaveTestFile()
    {
        QuickSaveWriter instWriter = QuickSaveWriter.Create("TestFile");
        instWriter.Write<int>("resources", 1);
    }
    public void LoadTestFile()
    {
        QuickSaveReader instReader = QuickSaveReader.Create("TestFile"); 
    }
}
