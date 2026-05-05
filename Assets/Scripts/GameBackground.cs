using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBackground : MonoBehaviour
{
    public float speed;
    private SpriteRenderer _spriteRenderer;
    public List<Sprite> sprites;
    public static Sprite lastSprite;

    private float _originalX;
    private float _moveWidth;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        List<Sprite> options = sprites.Where(s => s != lastSprite).ToList();
        _spriteRenderer.sprite = Utils.GetRandomItem(options);
        lastSprite = _spriteRenderer.sprite;

        _originalX = transform.position.x;
        _moveWidth = _spriteRenderer.sprite.bounds.size.x / 2f;
    }

    void Update ()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (speed > 0 && transform.position.x < (_originalX - _moveWidth))
        {
            transform.position += Vector3.right * _moveWidth;
        }
        else if (speed < 0 && transform.position.x > (_originalX + _moveWidth))
        {
            transform.position += Vector3.left * _moveWidth;
        }
    }
}