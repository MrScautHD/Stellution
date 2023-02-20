using Easel.Graphics;

namespace Easel.Content;

public class ModelContentTypeReader : IContentTypeReader
{
    public object LoadContentItem(string path)
    {
        return new Model(path);
    }
}