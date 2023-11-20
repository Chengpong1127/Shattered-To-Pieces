using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoopingBackground : MonoBehaviour
{

    [SerializeField] private RawImage _img;
    [SerializeField] private float _x,_y;
    // Update is called once per frame
    private void Start()
    {
        StartCoroutine(Looping());
    }
    public IEnumerator Looping()
    {
        _img.uvRect=new Rect(_img.uvRect.position+new Vector2(_x,_y)*Time.deltaTime,_img.uvRect.size);
        yield return null;
    }
}
