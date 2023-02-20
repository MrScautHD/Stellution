using Easel.Graphics;

namespace Easel.Content;

public class Texture2DContentTypeReader : IContentTypeReader
{
    public object LoadContentItem(string path)
    {
        return new Texture2D(path);
    }
}