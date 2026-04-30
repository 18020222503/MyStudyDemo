using System.IO;
using UnityEngine;

public class BloStream : MonoBehaviour
{
    public void Reset()
    {
        DataSize = 0;
    }

    public void SaveFile(string fileName)
    {
        FileStream bw = System.IO.File.Create(fileName);
        if (bw != null)
        {
            bw.Write(_data, 0, _data.Length);
            bw.Close();
        }
    }

    public byte[] GetData()
    {
        return _data;
    }

    public void SetData(byte[] data)
    {
        _data = data;
        DataSize = data.Length;
    }

    public long DataSize;

    [HideInInspector]
    [SerializeField]
    byte[] _data;
}
