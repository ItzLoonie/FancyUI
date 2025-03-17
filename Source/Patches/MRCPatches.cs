using BetterTOS2;
using Cinematics.Players;
using Game;
using Game.Characters;
using Home.HomeScene;
using Home.Shared;
using Mentions;
using Server.Shared.Cinematics;
using Server.Shared.Cinematics.Data;
using Server.Shared.Extensions;

namespace FancyUI.Patches;

[HarmonyPatch(typeof(ClientRoleExtensions), nameof(ClientRoleExtensions.ToColorizedDisplayString), typeof(Role), typeof(FactionType))]
public static class AddChangedConversionTags
{
    public static void Postfix(ref string __result, ref Role role, ref FactionType factionType)
    {
        if (role.IsResolved() || role is Role.FAMINE or Role.DEATH or Role.PESTILENCE or Role.WAR)
        {
            string text = role.ToDisplayString();

            if (factionType.GetChangedGradient() != null)
                __result = ApplyGradient(text, factionType.GetChangedGradient());
            else
                __result = $"<color={factionType.GetFactionColor()}>{text}</color>";
        }
    }

    public static string ApplyGradient(string text, Color color1, Color color2)
    {
        var gradient = new Gradient();
        gradient.SetKeys(
        [
            new(color1, 0f),
            new(color2, 1f)
        ],
        [
            new(1f, 0f),
            new(1f, 1f)
        ]);
        var text2 = "";

        for (var i = 0; i < text.Length; i++)
            text2 += $"<color={ToHexString(gradient.Evaluate((float)i / text.Length))}>{text[i]}</color>";

        return text2;
    }

    public static string ApplyThreeColorGradient(string text, Color color1, Color color2, Color color3)
    {
        var gradient = new Gradient();
        gradient.SetKeys(
        [
            new(color1, 0f),
            new(color2, 0.5f),
            new(color3, 1f)
        ],
        [
            new(1f, 0f),
            new(1f, 1f)
        ]);
        var text2 = "";

        for (var i = 0; i < text.Length; i++)
            text2 += $"<color={ToHexString(gradient.Evaluate((float)i / text.Length))}>{text[i]}</color>";

        return text2;
    }

    public static string ApplyGradient(string text, Gradient gradient)
    {
        var text2 = "";

        for (var i = 0; i < text.Length; i++)
            text2 += $"<color={ToHexString(gradient.Evaluate((float)i / text.Length))}>{text[i]}</color>";

        return text2;
    }

    public static string ToHexString(Color color)
    {
        Color32 color2 = color;
        return $"#{color2.r:X2}{color2.g:X2}{color2.b:X2}";
    }
}

[HarmonyPatch(typeof(MentionsProvider), nameof(MentionsProvider.DecodeSpeaker))]
public static class FancyChatExperimentalBTOS2
{
    public static List<int> ExcludedIds = [50, 69, 70, 71];

    public static bool Prefix(MentionsProvider __instance, ref string __result, string encodedText, int position, bool isAlive)
    {
        var text = Service.Home.UserService.Settings.ChatNameColor switch
        {
            1 => "B0B0B0",
            2 => "CC009E",
            _ => "FCCE3B",
        };
        var text2 = encodedText;

        if (!ExcludedIds.Contains(position))
        {
            if (isAlive)
            {
                var flag = Service.Game.Sim.simulation.observations.playerEffects.Any(x => x.Data.effects.Contains((EffectType)100) && x.Data.playerPosition == position);

                if (Utils.GetRoleInfo(position, out var playerInfo))
                {
                    if (playerInfo.Item2.GetChangedGradient() != null)
                    {
                        var gradient = playerInfo.Item2.GetChangedGradient();

                        if (flag)
                            gradient = ((FactionType)33).GetChangedGradient();

                        var text3 = "";

                        if (playerInfo.Item2 == ((FactionType)44))
                        {
                            text3 = AddChangedConversionTags.ApplyThreeColorGradient(Pepper.GetDiscussionPlayerByPosition(position).gameName + ":", gradient.Evaluate(0f),
                                gradient.Evaluate(0.5f), gradient.Evaluate(1f));
                        }
                        else
                            text3 = AddChangedConversionTags.ApplyGradient(Pepper.GetDiscussionPlayerByPosition(position).gameName + ":", gradient.Evaluate(0f), gradient.Evaluate(1f));

                        text2 = text2.Replace(string.Concat(
                        [
                            "<color=#",
                            ColorUtility.ToHtmlStringRGB(Pepper.GetDiscussionPlayerRoleColor(position)),
                            ">",
                            Pepper.GetDiscussionPlayerByPosition(position).gameName,
                            ":"
                        ]), text3);
                    }
                    else if (flag)
                    {
                        var gradient2 = ((FactionType)33).GetChangedGradient();
                        var text4 = AddChangedConversionTags.ApplyGradient(Pepper.GetDiscussionPlayerByPosition(position).gameName + ":", gradient2.Evaluate(0f), gradient2.Evaluate(1f));
                        text2 = text2.Replace(string.Concat(
                        [
                            "<color=#",
                            ColorUtility.ToHtmlStringRGB(Pepper.GetDiscussionPlayerRoleColor(position)),
                            ">",
                            Pepper.GetDiscussionPlayerByPosition(position).gameName,
                            ":"
                        ]), text4);
                    }
                    else
                    {
                        var text5 = ColorUtility.ToHtmlStringRGB(Utils.GetPlayerRoleColor(position));
                        text2 = text2.Replace("<color=#" + text + ">", "<color=#" + text5 + ">");
                    }
                }
                else if (flag)
                {
                    var gradient3 = ((FactionType)33).GetChangedGradient();
                    var text6 = AddChangedConversionTags.ApplyGradient(Pepper.GetDiscussionPlayerByPosition(position).gameName + ":", gradient3.Evaluate(0f), gradient3.Evaluate(1f));
                    text2 = text2.Replace(string.Concat(
                    [
                        "<color=#",
                        text,
                        ">",
                        Pepper.GetDiscussionPlayerByPosition(position).gameName,
                        ":"
                    ]), text6);
                }
            }
        }

        __result = __instance.ProcessSpeakerName(text2, position, isAlive);
        return false;
    }
}

[HarmonyPatch(typeof(ClientRoleExtensions), nameof(ClientRoleExtensions.ToColorizedDisplayString), typeof(Role), typeof(FactionType))]
public static class AddTTAndGradients
{
    [HarmonyPostfix]
    public static void Result(ref string __result, ref Role role, ref FactionType factionType)
    {
        var newtext = "";

        if (__result.Contains("<color=#B545FF>(Traitor)"))
            __result = __result.Replace("<color=#B545FF>(Traitor)</color>", "<style=CovenColor>(Traitor)</style>");

        if (RoleExtensions.IsResolved(role) || role is Role.FAMINE or Role.DEATH or Role.PESTILENCE or Role.WAR)
        {
            var text = "";
            text = ClientRoleExtensions.ToDisplayString(role);
            text = ModSettings.GetBool("Faction-Specific Role Names") ? Utils.ToRoleFactionDisplayString(role, factionType) : ClientRoleExtensions.ToDisplayString(role);

            if (factionType.GetChangedGradient() != null)
                newtext = AddChangedConversionTags.ApplyGradient(text, factionType.GetChangedGradient());
            else
            {
                newtext = string.Concat(
                [
                    "<color=",
                    ClientRoleExtensions.GetFactionColor(factionType),
                    ">",
                    text,
                    "</color>"
                ]);
            }

            if (RoleExtensions.GetFaction(role) != factionType && factionType != FactionType.NONE && ModSettings.GetBool("Faction Name Next to Role"))
            {
                if (factionType is not ((FactionType)33 or (FactionType)44))
                {
                    if (factionType.GetChangedGradient() != null)
                    {
                        newtext += " " + AddChangedConversionTags.ApplyGradient("(" + factionType.ToDisplayString() + ")", factionType.GetChangedGradient().Evaluate(0f),
                            factionType.GetChangedGradient().Evaluate(1f));
                    }
                    else
                        newtext += " " + "<color=" + ClientRoleExtensions.GetFactionColor(factionType) + ">(" + factionType.ToDisplayString() + ")</color>";
                }
                else if (factionType == (FactionType)33)
                {
                    newtext += " " + AddChangedConversionTags.ApplyGradient("(" + ModSettings.GetString("Recruit Label", "det.rolecustomizationmod") + ")",
                        factionType.GetChangedGradient().Evaluate(0f), factionType.GetChangedGradient().Evaluate(1f));
                }
                else if (factionType == (FactionType)44)
                {
                    newtext += " " + AddChangedConversionTags.ApplyThreeColorGradient("(" + factionType.ToDisplayString() + ")", factionType.GetChangedGradient().Evaluate(0f),
                        factionType.GetChangedGradient().Evaluate(0.5f), factionType.GetChangedGradient().Evaluate(1f));
                }

            }

            __result = newtext;
        }
    }
}

[HarmonyPatch(typeof(ClientRoleExtensions), nameof(ClientRoleExtensions.GetFactionColor))]
public static class SwapColor
{
    [HarmonyPostfix]
    public static void Swap(ref string __result, ref FactionType factionType)
    {
        __result = (int)factionType switch
        {
            1 => ModSettings.GetString("Town Start", "det.rolecustomizationmod"),
            2 => ModSettings.GetString("Coven Start", "det.rolecustomizationmod"),
            3 => ModSettings.GetString("Serial Killer Start", "det.rolecustomizationmod"),
            4 => ModSettings.GetString("Arsonist Start", "det.rolecustomizationmod"),
            5 => ModSettings.GetString("Werewolf Start", "det.rolecustomizationmod"),
            6 => ModSettings.GetString("Shroud Start", "det.rolecustomizationmod"),
            7 => ModSettings.GetString("Apocalypse Start", "det.rolecustomizationmod"),
            8 => ModSettings.GetString("Executioner Start", "det.rolecustomizationmod"),
            9 => ModSettings.GetString("Jester Start", "det.rolecustomizationmod"),
            10 => ModSettings.GetString("Pirate Start", "det.rolecustomizationmod"),
            11 => ModSettings.GetString("Doomsayer Start", "det.rolecustomizationmod"),
            12 => ModSettings.GetString("Vampire Start", "det.rolecustomizationmod"),
            13 => ModSettings.GetString("Cursed Soul Start", "det.rolecustomizationmod"),
            33 => ModSettings.GetString("Jackal/Recruit Start", "det.rolecustomizationmod"),
            34 => ModSettings.GetString("Frogs Start", "det.rolecustomizationmod"),
            35 => ModSettings.GetString("Lions Start", "det.rolecustomizationmod"),
            36 => ModSettings.GetString("Hawks Start", "det.rolecustomizationmod"),
            38 => ModSettings.GetString("Judge Start", "det.rolecustomizationmod"),
            39 => ModSettings.GetString("Auditor Start", "det.rolecustomizationmod"),
            40 => ModSettings.GetString("Inquisitor Start", "det.rolecustomizationmod"),
            41 => ModSettings.GetString("Starspawn Start", "det.rolecustomizationmod"),
            42 => ModSettings.GetString("Egotist Start", "det.rolecustomizationmod"),
            43 => ModSettings.GetString("Pandora Start", "det.rolecustomizationmod"),
            44 => ModSettings.GetString("Compliance Start", "det.rolecustomizationmod"),
            250 => ModSettings.GetString("Lovers", "det.rolecustomizationmod"),
            _ => ModSettings.GetString("Stoned/Hidden", "det.rolecustomizationmod"),
        };
    }
}

[HarmonyPatch(typeof(HomeSceneController), nameof(HomeSceneController.HandleClickPlay))]
public static class FixStyles
{
        [HarmonyPostfix]
        public static void RefreshStyles()
        {
            TMP_StyleSheet defaultStyleSheet = TMP_Settings.defaultStyleSheet;
            if (defaultStyleSheet == null) return;

            UpdateStyle("TownColor", "Town Start");
            UpdateStyle("CovenColor", "Coven Start");
            UpdateStyle("ApocalypseColor", "Apocalypse Start");
            UpdateStyle("SerialKillerColor", "Serial Killer Start");
            UpdateStyle("ArsonistColor", "Arsonist Start");
            UpdateStyle("WerewolfColor", "Werewolf Start");
            UpdateStyle("ShroudColor", "Shroud Start");
            UpdateStyle("ExecutionerColor", "Executioner Start");
            UpdateStyle("JesterColor", "Jester Start");
            UpdateStyle("PirateColor", "Pirate Start");
            UpdateStyle("DoomsayerColor", "Doomsayer Start");
            UpdateStyle("VampireColor", "Vampire Start");
            UpdateStyle("CursedSoulColor", "Cursed Soul Start");
            UpdateStyle("NeutralColor", "Neutral");

            defaultStyleSheet.RefreshStyles();
        }

        private static void UpdateStyle(string styleName, string modSettingKey)
        {
            TMP_Style style = TMP_Settings.defaultStyleSheet.GetStyle(styleName);
            if (style != null)
            {
                string colorValue = ModSettings.GetString(modSettingKey, "det.rolecustomizationmod");
                if (!string.IsNullOrEmpty(colorValue))
                {
                    string newOpeningDefinition = $"<color={colorValue}>";

                    // Use Reflection to modify the read-only field
                    FieldInfo fieldInfo = typeof(TMP_Style).GetField("m_OpeningDefinition", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(style, newOpeningDefinition);
                    }
                }
            }
        }
}

[HarmonyPatch(typeof(GameSceneController), nameof(GameSceneController.Start))]
public static class RefreshGame
{
    public static void Postfix() => FixStyles.RefreshStyles();
}

public static class GetChangedGradients
{
    public static Gradient GetChangedGradient(this FactionType faction)
    {
        var gradient = new Gradient();
        var array = new GradientColorKey[2];
        var array2 = new GradientAlphaKey[2];

        if (faction != (FactionType)13)
        {
            switch (faction)
            {
                case (FactionType)1:
                    array[0] = new(ModSettings.GetColor("Town Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Town End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)2:
                    array[0] = new(ModSettings.GetColor("Coven Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Coven End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)7:
                    array[0] = new(ModSettings.GetColor("Apocalypse Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Apocalypse End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)8:
                    array[0] = new(ModSettings.GetColor("Executioner Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Executioner End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)3:
                    array[0] = new(ModSettings.GetColor("Serial Killer Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Serial Killer End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)4:
                    array[0] = new(ModSettings.GetColor("Arsonist Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Arsonist End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)5:
                    array[0] = new(ModSettings.GetColor("Werewolf Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Werewolf End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)6:
                    array[0] = new(ModSettings.GetColor("Shroud Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Shroud End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)9:
                    array[0] = new(ModSettings.GetColor("Jester Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Jester End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)40:
                    array[0] = new(ModSettings.GetColor("Inquisitor Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Inquisitor End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)10:
                    array[0] = new(ModSettings.GetColor("Pirate Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Pirate End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)11:
                    array[0] = new(ModSettings.GetColor("Doomsayer Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Doomsayer End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)12:
                    array[0] = new(ModSettings.GetColor("Vampire Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Vampire End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)33:
                    array[0] = new(ModSettings.GetColor("Jackal/Recruit Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Jackal/Recruit End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)38:
                    array[0] = new(ModSettings.GetColor("Judge Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Judge End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)39:
                    array[0] = new(ModSettings.GetColor("Auditor Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Auditor End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)41:
                    array[0] = new(ModSettings.GetColor("Starspawn Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Starspawn End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)42:
                    array[0] = new(ModSettings.GetColor("Egotist Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Egotist End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)43:
                    array[0] = new(ModSettings.GetColor("Pandora Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Pandora End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)34:
                    array[0] = new(ModSettings.GetColor("Frogs Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Frogs End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)35:
                    array[0] = new(ModSettings.GetColor("Lions Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Lions End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)36:
                    array[0] = new(ModSettings.GetColor("Hawks Start", "det.rolecustomizationmod"), 0f);
                    array[1] = new(ModSettings.GetColor("Hawks End", "det.rolecustomizationmod"), 1f);
                    goto IL_259;

                case (FactionType)44:
                    array =
                    [
                        new(ModSettings.GetColor("Compliance Start", "det.rolecustomizationmod"), 0f),
                        new(ModSettings.GetColor("Compliance Middle", "det.rolecustomizationmod"), 0.5f),
                        new(ModSettings.GetColor("Compliance End", "det.rolecustomizationmod"), 1f)
                    ];
                    goto IL_259;

            }
            return null;
        }

        array[0] = new(ModSettings.GetColor("Cursed Soul Start", "det.rolecustomizationmod"), 0f);
        array[1] = new(ModSettings.GetColor("Cursed Soul End", "det.rolecustomizationmod"), 1f);

        IL_259:
            array2[0] = new(1f, 0f);
            array2[1] = new(1f, 1f);
            gradient.SetKeys(array, array2);

        return gradient;
    }
}

public class GradientRoleColorController : MonoBehaviour
{
    public RoleCardPanelBackground __instance;
    private readonly float duration = 10f;
    private float value = 0f;

    public void Start() => StartCoroutine(ChangeValueOverTime(__instance.currentFaction));

    public void OnDestroy() => StopCoroutine(ChangeValueOverTime(__instance.currentFaction));

    private IEnumerator ChangeValueOverTime(FactionType faction)
    {
        var grad = faction.GetChangedGradient();

        if (grad == null)
        {
            Destroy(this);
            yield break;
        }

        for (;;)
        {
            for (var t = 0f; t < duration; t += Time.deltaTime)
            {
                value = Mathf.Lerp(0f, 1f, t / duration);
                __instance.rolecardBackgroundInstance.SetColor(grad.Evaluate(value));
                yield return new WaitForEndOfFrame();
            }

            for (var t2 = 0f; t2 < duration; t2 += Time.deltaTime)
            {
                value = Mathf.Lerp(1f, 0f, t2 / duration);
                __instance.rolecardBackgroundInstance.SetColor(grad.Evaluate(value));
                yield return new WaitForEndOfFrame();
            }
        }
    }
}

[HarmonyPatch(typeof(MentionsProvider), nameof(MentionsProvider.ProcessSpeakerName))]
public static class PatchJudge
{
    public static void Postfix(string encodedText, int position, ref string __result)
    {
        if (Utils.IsBTOS2())
        {
            if (position == 70)
            {
                __result = "<link=\"r57\"><sprite=\"BTOSRoleIcons\" name=\"Role57\"><indent=1.1em><b>" + AddChangedConversionTags.ApplyGradient(ModSettings.GetString("Court Label",
                    "det.rolecustomizationmod"), ModSettings.GetColor("Judge Start", "det.rolecustomizationmod"), ModSettings.GetColor("Judge End", "det.rolecustomizationmod")) + ":" +
                    "</b> </link>" + encodedText.Replace("????: </color>", "").Replace("white", "#FFFF00");
            }
            else if (position == 69)
                __result = encodedText.Replace("????:", $"<sprite=\"BTOSRoleIcons\" name=\"Role16\"> {ModSettings.GetColor("Jury Label", "det.rolecustomizationmod")}:");
            else if (position == 71)
            {
                __result = "<link=\"r46\"><sprite=\"BTOSRoleIcons\" name=\"Role46\"><indent=1.1em><b>" + AddChangedConversionTags.ApplyGradient(ModSettings.GetString("Pirate Label",
                    "det.rolecustomizationmod"), ModSettings.GetColor("Pirate Start", "det.rolecustomizationmod"), ModSettings.GetColor("Pirate End",
                    "det.rolecustomizationmod")) + ":</b> </link>" + encodedText.Replace("????: </color>", "").Replace("white", "#ECC23E");
            }
        }
    }
}

[HarmonyPatch(typeof(FactionWinsCinematicPlayer), nameof(FactionWinsCinematicPlayer.Init))]
public static class PatchDefaultWinScreens
{
    public static void Postfix(FactionWinsCinematicPlayer __instance, ref ICinematicData cinematicData)
    {
        __instance.elapsedDuration = 0f;
        Debug.Log(string.Format("FactionWinsCinematicPlayer current phase at start = {0}", Pepper.GetGamePhase()));
        __instance.cinematicData = cinematicData as FactionWinsCinematicData;
        var winTimeByFaction = CinematicFactionWinsTimes.GetWinTimeByFaction(__instance.cinematicData.winningFaction);
        __instance.totalDuration = winTimeByFaction;
        __instance.callbackTimers.Clear();
        var spawnedCharacters = Service.Game.Cast.GetSpawnedCharacters();

        if (spawnedCharacters == null)
        {
            Debug.LogError("spawnedPlayers is null in GetCrowd()");
            return;
        }

        var positions = new HashSet<int>();
        __instance.cinematicData.entries.ForEach(e => positions.Add(e.position));
        spawnedCharacters.ForEach(c =>
        {
            if (positions.Contains(c.position))
                __instance.winningCharacters.Add(c);
            else
                c.characterSprite.SetColor(Color.clear);
        });
        var winningFaction = __instance.cinematicData.winningFaction;

        if (winningFaction == FactionType.TOWN)
        {
            Service.Home.AudioService.PlayMusic("Audio/Music/TownVictory.wav", false, AudioController.AudioChannel.Cinematic, true);
            __instance.evilProp.SetActive(false);
            __instance.goodProp.SetActive(true);
            __instance.m_Animator.SetInteger("State", 1);
        }
        else
        {
            Service.Home.AudioService.PlayMusic("Audio/Music/CovenVictory.wav", false, AudioController.AudioChannel.Cinematic, true);
            __instance.evilProp.SetActive(true);
            __instance.goodProp.SetActive(false);
            __instance.m_Animator.SetInteger("State", 2);
        }

        var text = string.Format("GUI_WINNERS_ARE_{0}", (int)winningFaction);
        var text2 = __instance.l10n(text);
        string gradientText;

        if (winningFaction.GetChangedGradient() != null)
        {
            var gradient = winningFaction.GetChangedGradient();
            __instance.leftImage.color = Utils.GetFactionStartingColor(winningFaction);
            __instance.rightImage.color = Utils.GetFactionEndingColor(winningFaction);

            if (winningFaction == (FactionType)44)
                gradientText = AddChangedConversionTags.ApplyThreeColorGradient(text2, gradient.Evaluate(0f), gradient.Evaluate(0.5f), gradient.Evaluate(1f));
            else
                gradientText = AddChangedConversionTags.ApplyGradient(text2, gradient.Evaluate(0f), gradient.Evaluate(1f));

            __instance.textAnimatorPlayer.ShowText(gradientText);
        }
        else
        {
            if (ColorUtility.TryParseHtmlString(winningFaction.GetFactionColor(), out Color color))
            {
                __instance.leftImage.color = color;
                __instance.rightImage.color = color;
                __instance.glow.color = color;
            }

            __instance.text.color = color;
            __instance.textAnimatorPlayer.ShowText(text2);
        }

        __instance.SetUpWinners(__instance.winningCharacters);
        return;
    }
}

[HarmonyPatch(typeof(FactionWinsStandardCinematicPlayer), nameof(FactionWinsStandardCinematicPlayer.Init))]
public static class PatchCustomWinScreens
{
    public static void Postfix(FactionWinsStandardCinematicPlayer __instance, ref ICinematicData cinematicData)
    {
        Debug.Log(string.Format("FactionWinsStandardCinematicPlayer current phase at end = {0}", Pepper.GetGamePhase()));
        __instance.elapsedDuration = 0f;
        __instance.cinematicData = cinematicData as FactionWinsCinematicData;
        var num = CinematicFactionWinsTimes.GetWinTimeByFaction(__instance.cinematicData.winningFaction);
        __instance.totalDuration = num;

        if (Pepper.IsResultsPhase())
            num += 0.2f;

        var winningFaction = __instance.cinematicData.winningFaction;

        if (winningFaction == FactionType.TOWN)
            Service.Home.AudioService.PlayMusic("Audio/Music/TownVictory.wav", false, AudioController.AudioChannel.Cinematic, true);
        else if (winningFaction is FactionType.COVEN or FactionType.NONE)
            Service.Home.AudioService.PlayMusic("Audio/Music/CovenVictory.wav", false, AudioController.AudioChannel.Cinematic, true);

        var text2 = __instance.l10n(string.Format("GUI_WINNERS_ARE_{0}", (int)winningFaction));
        string gradientText;

        if (winningFaction.GetChangedGradient() != null)
        {
            Gradient gradient = winningFaction.GetChangedGradient();

            if (winningFaction == (FactionType)44)
                gradientText = AddChangedConversionTags.ApplyThreeColorGradient(text2, gradient.Evaluate(0f), gradient.Evaluate(0.5f), gradient.Evaluate(1f));
            else
                gradientText = AddChangedConversionTags.ApplyGradient(text2, gradient.Evaluate(0f), gradient.Evaluate(1f));

            if (__instance.textAnimatorPlayer.gameObject.activeSelf)
                __instance.textAnimatorPlayer.ShowText(gradientText);
        }
        else if (__instance.textAnimatorPlayer.gameObject.activeSelf)
            __instance.textAnimatorPlayer.ShowText(text2);

        __instance.SetUpWinners();
        return;
    }
}

[HarmonyPatch(typeof(TosCharacterNametag), nameof(TosCharacterNametag.ColouredName))]
public static class TosCharacterNametagPatch2
{
    public static void Postfix(TosCharacterNametag __instance, ref string theName, ref FactionType factionType, ref Role role, ref string __result)
    {
        var roleName = ModSettings.GetBool("Faction-Specific Role Names") ? Utils.ToRoleFactionDisplayString(role, factionType) : role.ToDisplayString();

        if (factionType.GetChangedGradient() != null && role is not (Role.STONED or Role.HIDDEN))
        {
            var gradient = factionType.GetChangedGradient();
            var gradientName = "";
            var gradientRole = "";

            if (factionType is ((FactionType)44) and not ((FactionType)33))
            {
                gradientName = AddChangedConversionTags.ApplyThreeColorGradient(theName, gradient.Evaluate(0f), gradient.Evaluate(0.5f), gradient.Evaluate(1f));
                gradientRole = AddChangedConversionTags.ApplyThreeColorGradient("(" + roleName + ")", gradient.Evaluate(0f), gradient.Evaluate(0.5f), gradient.Evaluate(1f));
            }
            else if (factionType == (FactionType)33 && role != RolePlus.JACKAL)
            {
                Gradient jackalGradient = FactionTypePlus.JACKAL.GetChangedGradient();
                gradientName = AddChangedConversionTags.ApplyGradient(theName, jackalGradient.Evaluate(0f), jackalGradient.Evaluate(1f));
                gradientRole = AddChangedConversionTags.ApplyGradient("(" + roleName + ")", gradient.Evaluate(0f), jackalGradient.Evaluate(1f));
            }
            else
            {
                gradientName = AddChangedConversionTags.ApplyGradient(theName, gradient.Evaluate(0f), gradient.Evaluate(1f));
                gradientRole = AddChangedConversionTags.ApplyGradient("(" + roleName + ")", gradient.Evaluate(0f), gradient.Evaluate(1f));
            }

            if (Utils.IsBTOS2())
                __result = $"<size=36><sprite=\"BTOSRoleIcons\" name=\"Role{(int)role}\"></size>\n<size=24>{gradientName}</size>\n<size=18>{gradientRole}</size>";
            else
                __result = $"<size=36><sprite=\"RoleIcons\" name=\"Role{(int)role}\"></size>\n<size=24>{gradientName}</size>\n<size=18>{gradientRole}</size>";
        }
    }
}

[HarmonyPatch(typeof(RoleCardPanel), nameof(RoleCardPanel.UpdateTitle))]
public static class PatchRoleCard
{
    public static void Postfix(RoleCardPanel __instance)
    {
        var component = __instance.GetComponent<GradientRoleColorController>();

        if (component != null)
            UObject.Destroy(component);

        __instance.gameObject.AddComponent<GradientRoleColorController>().__instance = __instance.rolecardBG;
        __instance.roleNameText.text = Pepper.GetMyRole().ToChangedDisplayString(Pepper.GetMyFaction(), Service.Game.Sim.simulation.observations.roleCardObservation.Data.modifier);
    }

    public static string ToChangedDisplayString(this Role role, FactionType faction, ROLE_MODIFIER modifier)
    {
        var text = "";
        var roleName = ModSettings.GetBool("Faction-Specific Role Names") ? Utils.ToRoleFactionDisplayString(role, faction) : role.ToDisplayString();

        if (faction.GetChangedGradient() != null)
            text = AddChangedConversionTags.ApplyGradient(roleName, faction.GetChangedGradient());
        else
        {
            text = string.Concat(
            [
                "<color=",
                ClientRoleExtensions.GetFactionColor(faction),
                ">",
                roleName,
                "</color>"
            ]);
        }

        var gradientTT = faction.GetChangedGradient();

        if (modifier == (ROLE_MODIFIER)2 && gradientTT != null)
        {
            text = text + "\n<size=85%>" + AddChangedConversionTags.ApplyGradient($"({ModSettings.GetString("Town Traitor Label", "det.rolecustomizationmod")})", gradientTT.Evaluate(0f), gradientTT.Evaluate(1f)) + "</size>";
        }
        else if (modifier == (ROLE_MODIFIER)10)
        {
            var gradient = ((FactionType)33).GetChangedGradient();
            text = text + "\n<size=85%>" + AddChangedConversionTags.ApplyGradient($"({ModSettings.GetString("Recruit Label", "det.rolecustomizationmod")})", gradient.Evaluate(0f), gradient.Evaluate(1f)) + "</size>";
        }
        else if (RoleExtensions.GetFaction(role) != faction)
        {
            var gradient2 = faction.GetChangedGradient();

            if (gradient2 != null)
            {
                if (faction == (FactionType)44)
                {
                    text = text + "\n<size=85%>" + AddChangedConversionTags.ApplyThreeColorGradient("(" + faction.ToDisplayString() + ")", gradient2.Evaluate(0f), gradient2.Evaluate(0.5f),
                        gradient2.Evaluate(1f)) + "</size>";
                }
                else
                    text = text + "\n<size=85%>" + AddChangedConversionTags.ApplyGradient("(" + faction.ToDisplayString() + ")", gradient2.Evaluate(0f), gradient2.Evaluate(1f)) + "</size>";

                if (modifier == (ROLE_MODIFIER)1)
                {
                    text = text + "\n<size=85%>" + AddChangedConversionTags.ApplyGradient($"({ModSettings.GetString("VIP Label", "det.rolecustomizationmod")})", gradientTT.Evaluate(0f), gradientTT.Evaluate(1f)) + "</size>";
                }

            }
            else
            {
                text = string.Concat(
                [
                    text,
                    "\n<size=85%><color=",
                    ClientRoleExtensions.GetFactionColor(faction),
                    ">(",
                    faction.ToDisplayString(),
                    ")</color></size>"
                ]);
            }
        }

        return text;
    }
}

[HarmonyPatch(typeof(RoleCardPopupPanel), nameof(RoleCardPopupPanel.SetRole))]
public static class RoleCardPopupPatches
{
    public static void Postfix(ref Role role, RoleCardPopupPanel __instance) => __instance.roleNameText.text = ClientRoleExtensions.ToColorizedDisplayString(role);
}

[HarmonyPatch(typeof(TosAbilityPanelListItem), nameof(TosAbilityPanelListItem.SetKnownRole))]
public static class PlayerListPatch
{
    public static bool Prefix(ref Role role, ref FactionType faction, TosAbilityPanelListItem __instance)
    {
        __instance.playerRole = role;
        var roleName = ModSettings.GetBool("Faction-Specific Role Names") ? Utils.ToRoleFactionDisplayString(role, faction) : role.ToDisplayString();
        var factionName = faction.ToDisplayString();

        if (role is not (0 or (Role)byte.MaxValue))
        {
            var gradient = faction.GetChangedGradient();

            if (gradient != null && role is not ((Role)254 or (Role)241))
            {
                if (faction is ((FactionType)44) and not ((FactionType)33))
                {
                    __instance.playerRoleText.text = AddChangedConversionTags.ApplyThreeColorGradient("(" + roleName + ")", gradient.Evaluate(0f), gradient.Evaluate(0.5f),
                        gradient.Evaluate(1f));
                }
                else if (faction == (FactionType)33 && role != BetterTOS2.RolePlus.JACKAL)
                {
                    Gradient jackalGradient = FactionTypePlus.JACKAL.GetChangedGradient();

                    __instance.playerRoleText.text = AddChangedConversionTags.ApplyGradient("(" + roleName + ")", jackalGradient.Evaluate(0f), jackalGradient.Evaluate(1f));
                }
                else
                    __instance.playerRoleText.text = AddChangedConversionTags.ApplyGradient("(" + roleName + ")", gradient.Evaluate(0f), gradient.Evaluate(1f));

            }
            else if (role is not ((Role)254 or (Role)241))
            {
                __instance.playerRoleText.text = string.Concat(
                [
                    "<color=",
                    ClientRoleExtensions.GetFactionColor(faction),
                    ">(",
                    roleName,
                    ")</color>"
                ]);
            }
            else
            {
                __instance.playerRoleText.text = string.Concat(
                [
                    "<color=",
                    ClientRoleExtensions.GetFactionColor(RoleExtensions.GetFaction(role)),
                    ">(",
                    roleName,
                    ")</color>"
                ]);
            }

            __instance.playerRoleText.gameObject.SetActive(true);
        }

        return false;
    }
}