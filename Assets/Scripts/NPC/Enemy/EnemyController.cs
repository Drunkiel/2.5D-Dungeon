using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    public bool isStopped;
    public EntityStatistics _statistics;
    public HoldingController _holdingController;

    public Vector2 movement;
    public bool isMoving;

    [SerializeField] private Rigidbody rgBody;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float wanderingDistance;
    [SerializeField] private Vector3 positionToGo;

    private bool isFlipped;
    private bool isNewPositionFound;

    private void Start()
    {
        startPosition = transform.position;
        SetNewPosition();
    }

    private void Update()
    {
        isMoving = movement.magnitude > 0.01f;

        if (isStopped)
            return;

        if (Vector3.Distance(transform.position, positionToGo) < 0.2f)
        {
            if (!isNewPositionFound)
            {
                StartCoroutine(PauseBeforeNewPosition());
                isNewPositionFound = true;
            }
            else
                movement = Vector2.zero;
        }
        else
            GoTo(positionToGo);

        //Controls max speed
        if (rgBody.velocity.magnitude > _statistics.maxSpeed)
            rgBody.velocity = Vector3.ClampMagnitude(rgBody.velocity, _statistics.maxSpeed);
    }

    private void FixedUpdate()
    {
        if (transform.position.y < -10f)
        {
            print($"Enemy fall off the map {transform.name} on position: {transform.position}");
            Destroy(gameObject);
        }

        if (isStopped)
            return;

        Vector3 move = new Vector3(movement.x, 0, movement.y).normalized;
        rgBody.AddForce(move * (_statistics.speedForce * rgBody.mass), ForceMode.Force);
    }

    public void RecalculateStatistics()
    {
        _statistics.ResetStatistics();
        GearHolder _gearHolder = _holdingController._itemController._gearHolder;

        //Getting weapons
        WeaponItem _weaponLeft = _gearHolder.GetHoldingWeapon(WeaponHoldingType.Left_Hand);
        WeaponItem _weaponRight = _gearHolder.GetHoldingWeapon(WeaponHoldingType.Right_Hand);
        WeaponItem _weaponBoth = _gearHolder.GetHoldingWeapon(WeaponHoldingType.Both_Hands);

        //Getting armor
        ArmorItem _armorHead = _gearHolder.GetHoldingArmor(ArmorType.Helmet);
        ArmorItem _armorChestplate = _gearHolder.GetHoldingArmor(ArmorType.Chestplate);
        ArmorItem _armorBoots = _gearHolder.GetHoldingArmor(ArmorType.Boots);

        List<Attributes> _allAttributes = new();

        //Checks if any variable is empty
        if (_weaponLeft != null)
            _allAttributes.AddRange(_weaponLeft._itemData._attributes);

        if (_weaponRight != null)
            _allAttributes.AddRange(_weaponRight._itemData._attributes);

        if (_weaponBoth != null)
            _allAttributes.AddRange(_weaponBoth._itemData._attributes);

        if (_armorHead != null)
            _allAttributes.AddRange(_armorHead._itemData._attributes);

        if (_armorChestplate != null)
            _allAttributes.AddRange(_armorChestplate._itemData._attributes);

        if (_armorBoots != null)
            _allAttributes.AddRange(_armorBoots._itemData._attributes);

        //Setting all data
        for (int i = 0; i < _allAttributes.Count; i++)
        {
            switch (_allAttributes[i].attributeType)
            {
                case AttributeTypes.MeleeDamage:
                    _statistics.meleeDamage += _allAttributes[i].amount;
                    break;

                case AttributeTypes.RangeDamage:
                    _statistics.rangeDamage += _allAttributes[i].amount;
                    break;

                case AttributeTypes.MagicDamage:
                    _statistics.magicDamage += _allAttributes[i].amount;
                    break;

                case AttributeTypes.AllProtection:
                    _statistics.allProtection += _allAttributes[i].amount;
                    break;

                case AttributeTypes.MeleeProtection:
                    _statistics.meleeProtection += _allAttributes[i].amount;
                    break;

                case AttributeTypes.RangeProtection:
                    _statistics.rangeProtection += _allAttributes[i].amount;
                    break;

                case AttributeTypes.MagicProtection:
                    _statistics.magicProtection += _allAttributes[i].amount;
                    break;
            }
        }
    }

    private void SetNewPosition()
    {
        positionToGo = GetRandomPosition();
    }

    private IEnumerator PauseBeforeNewPosition()
    {
        yield return new WaitForSeconds(Random.Range(2, 5));
        isNewPositionFound = false;
        SetNewPosition();
    }

    private void GoTo(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        movement = new Vector2(
            direction.x * _statistics.speedForce,
            direction.z * _statistics.speedForce
        );

        //Flipping Enemy to direction they are going
        if (movement.x < 0 && !isFlipped)
        {
            transform.GetChild(0).localScale = new(-1, 1, 1);
            isFlipped = true;
        }
        else if (movement.x > 0 && isFlipped)
        {
            transform.GetChild(0).localScale = new(1, 1, 1);
            isFlipped = false;
        }
    }

    private Vector3 GetRandomPosition()
    {
        //Getting random position on X and Z axis
        float deltaX = Random.Range(-wanderingDistance, wanderingDistance);
        float deltaZ = Random.Range(-wanderingDistance, wanderingDistance);

        //New position
        Vector3 newPosition = new(startPosition.x + deltaX, startPosition.y, startPosition.z + deltaZ);
        return newPosition;
    }
}
