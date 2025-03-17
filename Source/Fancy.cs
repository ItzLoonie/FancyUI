namespace FancyUI;

[SalemMod, WitchcraftMod(typeof(Fancy), "Fancy UI", [ "Assets", "WoodMaterials" ], true)]
public class Fancy
{
    public static WitchcraftMod Instance { get; private set; }

    public void Start()
    {
        Instance = ModSingleton<Fancy>.Instance!;

        Instance.Message("Fancifying...", true);

        Assets = Instance.Assets;

        if (!Directory.Exists(IPPath))
            Directory.CreateDirectory(IPPath);

        var json = Path.Combine(IPPath, "OtherPacks.json");

        if (!File.Exists(json))
            File.WriteAllText(json, "");

        var vanilla = Path.Combine(IPPath, "Vanilla");

        if (!Directory.Exists(vanilla))
            Directory.CreateDirectory(vanilla);

        if (!Directory.Exists(SSPath))
            Directory.CreateDirectory(SSPath);

        json = Path.Combine(SSPath, "OtherSets.json");

        if (!File.Exists(json))
            File.WriteAllText(json, "");

        FixStyles.RefreshStyles();

        vanilla = Path.Combine(SSPath, "Vanilla");

        if (!Directory.Exists(vanilla))
            Directory.CreateDirectory(vanilla);

        try
        {
            LoadBtos();
        } catch {}

        Instance.Message("Fancy!", true);
    }

    public static AssetManager Assets { get; private set; }

    [UponAssetsLoaded]
    public static void UponLoad()
    {
        Blank = Assets.GetSprite("Blank");
        FancyAssetManager.Attack = Assets.GetSprite("Attack");
        FancyAssetManager.Defense = Assets.GetSprite("Defense");
        Ethereal = Assets.GetSprite("Ethereal");

        Grayscale = Assets.GetMaterial("GrayscaleM");

        LoadingGif = Assets.GetGif("Placeholder")!;
        LoadingGif.RenderAllFrames();
        Loading = new("Loading") { Frames = [ .. LoadingGif.Frames.Select(x => x.RenderedSprite) ] };

        Flame = Assets.GetGif("Flame")!;
        Flame.RenderAllFrames();

        TryLoadingSprites(Constants.CurrentPack(), PackType.IconPacks);
        LoadVanillaSpriteSheets();

        try
        {
            LoadBtos2SpriteSheet();
        } catch {}

        MenuButton.FancyMenu.Icon = Assets.GetSprite("Thumbnail");
    }

    public static StringDropdownOption SelectedIconPack;
    public static StringDropdownOption SelectedSilhouetteSet;

    public static EnumDropdownOption<UITheme> SelectedUITheme;

    public static StringDropdownOption MentionStyle1;
    public static StringDropdownOption MentionStyle2;

    public static StringDropdownOption FactionOverride1;
    public static StringDropdownOption FactionOverride2;

    public static ColorOption MainUIThemeWood;
    public static ColorOption MainUIThemePaper;
    public static ColorOption MainUIThemeLeather;
    public static ColorOption MainUIThemeMetal;
    public static ColorOption MainUIThemeFire;
    public static ColorOption MainUIThemeWax;

    public static FloatOption EasterEggChance;
    public static FloatOption AnimationDuration;

    public static FloatOption PlayerNumber;

    public static ToggleOption CustomNumbers;
    public static ToggleOption AllEasterEggs;
    public static ToggleOption PlayerPanelEasterEggs;
    public static ToggleOption DumpSpriteSheets;
    public static ToggleOption DebugPackLoading;
    public static ToggleOption ShowOverlayWhenJailed;
    public static ToggleOption ShowOverlayAsJailor;
    public static ToggleOption IconsInRoleReveal;

    /* public static ToggleOption MiscRoleCustomisation;
    public static StringOption RecruitLabel;
    public static StringOption TraitorLabel;
    public static StringOption VIPLabel;
    public static StringOption CourtLabel;
    public static StringOption JuryLabel;
    public static StringOption PirateLabel;
    public static ColorOption TownStart;
    public static ColorOption TownEnd;
    public static ColorOption CovenStart;
    public static ColorOption CovenEnd;
    public static ColorOption ApocalypseStart;
    public static ColorOption ApocalypseEnd;
    public static ColorOption VampireStart;
    public static ColorOption VampireEnd;
    public static ColorOption CursedSoulStart;
    public static ColorOption CursedSoulEnd;
    public static ColorOption PandoraStart;
    public static ColorOption PandoraEnd;
    public static ColorOption ComplianceStart;
    public static ColorOption ComplianceEnd;
    public static ColorOption SerialKillerStart;
    public static ColorOption SerialKillerEnd;
    public static ColorOption ArsonistStart;
    public static ColorOption ArsonistEnd;
    public static ColorOption WerewolfStart;
    public static ColorOption WerewolfEnd;
    public static ColorOption ShroudStart;
    public static ColorOption ShroudEnd;
    public static ColorOption JackalStart;
    public static ColorOption JackalEnd;
    public static ColorOption FrogsStart;
    public static ColorOption FrogsEnd;
    public static ColorOption HawksStart;
    public static ColorOption HawksEnd;
    public static ColorOption LionsStart;
    public static ColorOption LionsEnd;
    public static ColorOption EgotistStart;
    public static ColorOption EgotistEnd;
    public static ColorOption JesterStart;
    public static ColorOption JesterEnd;
    public static ColorOption ExecutionerStart;
    public static ColorOption ExecutionerEnd;
    public static ColorOption DoomsayerStart;
    public static ColorOption DoomsayerEnd;
    public static ColorOption PirateStart;
    public static ColorOption PirateEnd;
    public static ColorOption InquisitorStart;
    public static ColorOption InquisitorEnd;
    public static ColorOption StarspawnStart;
    public static ColorOption StarspawnEnd;
    public static ColorOption JudgeStart;
    public static ColorOption JudgeEnd;
    public static ColorOption AuditorStart;
    public static ColorOption AuditorEnd;*/


    [LoadConfigs]
    public static void LoadConfigs()
    {
        SelectedIconPack = new("SELECTED_ICON_PACK", "Vanilla", PackType.IconPacks, () => GetPackNames(PackType.IconPacks), onChanged: x => TryLoadingSprites(x, PackType.IconPacks));
        SelectedSilhouetteSet = new("SELECTED_SIL_SET", "Vanilla", PackType.SilhouetteSets, () => GetPackNames(PackType.SilhouetteSets), onChanged: x => TryLoadingSprites(x,
            PackType.SilhouetteSets));

        SelectedUITheme = new("SELECTED_UI_THEME", UITheme.Default, PackType.RecoloredUI, useTranslations: true);

        MentionStyle1 = new("MENTION_STYLE_1", "Regular", PackType.IconPacks, () => GetOptions(ModType.Vanilla, true), _ => Constants.EnableIcons());
        MentionStyle2 = new("MENTION_STYLE_2", "Regular", PackType.IconPacks, () => GetOptions(ModType.BTOS2, true), _ => Constants.BTOS2Exists() && Constants.EnableIcons());

        FactionOverride1 = new("FACTION_OVERRIDE_1", "None", PackType.IconPacks, () => GetOptions(ModType.Vanilla, false), _ => Constants.EnableIcons());
        FactionOverride2 = new("FACTION_OVERRIDE_2", "None", PackType.IconPacks, () => GetOptions(ModType.BTOS2, false), _ => Constants.BTOS2Exists() && Constants.EnableIcons());

        MainUIThemeFire = new("UI_FIRE", "#FFFFFF", PackType.RecoloredUI, _ => Constants.EnableCustomUI());
        MainUIThemePaper = new("UI_PAPER", "#FFFFFF", PackType.RecoloredUI, _ => Constants.EnableCustomUI());
        MainUIThemeMetal = new("UI_METAL", "#FFFFFF", PackType.RecoloredUI, _ => Constants.EnableCustomUI());
        MainUIThemeLeather = new("UI_LEATHER", "#FFFFFF", PackType.RecoloredUI, _ => Constants.EnableCustomUI());
        MainUIThemeWood = new("UI_WOOD", "#FFFFFF", PackType.RecoloredUI, _ => Constants.EnableCustomUI());
        MainUIThemeWax = new("UI_WAX", "#FFFFFF", PackType.RecoloredUI, _ => Constants.EnableCustomUI());

        PlayerNumber = new("PLAYER_NUMBER", 0, PackType.IconPacks, 0, 15, true, _ => Constants.CustomNumbers());

        EasterEggChance = new("EE_CHANCE", 5, PackType.IconPacks, 0, 100, true, _ => Constants.EnableIcons());
        AnimationDuration = new("ANIM_DURATION", 2, PackType.SilhouetteSets, 0.5f, 10, setActive: _ => Constants.EnableSwaps());

        CustomNumbers = new("CUSTOM_NUMBERS", false, PackType.IconPacks, _ => Constants.EnableIcons());
        AllEasterEggs = new("ALL_EE", false, PackType.IconPacks, _ => Constants.EnableIcons());
        PlayerPanelEasterEggs = new("PLAYER_PANEL_EE", false, PackType.IconPacks, _ => Constants.EnableIcons());
        DumpSpriteSheets = new("DUMP_SHEETS", false, PackType.Settings);
        DebugPackLoading = new("DEBUG_LOADING", false, PackType.Settings);
        ShowOverlayWhenJailed = new("SHOW_TO_JAILED", true, PackType.Settings);
        ShowOverlayAsJailor = new("SHOW_TO_JAILOR", false, PackType.Settings);
        IconsInRoleReveal = new("ROLE_REVEAL_ICONS", true, PackType.Settings);


        /* MiscRoleCustomisation = new("MRC", false, PackType.MiscRoleCustomisation);
        RecruitLabel = new("RECRUIT_LABEL", "Recruited", PackType.MiscRoleCustomisation, _ => Constants.MRC())
        TraitorLabel = new("TRAITOR_LABEL", "Recruited", PackType.MiscRoleCustomisation, _ => Constants.MRC())
        VIPLabel = new("VIP_LABEL", "Recruited", PackType.MiscRoleCustomisation, _ => Constants.MRC())
        CourtLabel = new("COURT_LABEL", "Recruited", PackType.MiscRoleCustomisation, _ => Constants.MRC())
        JuryLabel = new("JURY_LABEL", "Recruited", PackType.MiscRoleCustomisation, _ => Constants.MRC())
        PirateLabel = new("PIRATE_LABEL", "Recruited", PackType.MiscRoleCustomisation, _ => Constants.MRC())
        TownStart = new("TOWN_START", "#06E00C", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        TownEnd = new("TOWN_END", "#06E00C", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        CovenStart = new("COVEN_START", "#B545FF", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        CovenEnd = new("COVEN_END", "#B545FF", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        ApocalypseStart = new("APOCALYPSE_START", "#FF004E", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        ApocalypseEnd = new("APOCALYPSE_END", "#FF004E", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        JesterStart = new("JESTER_START", "#F5A6D4", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        JesterEnd = new("JESTER_END", "#F5A6D4", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        DoomsayerStart = new("DOOMSAYER_START", "#00CC99", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        DoomsayerEnd = new("DOOMSAYER_END", "#00CC99", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        PirateStart = new("PIRATE_START", "#ECC23E", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        PirateEnd = new("PIRATE_END", "#ECC23E", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        ExecutionerStart = new("EXECUTIONER_START", "#949797", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        ExecutionerEnd = new("EXECUTIONER_END", "#949797", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        InquisitorStart = new("INQUISITOR_START", "#821252", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        InquisitorEnd = new("INQUISITOR_END", "#821252", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        ArsonistStart = new("ARSONIST_START", "#DB7601", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        ArsonistEnd = new("ARSONIST_END", "#DB7601", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        SerialKillerStart = new("SERIALKILLER_START", "#1D4DFC", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        SerialKillerEnd = new("SERIALKILLER_END", "#1D4DFC", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        ShroudStart = new("SHROUD_START", "#6699FF", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        ShroudEnd = new("SHROUD_END", "#6699FF", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        WerewolfStart = new("WEREWOLF_START", "#9D7038", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        WerewolfEnd = new("WEREWOLF_END", "#9D7038", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        VampireStart = new("VAMPIRE_START", "#A22929", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        VampireEnd = new("VAMPIRE_END", "#A22929", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        AuditorStart = new("AUDITOR_START", "#AEBA87", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        AuditorEnd = new("AUDITOR_END", "#E8FCC5", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        JudgeStart = new("JUDGE_START", "#C77364", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        JudgeEnd = new("JUDGE_END", "#C93D50", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        StarspawnStart = new("STARSPAWN_START", "#FCE79A", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        StarspawnEnd = new("STARSPAWN_END", "#999CFF", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        CursedSoulStart = new("CURSEDSOUL_START", "#4FFF9F", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        CursedSoulEnd = new("CURSEDSOUL_END", "#B54FFF", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        JackalStart = new("JACKAL_START", "#404040", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        JackalEnd = new("JACKAL_END", "#D0D0D0", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        PandoraStart = new("PANDORA_START", "#B545FF", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        PandoraEnd = new("PANDORA_END", "#FF004E", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        ComplianceStart = new("COMPLIANCE_START", "#2D44B5", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        ComplianceMiddle = new("COMPLIANCE_MIDDLE", "#AE1B1E", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        ComplianceEnd = new("COMPLIANCE_END", "#FC9F32", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        EgotistStart = new("EGOTIST_START", "#359f3f", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        EgotistEnd = new("EGOTIST_END", "#3f359f", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        Neutral = new("NEUTRAL", "#A9A9A9", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        StonedHidden = new("STONED_HIDDEN", "#9C9A9A", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        Lovers = new("Lovers", "#FEA6FA", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        FrogsStart = new("FROGS_START", "#1e49cf", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        FrogsEnd = new("FROGS_END", "#1e49cf", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        LionsStart = new("LIONS_START", "#ffc34f", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        LionsEnd = new("LIONS_END", "#ffc34f", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        HawksStart = new("HAWKS_START", "#a81538", PackType.MiscRoleCustomisation, _ => Constants.MRC());
        HawksEnd = new("HAWKS_END", "#a81538", PackType.MiscRoleCustomisation, _ => Constants.MRC()); */

    }

    private static IEnumerable<string> GetPackNames(PackType type)
    {
        yield return "Vanilla";

        foreach (var dir in Directory.EnumerateDirectories(Path.Combine(Instance.ModPath, type.ToString())))
        {
            if (!dir.ContainsAny("Vanilla", "BTOS2"))
                yield return dir.FancySanitisePath();
        }
    }

    private static IEnumerable<string> GetOptions(ModType mod, bool mentionStyle)
    {
        try
        {
            var result = new List<string>();

            if (IconPacks.TryGetValue(Constants.CurrentPack(), out var pack))
            {
                result.Add(mentionStyle ? "Regular" : "None");

                if (pack.Assets.TryGetValue(ModType.Common, out var assets))
                {
                    foreach (var (folder, icons) in assets.BaseIcons)
                    {
                        if (icons.Count > 0 && !result.Contains(folder) && folder != "Custom")
                            result.Add(folder);
                    }
                }

                if (pack.Assets.TryGetValue(mod, out assets))
                {
                    foreach (var (folder, icons) in assets.BaseIcons)
                    {
                        if (icons.Count > 0 && !result.Contains(folder) && folder != "Custom")
                            result.Add(folder);
                    }
                }

                result.Add("Custom");
            }
            else
                result.Add("None");

            if (mentionStyle)
                result.Add(mod.ToString());
            else
                result.Remove("Regular");

            return result;
        }
        catch
        {
            return [ mentionStyle ? mod.ToString() : "None" ];
        }
    }

    // private static void AttemptCreateSpriteSheet(FancyUI.ModType mod, string name)
    // {
    //     if (AssetManager.IconPacks.TryGetValue(Constants.CurrentPack(), out var pack))
    //     {
    //         if (!pack.Assets.TryGetValue(mod, out var iconAssets))
    //             return;

    //         var modName = mod.ToString();

    //         if ((!iconAssets.MentionStyles.TryGetValue(name, out var asset) || !asset) && iconAssets.BaseIcons.TryGetValue(name, out var baseIcons))
    //         {
    //             iconAssets.MentionStyles[name] = asset = pack.BuildSpriteSheet(mod, modName, name, baseIcons);
    //             Utils.DumpSprite(asset?.spriteSheet as Texture2D, $"{name}{mod}RoleIcons", Path.Combine(pack.PackPath, modName));
    //         }
    //     }
    // }
}

[SalemMenuItem]
public static class MenuButton
{
    public static readonly SalemMenuButton FancyMenu = new()
    {
        Label = "Fancy UI",
        OnClick = OpenMenu
    };

    private static void OpenMenu()
    {
        var go = UObject.Instantiate(Fancy.Assets.GetGameObject("FancyUI"), CacheHomeSceneController.Controller.SafeArea.transform, false);
        go.transform.localPosition = new(0, 0, 0);
        go.transform.localScale = Vector3.one * 2f;
        go.AddComponent<UI.FancyUI>();
    }
}