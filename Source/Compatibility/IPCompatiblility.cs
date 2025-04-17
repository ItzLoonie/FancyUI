namespace FancyUI.Compatibility;

public static class Btos2IPCompatibility
{
    private static FieldInfo RoleIcon;

    private static FieldInfo Role;
    private static FieldInfo Icon;
    private static FieldInfo Banned;

    public static bool Init()
    {
        try
        {
            if (!ModStates.EnabledMods.TryFinding(x => x.HarmonyId == "curtis.tuba.better.tos2", out var btos2Mod))
                return false;

            var btos2Assembly = Assembly.LoadFile(btos2Mod!.AssemblyPath);
            var btos2Types = AccessTools.GetTypesFromAssembly(btos2Assembly);
            var roleDeckPlusPanelController = btos2Types.FirstOrDefault(x => x.Name == "RoleDeckPlusPanelController")!;
            var deckItem = roleDeckPlusPanelController.GetNestedTypes(AccessTools.all).FirstOrDefault(x => x.Name == "DeckItem");
            var roleSelectionPlusController = btos2Types.FirstOrDefault(x => x.Name == "RoleSelectionPlusController")!;
            var menuRole = roleSelectionPlusController.GetNestedTypes(AccessTools.all).FirstOrDefault(x => x.Name == "MenuRole");

            RoleIcon = AccessTools.Field(deckItem, "roleIcon");

            Icon = AccessTools.Field(menuRole, "icon");
            Role = AccessTools.Field(menuRole, "role");
            Banned = AccessTools.Field(menuRole, "banned");

            var setData = AccessTools.Method(deckItem, "SetData", [ typeof(Role), typeof(FactionType), typeof(bool), roleDeckPlusPanelController ]);
            var refreshData = AccessTools.Method(menuRole, "RefreshData");
            var validateStartButtonState = AccessTools.Method(roleDeckPlusPanelController, "ValidateStartButtonState");
            var compat = typeof(Btos2IPCompatibility);

            Btos2Compatibility.Btos2PatchesHarmony.Patch(setData, null, new(AccessTools.Method(compat, nameof(ItemPostfix1))));
            Btos2Compatibility.Btos2PatchesHarmony.Patch(refreshData, null, new(AccessTools.Method(compat, nameof(ItemPostfix2))));
            Btos2Compatibility.Btos2PatchesHarmony.Patch(validateStartButtonState, null, new(AccessTools.Method(compat, nameof(RoleDeckPostfix))));
            Fancy.Instance.Message("BTOS2 compatibility was successful");
            return true;
        }
        catch (Exception ex)
        {
            Fancy.Instance.Error($"BTOS2 compatibility patch loading failed because:\n{ex}");
            return false;
        }
    }

    public static void ItemPostfix1(dynamic __instance, ref Role a_role, ref FactionType faction, ref bool isBan)
    {
        if (!Constants.EnableIcons())
            return;

        var roleIcon = (Image)RoleIcon.GetValue(__instance);

        if (!roleIcon)
            return;

        if (isBan)
        {
            var sprite2 = GetSprite("Banned");

            if (sprite2.IsValid())
                roleIcon.sprite = sprite2;

            return;
        }

        var ogfaction = a_role.GetFactionType(ModType.BTOS2);
        var reg = ogfaction != faction;
        var name = Utils.RoleName(a_role);
        var sprite = GetSprite(reg, name, Utils.FactionName(faction));

        if (!sprite.IsValid() && reg)
            sprite = GetSprite(name, Utils.FactionName(ogfaction));

        if (sprite.IsValid())
            roleIcon.sprite = sprite;
    }

    public static void ItemPostfix2(dynamic __instance)
    {
        if (!Constants.EnableIcons())
            return;

        var roleIcon = (Image)Icon.GetValue(__instance);

        if (!roleIcon)
            return;

        var role = (Role)Role.GetValue(__instance);
        var sprite = GetSprite(Utils.RoleName(role), Utils.FactionName(role.GetFactionType(ModType.BTOS2)));

        if (sprite.IsValid())
            roleIcon.sprite = sprite;

        var banned = (GameObject)Banned.GetValue(__instance);

        if (!banned)
            return;

        var bannedIcon = banned.GetComponent<Image>();
        var bannedSprite = GetSprite("Banned");

        if (bannedIcon && bannedSprite.IsValid())
            bannedIcon.sprite = bannedSprite;
    }

    public static void RoleDeckPostfix(dynamic __instance) => ((Image)__instance.GetComponent<Image>()).SetImageColor(ColorType.Paper);
}