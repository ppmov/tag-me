using System.Collections;
using UnityEngine;

public class InvisibleAbility : Ability
{
    private SpriteRenderer[] renderers;
    private Material initialMaterial;

    public InvisibleAbility(Unit parent) : base(parent)
    {
        renderers = parent.GetComponentsInChildren<SpriteRenderer>(true);
        initialMaterial = renderers[0].material;
    }

    protected override void Enable()
    {
        foreach (SpriteRenderer renderer in renderers)
            if (renderer.gameObject.CompareTag("Skin"))
                renderer.material = Settings.InvisibleMaterial;
            else
            if (Parent != Game.Party?.Myself?.Unit)
                renderer.material = Settings.InvisibleMaterial;
    }

    protected override void Disable()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material = initialMaterial;

        Parent.StartCoroutine(WaitToBlock());
    }

    private IEnumerator WaitToBlock()
    {
        yield return new WaitUntil(() => !IsCooling);
        IsCooling = true;
    }
}
