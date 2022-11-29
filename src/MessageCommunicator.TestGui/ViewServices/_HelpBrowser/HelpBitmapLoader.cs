using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Markdown.Avalonia.Utils;
using ReactiveUI;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class HelpBitmapLoader : IBitmapLoader
    {
        /// <inheritdoc />
        public string? AssetPathRoot { get; set; }
        
        private ConcurrentDictionary<Uri, WeakReference<Bitmap>> _bitmapCache;

        private IAssetLoader _assetLoader;
        private string[] AssetAssemblyNames { get; }

        public HelpBitmapLoader(Assembly assetAssembly, string? helpRoutePath)
        {
            _assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>()!;
            this.AssetPathRoot = helpRoutePath ?? string.Empty;

            var assetAssemblyName = assetAssembly.GetName().Name;
            if (string.IsNullOrEmpty(assetAssemblyName))
            {
                throw new ArgumentException("Given assembly does not have a name!", nameof(assetAssembly));
            }
            this.AssetAssemblyNames = new[]{ assetAssemblyName };

            this._bitmapCache = new ConcurrentDictionary<Uri, WeakReference<Bitmap>>();
        }

        private void Compact()
        {
            foreach (var entry in _bitmapCache.ToArray())
            {
                if (!entry.Value.TryGetTarget(out var dummy))
                {
                    ((IDictionary<Uri, WeakReference<Bitmap>>)_bitmapCache).Remove(entry.Key);
                }
            }
        }

        public Task<Bitmap?> GetAsync(string urlTxt)
        {
            return Task.Run(() => this.Get(urlTxt));
        }

        public Bitmap? Get(string urlTxt)
        {
            Bitmap? imgSource = null;

            // check network
            if (Uri.TryCreate(urlTxt, UriKind.Absolute, out var url))
            {
                imgSource = this.Get(url);
            }

            // check resources
            if (imgSource is null)
            {
                foreach (var asmNm in this.AssetAssemblyNames)
                {
                    var resourcePath = (!string.IsNullOrEmpty(this.AssetPathRoot)
                        ? Path.Combine(this.AssetPathRoot, urlTxt)
                        : urlTxt).TrimStart(new[] { '/', '\\' });
                    resourcePath = Path.Combine(asmNm, resourcePath).Replace('\\', '/');

                    var assetUrl = new Uri($"avares://{resourcePath}");
                    imgSource = this.Get(assetUrl);

                    if (imgSource != null) break;
                }
            }

            // check filesystem
            if (imgSource is null && this.AssetPathRoot != null)
            {
                try
                {
                    using (var strm = File.OpenRead(Path.Combine(this.AssetPathRoot, urlTxt)))
                        imgSource = new Bitmap(strm);
                }
                catch
                {
                    // Ignored
                }
            }

            return imgSource;
        }

        private Bitmap? Get(Uri url)
        {
            if (_bitmapCache.TryGetValue(url, out var reference))
            {
                if (reference.TryGetTarget(out var image))
                {
                    return image;
                }
            }

            Compact();

            Bitmap? imgSource = null;
            try
            {
                switch (url.Scheme)
                {
                    case "http":
                    case "https":
                        
                        using (var wc = new HttpClient())
                        using (var resourceStream = wc.GetStreamAsync(url).Result)
                            imgSource = new Bitmap(resourceStream);
                        break;

                    case "file":
                        using (var resourceStream = File.OpenRead(url.LocalPath))
                            imgSource = new Bitmap(resourceStream);
                        break;

                    case "avares":
                        using (var resourceStream = _assetLoader.Open(url))
                            imgSource = new Bitmap(resourceStream);
                        break;
                }
            }
            catch
            {
                // Ignored
            }

            if (imgSource != null)
            {
                _bitmapCache[url] = new WeakReference<Bitmap>(imgSource);
            }

            return imgSource;
        }
    }
}
