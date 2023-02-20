using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Easel.Audio;
using Easel.Content.Localization;
using Easel.Core;
using Easel.Data;
using Easel.Formats;
using Easel.Graphics;
using Easel.GUI;

namespace Easel.Content;

public class ContentManager
{
    public string ContentRootDir;

    public Locale Locale;

    private string _localeDir;
    private Dictionary<string, string> _loadedLocales;

    //private Dictionary<string, ContentCacheItem> _cachedObjects;
    //private List<string> _objectsToRemove;

    public TimeSpan DeleteObjectsAfter;
    public Timer DeleteObjectsTimer;

    private Dictionary<Type, IContentTypeReader> _contentTypes;

    public ContentManager(string contentRootDir = "Content")
    {
        _contentTypes = new Dictionary<Type, IContentTypeReader>();
        AddNewTypeReader(typeof(Bitmap), new BitmapContentTypeReader());
        AddNewTypeReader(typeof(Texture2D), new Texture2DContentTypeReader());
        AddNewTypeReader(typeof(Model), new ModelContentTypeReader());
        //AddNewTypeReader(typeof(EaselTexture), new EaselTextureContentTypeReader());
        AddNewTypeReader(typeof(Font), new FontContentTypeReader());
        AddNewTypeReader(typeof(Sound), new SoundContentTypeReader());

        ContentRootDir = contentRootDir;

        _loadedLocales = new Dictionary<string, string>();

        /*_cachedObjects = new Dictionary<string, ContentCacheItem>();
        _objectsToRemove = new List<string>();
        
        DeleteObjectsAfter = TimeSpan.FromMinutes(5);
        DeleteObjectsTimer = new Timer(60000);
        DeleteObjectsTimer.Elapsed += (sender, args) => RemovedUnusedObjectsFromCache();
        DeleteObjectsTimer.Start();*/
    }

    /*public void RemovedUnusedObjectsFromCache()
    {
        foreach ((string key, ContentCacheItem item) in _cachedObjects)
        {
            if (item.NeedsRemove(DeleteObjectsAfter))
            {
                Logger.Debug("Removing \"" + item + "\" from cache.");
                _objectsToRemove.Add(key);
            }
        }

        foreach (string item in _objectsToRemove)
            _cachedObjects.Remove(item);
        
        _objectsToRemove.Clear();
    }*/

    /// <summary>
    /// Load the given file into the given type. Note: While you do not have to provide an extension, it's recommended
    /// that you do, as there will be a slight speed penalty for not providing the extension.
    /// </summary>
    /// <param name="path">The relative path to the file.</param>
    /// <typeparam name="T">The type of the object you want to load.</typeparam>
    /// <returns>The loaded file.</returns>
    public T Load<T>(string path)
    {
        if (!_contentTypes.ContainsKey(typeof(T)))
            throw new NotSupportedException("A content type reader for type \"" + typeof(T).FullName +
                                            "\" has not been implemented.");
        
        //if (_cachedObjects.TryGetValue(path, out ContentCacheItem item))
        //    return item.Get<T>();
        
        Logger.Debug($"Loading content item \"{path}\"...");
        string fullPath = Path.Combine(ContentRootDir, path);
        if (!Path.HasExtension(fullPath))
        {
            IEnumerable<string> dirs = Directory.GetFiles(Path.GetDirectoryName(fullPath)).Where(s => Path.GetFileNameWithoutExtension(s) == Path.GetFileName(path));
            if (dirs.Count() > 1)
                Logger.Warn("Multiple files were found with the given name. Provide a file extension to load a specific file. The first file found will be loaded.");
            
            fullPath = dirs.First();
        }

        //item = new ContentCacheItem(_contentTypes[typeof(T)].LoadContentItem(fullPath));
        //_cachedObjects.Add(path, item);
        //return item.Get<T>();
        return (T) _contentTypes[typeof(T)].LoadContentItem(fullPath);
    }

    public void SetLocale(string id)
    {
        Locale = XmlSerializer.Deserialize<Locale>(File.ReadAllText(_loadedLocales[id]));
        Locale.Strings = new Dictionary<string, string>(Locale.XmlStrings.Length);
        foreach (Locale.XmlLocale locale in Locale.XmlStrings)
        {
            string value = locale.Value.Replace("\\n", "\n");
            Locale.Strings.Add(locale.Key, value);
        }
    }

    public void LoadLocales(string directory, string pattern)
    {
        Logger.Debug("Loading locales from directory...");
        string dir = Path.Combine(ContentRootDir, directory);
        if (!Directory.Exists(dir))
            return;

        Dictionary<string, string> locales = new Dictionary<string, string>();
        
        foreach (string file in Directory.GetFiles(dir, pattern))
            locales.Add(Path.GetFileNameWithoutExtension(file), file);

        _loadedLocales = locales;
    }

    public string[] GetAllFiles(string directory, string pattern = "")
    {
        return Directory.GetFiles(Path.Combine(ContentRootDir, directory), pattern);
    }

    public string GetFullPath(string localPath)
    {
        return Path.Combine(ContentRootDir, localPath);
    }

    /// <summary>
    /// Add a custom type reader to load content.
    /// </summary>
    /// <param name="type">The type to attach this reader to.</param>
    /// <param name="reader">The reader itself.</param>
    public void AddNewTypeReader(Type type, IContentTypeReader reader)
    {
        _contentTypes.Add(type, reader);
    }

    private class ContentCacheItem
    {
        private object _item;
        private DateTime _lastAccessTime;

        public ContentCacheItem(object item)
        {
            _item = item;
            _lastAccessTime = DateTime.Now;
        }

        public T Get<T>()
        {
            _lastAccessTime = DateTime.Now;
            return (T) _item;
        }

        public bool NeedsRemove(TimeSpan maxTimeSpan)
        {
            return DateTime.Now - _lastAccessTime >= maxTimeSpan;
        }
    }
}