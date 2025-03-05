using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float force;
    public bool useGravity;
    public string entityTag;
    private SkillDataParser _skillDataParser;
    [SerializeField] private CollisionController _collisionController;
    private EntityStatistics _casterStatistics;
    private CombatUI _combatUI;
    private bool disable;

    public void Shoot()
    {
        GameObject skillCopyObject = Instantiate(gameObject, transform.position, transform.rotation, null);
        Rigidbody skillCopyRgBody = skillCopyObject.GetComponent<Rigidbody>();
        skillCopyObject.transform.GetChild(0).gameObject.SetActive(true);
        skillCopyRgBody.useGravity = useGravity;
        skillCopyRgBody.constraints = RigidbodyConstraints.FreezeRotation;
        skillCopyRgBody.AddForce(transform.forward * force);
    }

    public void SetData(SkillDataParser _skillDataParser, EntityStatistics _casterStatistics, CombatUI _combatUI, string entityTag)
    {
        this._skillDataParser = _skillDataParser;
        this._casterStatistics = _casterStatistics;
        this._combatUI = _combatUI;
        this.entityTag = entityTag;
        _collisionController.entityTag = this.entityTag;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(entityTag) && !disable)
            StartCoroutine(TryAttackUntilSuccess());
    }

    private IEnumerator TryAttackUntilSuccess()
    {
        disable = true; 
        while (!CombatController.instance.AttackSkill(_skillDataParser, _collisionController, _casterStatistics, _combatUI))
            yield return null;

        Destroy(gameObject);
    }
}
