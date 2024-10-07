using System.Diagnostics;

namespace FancyUI.UI;

public abstract class BaseUI : UIController
{
    public const string REPO = "https://raw.githubusercontent.com/AlchlcDvl/RoleIconRecolors/main";
    public static readonly Dictionary<string, bool> Running = [];
    public static bool HandlerRunning;

    private GameObject Back;
    private GameObject OpenDir;
    private GameObject Confirm;
    private TMP_InputField PackName;
    private TMP_InputField RepoName;
    private TMP_InputField RepoOwner;
    private TMP_InputField BranchName;
    public GameObject NoPacks;
    public GameObject PackTemplate;

    public bool Abort { get; set; }

    public abstract string Type { get; }
    public abstract string Path { get; }

    public readonly List<GameObject> PackGOs = [];

    public virtual void Awake()
    {
        Back = transform.Find("Buttons/Back").gameObject;
        OpenDir = transform.Find("Buttons/Directory").gameObject;
        Confirm = transform.Find("Inputs/Confirm").gameObject;
        PackName = transform.Find("Inputs/PackName").gameObject.GetComponent<TMP_InputField>();
        RepoName = transform.Find("Inputs/RepoName").gameObject.GetComponent<TMP_InputField>();
        RepoOwner = transform.Find("Inputs/RepoOwner").gameObject.GetComponent<TMP_InputField>();
        BranchName = transform.Find("Inputs/BranchName").gameObject.GetComponent<TMP_InputField>();
        NoPacks = transform.Find("ScrollView/NoPacks").gameObject;
        PackTemplate = transform.Find("ScrollView/Viewport/Content/PackTemplate").gameObject;
    }

    public virtual void SetupMenu()
    {
        Back.GetComponent<Button>().onClick.AddListener(GoBack);
        Back.AddComponent<TooltipTrigger>().NonLocalizedString = "Close Packs Menu";

        OpenDir.GetComponent<Button>().onClick.AddListener(OpenDirectory);
        OpenDir.AddComponent<TooltipTrigger>().NonLocalizedString = "Open Icons Folder";

        var dirButton = OpenDir.AddComponent<HoverEffect>();
        dirButton.OnMouseOver.AddListener(() => OpenDir.GetComponent<Image>().sprite = AssetManager.Assets["OpenChest"]);
        dirButton.OnMouseOut.AddListener(() => OpenDir.GetComponent<Image>().sprite = AssetManager.Assets["ClosedChest"]);

        Confirm.GetComponent<Button>().onClick.AddListener(AfterGenerating);
        Confirm.AddComponent<TooltipTrigger>().NonLocalizedString = "Confirm Link Parameters And Generate Link";

        PackName.AddComponent<TooltipTrigger>().NonLocalizedString = $"Name Of The {Type} (REQUIRED)";
        RepoName.AddComponent<TooltipTrigger>().NonLocalizedString = $"Name Of The {Type} GitHub Repository (Defaults To: RoleIconRecolors)";
        RepoOwner.AddComponent<TooltipTrigger>().NonLocalizedString = $"Name Of The {Type} GitHub Repository Owner (Defaults To: AlchlcDvl)";
        BranchName.AddComponent<TooltipTrigger>().NonLocalizedString = $"Name Of The {Type} GitHub Repository Branch It Is In (Defaults To: main)";
    }

    public void GoBack()
    {
        gameObject.SetActive(false);
        FancyUI.Instance.gameObject.SetActive(true);
    }

    public abstract void AfterGenerating();

    public PackJson GenerateLinkAndAddToPackCount()
    {
        var name = PackName.text;

        if (StringUtils.IsNullEmptyOrWhiteSpace(name))
        {
            Logging.LogError("Tried to generate pack link with no pack name");
            return null;
        }

        var packJson = new PackJson()
        {
            Name = name,
            RepoName = RepoName.text,
            RepoOwner = RepoOwner.text,
            Branch = BranchName.text,
        };
        packJson.SetDefaults();
        return packJson;
    }

    public void OpenDirectory()
    {
        // code stolen from jan who stole from tuba
        if (Environment.OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
            Process.Start("open", $"\"{Path}\"");
        else
            Application.OpenURL($"file://{Path}");
    }
}