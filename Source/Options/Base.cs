namespace FancyUI.Options;

public abstract class Option
{
    public static readonly List<Option> All = [];
    protected string ID { get; }
    public PackType Page { get; }

    public virtual Setting BoxedSetting { get; set; }

    protected Option(string id, PackType page)
    {
        ID = id;
        Page = page;
        All.Add(this);
    }

    public abstract void Set(string value);
}