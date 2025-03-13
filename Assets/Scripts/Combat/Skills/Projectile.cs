using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float force;
    public bool useGravity;
    public string[] entityTags;
    public float destroyDelay = 1;
    [SerializeField] private SkillDataParser _skillDataParser;
    [SerializeField] private CollisionController _parentCollisionController;
    [SerializeField] private CollisionController _collisionController;
    [SerializeField] private Transform casterTransform;
    [SerializeField] private EntityStatistics _casterStatistics;
    [SerializeField] private CombatUI _combatUI;
    private bool disable = true;

    public void Shoot()
    {
        GameObject skillCopyObject = Instantiate(gameObject, transform.position, transform.rotation, null);
        Rigidbody skillCopyRgBody = skillCopyObject.GetComponent<Rigidbody>();
        skillCopyObject.transform.GetChild(0).gameObject.SetActive(true);
        skillCopyRgBody.useGravity = useGravity;
        skillCopyRgBody.constraints = RigidbodyConstraints.FreezeRotation;
        Vector3 randomOffset = new(
            Random.Range(-0.05f, 0.05f),
            Random.Range(-0.05f, 0.05f),
            Random.Range(-0.05f, 0.05f)
        );
        skillCopyRgBody.AddForce(entityTags[0][0] != 'P' ? transform.forward * force : (-(transform.position - PlayerController.instance.transform.position).normalized + randomOffset).normalized * force);
        skillCopyObject.GetComponent<Projectile>().disable = false;
        Destroy(skillCopyObject, destroyDelay);
    }

    public void SetData(SkillDataParser _skillDataParser, EntityStatistics _casterStatistics, Transform casterTransform, CombatUI _combatUI, string[] entityTags)
    {
        this._skillDataParser = _skillDataParser;
        this._casterStatistics = _casterStatistics;
        this._combatUI = _combatUI;
        this.casterTransform = casterTransform;
        this.entityTags = entityTags;
        _collisionController.entityTags = this.entityTags;
    }

    void OnTriggerEnter(Collider collider)
    {
        foreach (string entityTag in entityTags)
        {
            if (string.IsNullOrEmpty(entityTag))
                return;

            if (collider.CompareTag(entityTag) && !disable)
                StartCoroutine(TryAttackUntilSuccess());
        }
    }

    private IEnumerator TryAttackUntilSuccess()
    {
        disable = true;
        while (!CombatController.instance.AttackSkill(_skillDataParser, _collisionController, casterTransform, _casterStatistics, _combatUI))
            yield return null;

        Destroy(gameObject);
    }
}
