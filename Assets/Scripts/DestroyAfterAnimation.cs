using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    void Start()
    {
        //Aqui acessamos o animator do objeto para descobrir qual a animação está tocando no momento e sua duração(length)
        //O segundo parâmetro passado no destroy é uma duração e com isso conseguimos destruí-la depois dela ter terminado de "tocar"
        Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
    }
}
