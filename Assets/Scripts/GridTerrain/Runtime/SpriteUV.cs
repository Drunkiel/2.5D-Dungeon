using UnityEngine;

public static class SpriteUV
{
    public static void Get(Sprite sprite, out Vector2 min, out Vector2 max)
    {
        Rect r = sprite.textureRect;
        Texture t = sprite.texture;

        min = new Vector2(
            r.xMin / t.width,
            r.yMin / t.height
        );

        max = new Vector2(
            r.xMax / t.width,
            r.yMax / t.height
        );
    }
}
