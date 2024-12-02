namespace FancyUI.Options;

public abstract class Option<TValue, TSetting>(string id, TValue defaultValue, OptionType type, Func<TValue, bool> setActive = null, Action<TValue> onChanged = null) : Option(id, type,
    defaultValue.ToString()) where TSetting : Setting
{
    public Config<TValue> Entry { get; } = Fancy.Instance.Configs.Bind(id, defaultValue);
    public Func<TValue, bool> SetActive { get; } = setActive ?? (_ => true);
    public Action<TValue> OnChanged { get; } = onChanged ?? (_ => {});
    public TValue DefaultValue { get; } = defaultValue;

    private TSetting _setting;
    public TSetting Setting
    {
        get => _setting;
        set
        {
            _setting = value;
            OptionCreated();
        }
    }

    public TValue Get() => Entry.Value;

    public void Set(TValue value) => Entry.Value = value;

    public virtual void OptionCreated()
    {
        Setting.name = ID;
        Setting.TitleText.SetText(SettingsAndTestingUI.Instance.l10n($"FANCY_{ID}_NAME"));
        Setting.Background.EnsureComponent<TooltipTrigger>().LookupKey = $"FANCY_{ID}_DESC";
    }
}