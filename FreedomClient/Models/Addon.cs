using PropertyChanged;
using System;
using System.Text.Json.Serialization;

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

        public static Addon TotalRP3 = new()
        {
            Manifest =  "https://cdn.wowfreedom-rp.com/client_content/addons/totalRP3.manifest",
            Title = "Total RP 3",
            Signature =  "https://cdn.wowfreedom-rp.com/client_content/addons/totalRP3.signature",
            ImageSrc = "https://media.forgecdn.net/avatars/219/404/637015802143725785.png",
            Version = "2.3.13",
            Author = "Telkostrasz \u0026 Ellypse",
            Description = "The best roleplaying addon for World of Warcraft."
        };

        public static Addon Elephant = new()
        {
            Manifest = "https://cdn.wowfreedom-rp.com/client_content/addons/Elephant.manifest",
            Title = "Elephant",
            Signature = "https://cdn.wowfreedom-rp.com/client_content/addons/Elephant.signature",
            ImageSrc = "https://media.forgecdn.net/avatars/64/706/636155467072456515.jpeg",
            Version = "9.2.7",
            Author = "AllInOneMighty",
            Description = "A friendly companion that remembers the chat."
        };
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
