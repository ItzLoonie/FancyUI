namespace IconPacks;

public class IconPack(string name)
{
    public Dictionary<string, Dictionary<string, Sprite>> BaseIcons { get; set; } = [];
    public Dictionary<string, Dictionary<string, List<Sprite>>> EasterEggs { get; set; } = [];

    public Dictionary<string, TMP_SpriteAsset> MentionStyles { get; set; } = [];
    public Dictionary<string, TMP_SpriteAsset> BTOS2MentionStyles { get; set; } = [];
    public Dictionary<string, TMP_SpriteAsset> LegacyMentionStyles { get; set; } = [];
    public TMP_SpriteAsset PlayerNumbers { get; set; }

    public string Name { get; } = name;

    private string PackPath => Path.Combine(AssetManager.ModPath, Name);

    private static readonly string[] Folders = [ "Regular", "Town", "Coven", "SerialKiller", "Arsonist", "Werewolf", "Shroud", "Apocalypse", "Executioner", "Jester", "Pirate", "Doomsayer",
        "Judge", "Auditor", "Starspawn", "Inquisitor", "Vampire", "CursedSoul", "Jackal", "Lions", "Frogs", "Hawks", "VIP", "Pandora", "Compliant", "Egoist", "PlayerNumbers",  "Custom",
        /*"Mafia", "Amnesiac", "Juggernaut", "GuardianAngel", "Evils"*/ ];
    //private static readonly string[] Mods = [ "Vanilla", "BTOS2"/*, "Legacy"*/ ];
    public static readonly string[] FileTypes = [ "png", "jpg" ];

    public void Debug()
    {
        Logging.LogMessage($"Debugging {Name}");

        foreach (var (name2, icons) in BaseIcons)
        {
            foreach (var (name3, icon) in icons)
            {
                if (icon)
                    Logging.LogMessage($"{Name} {name3} has a(n) {name2} sprite!");
            }
        }

        Logging.LogMessage($"{Name} {BaseIcons.Count} Base Assets loaded!");

        foreach (var (name2, icons) in EasterEggs)
        {
            foreach (var (name3, icon) in icons)
            {
                if (icon.Count > 0)
                    Logging.LogMessage($"{Name} {name3} has {icon.Count} {name2} easter egg sprite(s)!");
            }
        }

        Logging.LogMessage($"{Name} {EasterEggs.Count} Easter Egg Assets loaded!");

        foreach (var (name2, sheet) in MentionStyles)
        {
            if (sheet)
                Logging.LogMessage($"{Name} {name2} mention style exists!");
        }

        Logging.LogMessage($"{Name} {MentionStyles.Count} Vanilla mention styles exist!");

        foreach (var (name2, sheet) in BTOS2MentionStyles)
        {
            if (sheet)
                Logging.LogMessage($"{Name} {name2} BTOS2 mention style exists!");
        }

        Logging.LogMessage($"{Name} {BTOS2MentionStyles.Count} BTOS2 mention styles exist!");

        foreach (var (name2, sheet) in LegacyMentionStyles)
        {
            if (sheet)
                Logging.LogMessage($"{Name} {name2} Legacy mention style exists!");
        }

        Logging.LogMessage($"{Name} {LegacyMentionStyles.Count} Legacy mention styles exist!");

        if (PlayerNumbers)
            Logging.LogMessage($"{Name} has a PlayerNumbers sprite sheet!");
    }

    public void Delete()
    {
        Logging.LogMessage($"Deleteing {Name}");

        BaseIcons.ForEach((_, x) => x.Values.ForEach(UObject.Destroy));
        BaseIcons.ForEach((_, x) => x.Clear());
        BaseIcons.Clear();

        EasterEggs.Values.ForEach(x => x.Values.ForEach(y => y.ForEach(UObject.Destroy)));
        EasterEggs.Values.ForEach(x => x.Values.ForEach(y => y.Clear()));
        EasterEggs.Values.ForEach(x => x.Clear());
        EasterEggs.Clear();

        MentionStyles.Values.ForEach(UObject.Destroy);
        MentionStyles.Clear();

        BTOS2MentionStyles.Values.ForEach(UObject.Destroy);
        BTOS2MentionStyles.Clear();

        LegacyMentionStyles.Values.ForEach(UObject.Destroy);
        LegacyMentionStyles.Clear();
    }

    public void Reload()
    {
        Logging.LogMessage($"Reloading {Name}");
        Delete();
        Load();
        AssetManager.SetScrollSprites();
        Debug();
    }

    public void Load()
    {
        Logging.LogMessage($"Loading {Name}", true);

        try
        {
            foreach (var name in Folders)
            {
                if (!AssetManager.GlobalEasterEggs.ContainsKey(name))
                    AssetManager.GlobalEasterEggs[name] = [];

                BaseIcons[name] = [];
                EasterEggs[name] = [];
                var baseName = name + (name is "Custom" or "PlayerNumbers" ? "" : "Base");
                var baseFolder = Path.Combine(PackPath, baseName);

                if (Directory.Exists(baseFolder))
                {
                    foreach (var type in FileTypes)
                    {
                        foreach (var file in Directory.GetFiles(baseFolder, $"*.{type}"))
                        {
                            var filePath = Path.Combine(baseFolder, $"{file.SanitisePath()}.{type}");
                            var sprite = AssetManager.LoadDiskSprite(filePath.SanitisePath(), baseName, Name, type);

                            if (sprite)
                                BaseIcons[name][filePath.SanitisePath(true)] = sprite;
                        }
                    }
                }
                else
                {
                    Logging.LogWarning($"{Name} {baseName} folder doesn't exist");
                    Directory.CreateDirectory(baseFolder);
                }

                if (name is not ("Custom" or "PlayerNumbers"))
                {
                    var eeName = name + "EasterEggs";
                    var eeFolder = Path.Combine(PackPath, eeName);

                    if (Directory.Exists(eeFolder))
                    {
                        foreach (var type in FileTypes)
                        {
                            foreach (var file in Directory.GetFiles(eeFolder, $"*.{type}"))
                            {
                                var filePath = Path.Combine(eeFolder, $"{file.SanitisePath()}.{type}");
                                var sprite = AssetManager.LoadDiskSprite(filePath.SanitisePath(), eeName, Name, type);
                                filePath = filePath.SanitisePath(true);

                                if (sprite == null)
                                    continue;

                                if (!EasterEggs[name].ContainsKey(filePath))
                                    EasterEggs[name][filePath] = [];

                                if (!AssetManager.GlobalEasterEggs[name].ContainsKey(filePath))
                                    AssetManager.GlobalEasterEggs[name][filePath] = [];

                                EasterEggs[name][filePath].Add(sprite);
                                AssetManager.GlobalEasterEggs[name][filePath].Add(sprite);
                            }
                        }
                    }
                    else
                    {
                        Logging.LogWarning($"{Name} {eeName} folder doesn't exist");
                        Directory.CreateDirectory(eeFolder);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Logging.LogError(e);
        }

        // love ya pat

        var (rolesWithIndexDict, rolesWithIndex) = (new Dictionary<string, string>(), new Dictionary<string, int>());

        if (BaseIcons.TryGetValue("PlayerNumbers", out var list2) && list2.Count > 0)
        {
            try
            {
                var textures = new List<Texture2D>();
                var sprites = new List<Sprite>();

                for (var i = 0; i < 16; i++)
                {
                    var sprite = GetSprite($"{i}", false, "PlayerNumbers", false);

                    if (sprite == AssetManager.Blank || sprite == null)
                        sprite = Witchcraft.Witchcraft.Assets.TryGetValue($"{i}", out var sprite1) ? sprite1 : AssetManager.Blank;

                    if (sprite != AssetManager.Blank && sprite != null)
                    {
                        sprite.name = sprite.texture.name = $"PlayerNumbers_{i}";
                        textures.Add(sprite.texture);
                        sprites.Add(sprite);
                    }
                    else
                        Logging.LogWarning($"NO PLAYER NUMBER ICON FOR {i}?!");

                    rolesWithIndexDict.Add($"PlayerNumbers_{i}", $"PlayerNumbers_{i}");
                }

                PlayerNumbers = AssetManager.BuildGlyphs([..sprites], [..textures], $"PlayerNumbers ({Name})", rolesWithIndexDict, false);
                Utils.DumpSprite(PlayerNumbers.spriteSheet as Texture2D, "PlayerNumbers", PackPath);
            }
            catch (Exception e)
            {
                Logging.LogError(e);
                PlayerNumbers = null;
            }
        }

        (rolesWithIndexDict, rolesWithIndex) = Utils.Filtered();

        foreach (var style in Folders)
        {
            if (/*IsLegacyModdedFolder(style) ||*/ IsBTOS2ModdedFolder(style) || style == "PlayerNumbers")
                continue;

            try
            {
                var textures = new List<Texture2D>();
                var sprites = new List<Sprite>();

                foreach (var (role, roleInt) in rolesWithIndex)
                {
                    var actualRole = (Role)roleInt;
                    var name = Utils.RoleName(actualRole, ModType.Vanilla);
                    var sprite = GetSprite(name, false, style, false);

                    if ((sprite == AssetManager.Blank || sprite == null) && style != "Regular")
                        sprite = GetSprite(name, false, "Regular", false);

                    if (sprite == AssetManager.Blank || sprite == null)
                        sprite = Witchcraft.Witchcraft.Assets.TryGetValue(name, out var sprite1) ? sprite1 : AssetManager.Blank;

                    if (sprite != AssetManager.Blank && sprite != null)
                    {
                        sprite.name = sprite.texture.name = role;
                        textures.Add(sprite.texture);
                        sprites.Add(sprite);
                    }
                    else
                        Logging.LogWarning($"NO ICON FOR {name}?!");
                }

                var asset = AssetManager.BuildGlyphs([..sprites], [..textures], $"RoleIcons ({Name}, {style})", rolesWithIndexDict);
                MentionStyles[style] = asset;
                Utils.DumpSprite(asset.spriteSheet as Texture2D, $"{style}RoleIcons", PackPath);
            }
            catch (Exception e)
            {
                Logging.LogError(e);
                MentionStyles[style] = null;
            }
        }

        if (Constants.BTOS2Exists)
        {
            (rolesWithIndexDict, rolesWithIndex) = Utils.Filtered(ModType.BTOS2);

            foreach (var style in Folders)
            {
                if (style is "Executioner" or "PlayerNumbers")
                    continue;

                try
                {
                    var textures = new List<Texture2D>();
                    var sprites = new List<Sprite>();

                    foreach (var (role, roleInt) in rolesWithIndex)
                    {
                        var name = Utils.RoleName((Role)roleInt, ModType.BTOS2);
                        var sprite = GetSprite(name, false, style, false);

                        if ((sprite == AssetManager.Blank || sprite == null) && style != "Regular")
                            sprite = GetSprite(name, false, "Regular", false);

                        if (sprite == AssetManager.Blank || sprite == null)
                            sprite = Witchcraft.Witchcraft.Assets.TryGetValue(name + "_BTOS2", out var sprite1) ? sprite1 : AssetManager.Blank;

                        if (sprite == AssetManager.Blank || sprite == null)
                            sprite = Witchcraft.Witchcraft.Assets.TryGetValue(name, out var sprite1) ? sprite1 : AssetManager.Blank;

                        if (sprite != AssetManager.Blank && sprite != null)
                        {
                            sprite.name = sprite.texture.name = role;
                            textures.Add(sprite.texture);
                            sprites.Add(sprite);
                        }
                        else
                            Logging.LogWarning($"NO BTOS2 ICON FOR {name}?!");
                    }

                    var asset = AssetManager.BuildGlyphs([..sprites], [..textures], $"BTOSRoleIcons ({Name}, {style})", rolesWithIndexDict);
                    BTOS2MentionStyles[style] = asset;
                    Utils.DumpSprite(asset.spriteSheet as Texture2D, $"{style}BTOS2RoleIcons", PackPath);
                }
                catch (Exception e)
                {
                    Logging.LogError(e);
                    BTOS2MentionStyles[style] = null;
                }
            }
        }

        /*if (Constants.LegacyExists)
        {
            (rolesWithIndexDict, rolesWithIndex) = Utils.Filtered(ModType.Legacy);

            foreach (var style in Folders)
            {
                if (style == "PlayerNumbers")
                    continue;

                try
                {
                    var textures = new List<Texture2D>();
                    var sprites = new List<Sprite>();

                    foreach (var (role, roleInt) in rolesWithIndex)
                    {
                        var name = Utils.RoleName((Role)roleInt, ModType.Legacy);
                        var sprite = GetSprite(name, false, style, false);

                        if ((sprite == AssetManager.Blank || sprite == null) && style != "Regular")
                            sprite = GetSprite(name, false, "Regular", false);

                        if (sprite == AssetManager.Blank || sprite == null)
                            sprite = Witchcraft.Witchcraft.Assets.TryGetValue(name + "_Legacy", out var sprite1) ? sprite1 : AssetManager.Blank;

                        if (sprite == AssetManager.Blank || sprite == null)
                            sprite = Witchcraft.Witchcraft.Assets.TryGetValue(name, out var sprite1) ? sprite1 : AssetManager.Blank;

                        if (sprite != AssetManager.Blank && sprite != null)
                        {
                            sprite.name = sprite.texture.name = role;
                            textures.Add(sprite.texture);
                            sprites.Add(sprite);
                        }
                        else
                            Logging.LogWarning($"NO LEGACY ICON FOR {name}?!");
                    }

                    var asset = AssetManager.BuildGlyphs([..sprites], [..textures], $"LegacyRoleIcons ({Name}, {style})", rolesWithIndexDict);
                    LegacyMentionStyles[style] = asset;
                    Utils.DumpSprite(asset.spriteSheet as Texture2D, $"{style}LegacyRoleIcons", PackPath);
                }
                catch (Exception e)
                {
                    Logging.LogError(e);
                    LegacyMentionStyles[style] = null;
                }
            }
        }*/

        Logging.LogMessage($"{Name} Loaded!", true);
    }

    public Sprite GetSprite(string name, bool allowEE, string type, bool log)
    {
        if (!BaseIcons[type].TryGetValue(name, out var sprite))
        {
            if (log)
                Logging.LogWarning($"Couldn't find {name} in {Name}'s {type} resources");

            if (type != "Regular" && !BaseIcons["Regular"].TryGetValue(name, out sprite))
            {
                if (log)
                    Logging.LogWarning($"Couldn't find {name} in {Name}'s Regular resources");

                sprite = null;
            }
        }

        if ((URandom.RandomRangeInt(1, 101) <= Constants.EasterEggChance && allowEE) || !sprite)
        {
            var sprites = new List<Sprite>();

            if (Constants.AllEasterEggs)
            {
                if (!AssetManager.GlobalEasterEggs[type].TryGetValue(name, out sprites))
                {
                    if (type != "Regular")
                        AssetManager.GlobalEasterEggs["Regular"].TryGetValue(name, out sprites);
                }

                sprites ??= [];
            }

            if (sprites.Count == 0)
            {
                if (!EasterEggs[type].TryGetValue(name, out sprites))
                {
                    if (type != "Regular")
                        EasterEggs["Regular"].TryGetValue(name, out sprites);
                }

                sprites ??= [];
            }

            if (sprites.Count > 0)
                return sprites.Random();
        }

        return sprite ?? AssetManager.Blank;
    }

    public static implicit operator bool(IconPack exists) => exists != null;

    private static bool IsBTOS2ModdedFolder(string folderName) => folderName is "Jackal" or "Judge" or "Auditor" or "Starspawn" or "Inquisitor" or "Lions" or "Frogs" or "Hawks" or "Pandora"
        or "Compliant" or "Egoist";

    //private static bool IsLegacyModdedFolder(string folderName) => folderName is "Mafia" or "Amnesiac" or "Juggernaut" or "GuardianAngel" or "Evils";
}