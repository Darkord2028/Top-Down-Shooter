using UnityEngine;
using UnityEngine.Pool;

public class WorldObjectPoolManager : MonoBehaviour
{
    public static WorldObjectPoolManager instance = null;

    //Object Pooling
    public ObjectPool<TrailRenderer> bulletTrailPool { get; private set; }

    //Pool Holder
    private GameObject bulletTrailHolder;

    #region Unity Callback Funtions

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (bulletTrailHolder != this)
        {
            Destroy(instance);
        }

        bulletTrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        bulletTrailHolder = new GameObject("Bullet Trail Pool");

    }

    private void Start()
    {
        
    }

    #endregion

    #region Object Pool Create Functions

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        instance.transform.SetParent(bulletTrailHolder.transform, true);
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        
        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    #endregion

}
