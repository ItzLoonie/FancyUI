namespace FancyUI.ColouredWood;

public class GradientController : MonoBehaviour
{
    public RoleCardPanel RoleCard { get; set; }

    public void Start()
    {
        RoleCard = gameObject.GetComponent<RoleCardPanel>();
    }
}