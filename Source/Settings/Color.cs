namespace FancyUI.Settings;

public class ColorSetting : BaseInputSetting
{
    public Image ValueBG { get; set; }
    public ColorOption Option { get; set; }

    public override void Awake()
    {
        base.Awake();
        ValueBG = Input.transform.GetComponent<Image>();
    }

    public void Start()
    {
        if (Option == null)
            return;

        Input.SetTextWithoutNotify(Option.Value);
        Input.restoreOriginalTextOnEscape = true;
        Input.onValueChanged.AddListener(OnValueChanged);
        Input.onValueChanged.AddListener(_ => SettingsAndTestingUI.Instance.RefreshOptions());

        ValueBG.color = Option.Value.ToColor();
    }

    public void OnValueChanged(string value)
    {
        var cache = value;

        if (!value.StartsWith("#"))
            value = "#" + value;

        if (ColorUtility.TryParseHtmlString(value, out var color) && value.Length is 7 or 9)
            ValueBG.color = color;

        if (cache != value)
            Input.SetTextWithoutNotify(value);

        Option.Value = value;
    }

    public override bool SetActive() => Option.SetActive(Option.Value) && Option.Page == SettingsAndTestingUI.Instance.Page;
}