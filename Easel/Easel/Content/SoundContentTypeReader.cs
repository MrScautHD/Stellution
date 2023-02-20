using Easel.Audio;

namespace Easel.Content;

public class SoundContentTypeReader : IContentTypeReader
{
    public object LoadContentItem(string path)
    {
        return new Sound(path);
    }
}