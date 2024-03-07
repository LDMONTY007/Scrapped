using UnityEngine;

/// <summary>
/// Interactible interface for use with the player.
/// </summary>
public interface IInteractible
{
    void OnFocus(PlayerController p);

    void OnLostFocus(PlayerController p);

    void OnInteract(PlayerController p);
}
