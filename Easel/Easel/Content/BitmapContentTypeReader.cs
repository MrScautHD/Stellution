using Easel.Graphics;

namespace Easel.Content;

public class BitmapContentTypeReader : IContentTypeReader
{
    public object LoadContentItem(string path)
    {
        return new Bitmap(path);
    }
}