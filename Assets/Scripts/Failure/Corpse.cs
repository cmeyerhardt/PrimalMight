using UnityEngine;

public class Corpse : MonoBehaviour
{
    [SerializeField] public CorpseTypeEnum corpseType = CorpseTypeEnum.Flesh;
    [SerializeField] public CorpseAction interactAction = CorpseAction.Nothing;
    [SerializeField] public Item item = null;
    [SerializeField] public Transform displayRoot = null;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void Decouple()
    {
        displayRoot.parent = null;
        DestroySelf();
    }
}
