using UnityEngine;

public interface IMonoDecorator<T> where T : MonoBehaviour
{
    T Decorate(T mono);
}
