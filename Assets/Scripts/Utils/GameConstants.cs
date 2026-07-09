using UnityEngine;

public static class Tags
{
    public const string Player = "Player";
    public const string Zombie = "Zombie";
    public const string ItemDrop = "ItemDrop";
    public const string Bullet = "Bullet";
    public const string Wall = "Wall";
}

public static class AnimParams
{
    public static readonly int Die = Animator.StringToHash("Die");
    public static readonly int Attack = Animator.StringToHash("Attack");
    public static readonly int Reload = Animator.StringToHash("Reload");
    public static readonly int Speed = Animator.StringToHash("Speed");
    public static readonly int ReloadSpeed = Animator.StringToHash("ReloadSpeed");
    public static readonly int IsWalking = Animator.StringToHash("IsWalking");
    public static readonly int DirX = Animator.StringToHash("DirX");
    public static readonly int DirY = Animator.StringToHash("DirY");
}