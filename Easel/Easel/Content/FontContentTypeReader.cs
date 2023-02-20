using Easel.GUI;

namespace Easel.Content;

public class FontContentTypeReader : IContentTypeReader
{
    public object LoadContentItem(string path)
    {
        return new Font(path);
    }
}