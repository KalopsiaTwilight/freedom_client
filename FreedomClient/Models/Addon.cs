using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FreedomClient.Models
{
    [AddINotifyPropertyChangedInterface]
    public class AddonItem
    {
        public AddonItem()
        {
            Author = string.Empty;
            Version = string.Empty;
            ImageSrc = string.Empty;
            Description = string.Empty;
            Title = string.Empty;
            IsInstalled = false;
        }
        [AlsoNotifyFor(nameof(DisplayAuthor))]
        public string Author { get; set; }
        [JsonIgnore]
        public string DisplayAuthor => Author.Length > 22 ? string.Concat(Author.AsSpan(0, 19), "...") : Author;
        public string Version { get; set; }
        public string ImageSrc { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public bool IsInstalled { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    public class Addon: AddonItem
    {
        public Addon()
        {
            Manifest = string.Empty;
            Signature = string.Empty;
        }
        public string Manifest { get; set; }
        public string Signature{ get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    public class AddonCollection: AddonItem
    {
        public AddonCollection()
        {
            Addons = Array.Empty<string>();
        }

        public string[] Addons { get; set; }
    }
}
