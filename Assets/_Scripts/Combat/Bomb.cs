﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    private CombatManager combatManager;
    private GridManager gridManager;
    private GameObject model;
    //Settings
    private float detonationTime; //how should we handle this..
    private int range;
    private BombType bombType;

    //Functional
    private float startTime;
    private Vector2Int gridPosition;

    public void SetCombatManager(CombatManager _combatManager) //this is set on object creation
    {
        combatManager = _combatManager;
    }

    public void SetGridManager(GridManager _gridManager) //this is set on object creation.. eh this is getting kind of messy. 
    {
        gridManager = _gridManager;
    }

    public void SetGridPosition(Vector2Int _gridPosition) //called from the PlayerCombat script while dropping a new bomb
    {
        gridPosition = _gridPosition;

        transform.position = gridManager.GridToWorldPosition(gridPosition);
    }

    public void SetType (BombType _bombType) //as above, called from PlayerCombat
    {
        bombType = _bombType;

        RemoveModel();//we shouldn't have an existing model, but if we do clean it up

        model = combatManager.GetModel(bombType);
        model.transform.SetParent(transform);
        model.transform.localPosition = Vector3.zero;
    }

    private void OnEnable() //the PlayerCombat script enables this object as the last step, triggering the detonation cycle to start.
    {
        detonationTime = 3.0f;
        range = 3;
        startTime = Time.time;
    }

    protected void Update ()
    {
        if(Time.time - startTime >= detonationTime)
        {
            Detonate();
        }
    }

    protected void Detonate()
    {
        Debug.Log("Bomb detonating at " + gridPosition.x.ToString() + "," + gridPosition.z.ToString() + " with range" + range);
        combatManager.Detonate(gridPosition, range); //this will only get executed on the server

        RemoveModel(); //this will be called on every client, so the bomb will actually vanish... but the explosion would have latency, rethink this. 
        gameObject.SetActive(false);
    }

    private void RemoveModel()
    {
        if (model) 
        {
            model.transform.SetParent(combatManager.transform);
            model.SetActive(false);
        }
    }
}
