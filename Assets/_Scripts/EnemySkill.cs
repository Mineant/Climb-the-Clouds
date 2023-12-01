using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class EnemySkill : MonoBehaviour
{
    public enum EnemySkillWeaponType { Prefab, Scene }
    public string ID;
    public float MaxCooldown;
    public float CurrentCooldown;
    public GameObject PrepAIObject;
    public GameObject CanUseAIObject;
    public EnemySkillWeaponType WeaponType;
    public BaseWeapon SkillWeapon;

    public Action OnFinishedUsingSkill;
    public bool UsingSkill { get; protected set; }
    protected Character _character;
    private TopDownController _controller;

    protected BaseWeapon _weaponInstance;
    protected WeaponAim _weaponAim;
    protected BaseWeaponHandler _baseWeaponHandler;
    protected AIAction[] _prepActions;
    protected AIDecision[] _prepDecisions;
    protected AIDecision[] _canUseDecisions;
    protected AIBrain _brain;
    protected CharacterMovement _movement;
    private CharacterOrientation3D _orientation;


    void Awake()
    {
        _character = GetComponentInParent<Character>();
        _movement = GetComponentInParent<CharacterMovement>();
        _orientation = GetComponentInParent<CharacterOrientation3D>();
        _controller = GetComponentInParent<TopDownController>();
        _baseWeaponHandler = _character.GetComponentInChildren<BaseWeaponHandler>();
        UsingSkill = false;
        _prepActions = PrepAIObject.GetComponentsInChildren<AIAction>();
        _prepDecisions = PrepAIObject.GetComponentsInChildren<AIDecision>();
        _canUseDecisions = CanUseAIObject.GetComponentsInChildren<AIDecision>();
        _brain = GetComponentInParent<AIBrain>();
    }

    void Start()
    {
        if (WeaponType == EnemySkillWeaponType.Prefab)
            _weaponInstance = _baseWeaponHandler.AddWeapon(SkillWeapon);
        else
            _weaponInstance = _baseWeaponHandler.AddSceneWeapon(SkillWeapon);

        _weaponAim = _weaponInstance.GetComponent<WeaponAim>();
        _weaponAim.AimControl = WeaponAim.AimControls.Script;
    }

    void Update()
    {
        if (CurrentCooldown > 0f) CurrentCooldown -= Time.deltaTime;
    }

    public bool CanUseSkill()
    {
        return !UsingSkill && CurrentCooldown <= 0f && _canUseDecisions.All(d => d.Decide());
    }

    public void UseSkill()
    {
        StartCoroutine(_UseSkillCoroutine());
    }

    private IEnumerator _UseSkillCoroutine()
    {
        UsingSkill = true;

        while (true)
        {
            foreach (var prepAction in _prepActions)
                prepAction.PerformAction();

            if (_prepDecisions.Any(d => d.Decide()))
                break;

            yield return 0;
        }

        // Set the aim before shooting.
        Vector3 directionToTarget = _brain.Target.position - this.transform.position;
        Vector3 movementVector = new Vector3(directionToTarget.x, 0f, directionToTarget.z);
        // _orientation.RotationMode = CharacterOrientation3D.RotationModes.None;
        for (int i = 0; i < 2; i++)
        {
            _controller.CurrentDirection = movementVector;
            _movement.SetMovement(movementVector);
            // _orientation.Face(directionToTarget);
            _weaponAim.SetCurrentAim(movementVector);
            yield return true;
        }



        bool finishedUsingSkill = false;
        _weaponInstance.WeaponState.OnStateChange += OnWeaponStateChange;
        _baseWeaponHandler.StartShooting(_weaponInstance);
        while (!finishedUsingSkill)
        {
            _controller.CurrentDirection = movementVector;
            _movement.SetMovement(movementVector);
            // _orientation.Face(directionToTarget);
            _weaponAim.SetCurrentAim(movementVector);
            yield return null;
        }
        _weaponInstance.WeaponState.OnStateChange -= OnWeaponStateChange;

        CurrentCooldown = MaxCooldown;
        UsingSkill = false;
        // _orientation.RotationMode = CharacterOrientation3D.RotationModes.MovementDirection;


        if (OnFinishedUsingSkill != null) OnFinishedUsingSkill.Invoke();

        void OnWeaponStateChange()
        {
            if (_weaponInstance.WeaponState.CurrentState == Weapon.WeaponStates.WeaponStop)
            {
                finishedUsingSkill = true;
            }
        }
    }

}
