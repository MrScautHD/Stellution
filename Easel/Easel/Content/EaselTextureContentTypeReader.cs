using System.IO;
using Easel.Formats;

namespace Easel.Content;

public class EaselTextureContentTypeReader : IContentTypeReader
{
    public object LoadContentItem(string path)
    {
        //return EaselTexture.Deserialize(File.ReadAllBytes(path));
        return null;
    }
}