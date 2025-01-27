using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] gunStats gun;

    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        IPickup pick = other.GetComponent<IPickup>();

        if (pick != null)
        {
            //transfer the gun to the object that entered trigger
            pick.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
