using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ordinance;
using Assets.Scripts.Vehicles;
using Assets.Scripts.Vehicles.Components;
using UnityEngine;

public class StoresManagementSystem : ComponentBase
{
    public StoresManagementSystem()
    {

    }

    //Dynamic hardpoints
    private List<Hardpoint> hardpoints;
    private List<Hardpoint> selectedHardpoints;
    public int RippleQuantity;
    public float RippleDelay;


    //Static hardpoints
    private List<StaticHardpoint> staticHardpoints;

    public void ReloadSMS()
    {
        //Get hardpoints
        this.hardpoints = this.ParentVehicle.VehicleComponents.GetComponentsOfType<Hardpoint>().ToList();
        this.staticHardpoints = this.ParentVehicle.VehicleComponents.GetComponentsOfType<StaticHardpoint>().ToList();
    }

    public void SelectSecondaryOrdinanceType(OrdinanceBase type)
    {
        this.selectedHardpoints = this.hardpoints.Where(x => x.Attatchment == type).ToList();
    }

    public void SelectHardpoint(int hardpointNumber)
    {
        Hardpoint hardpoint = hardpoints[hardpointNumber];
        if (hardpoint.Attatchment == selectedHardpoints[0].Attatchment)
        {
            this.selectedHardpoints.Add(hardpoint);
        }
        else
        {
            this.selectedHardpoints.Clear();
            this.selectedHardpoints.Add(hardpoint);
        }
        /*
        //If the new hardpoint has the same ordinance as the already selected ordinance, add it to the selection
        if (hardpoints[hardpointNumber].CurrentOrdinance == selectedHardpoints.ordinance)
        {
            if (this.selectedHardpoints.hardpointNumbers.Contains(hardpointNumber))
            {
                this.selectedHardpoints.hardpointNumbers.Remove(hardpointNumber);
            }
            else
            {
                this.selectedHardpoints.hardpointNumbers.Add(hardpointNumber);
            }
        }
        else //If the new hardpoint dosen't have the same ordinance as the already selected ordinance, clear the selection and add only the new hardpoint
        {
            this.selectedHardpoints.hardpointNumbers.Clear();
            this.selectedHardpoints.hardpointNumbers.Add(hardpointNumber);
            this.selectedHardpoints.ordinance = hardpoints[hardpointNumber].CurrentOrdinance;
        }*/
    }

    public void SelectHardpoint(string hardpointName)
    {
        Hardpoint hardpoint = hardpoints.Where(x => x.HardpointName == hardpointName).FirstOrDefault();
        if (hardpoint.Attatchment == selectedHardpoints[0].Attatchment)
        {
            this.selectedHardpoints.Add(hardpoint);
        }
        else
        {
            this.selectedHardpoints.Clear();
            this.selectedHardpoints.Add(hardpoint);
        }
    }

    public void StartFirePrimaryWeapon()
    {
        
    }

    public void StopFirePrimaryWeapon()
    {
        
    }
    
    public void DeployCounterMeasures()
    {

    }

    public void FireSecondaryWeapon(MonoBehaviour monoBehaviour)
    {
        if (RippleQuantity > 1)
        {
            monoBehaviour.StartCoroutine(RippleWeapon(RippleQuantity, RippleDelay));
        }
        else
        {
            selectedHardpoints[0].Attatchment.Fire();
        }
    }

    #region TEST
    //_ = RippleWeapon(RippleQuantity, RippleDelay);
    //async Task RippleWeapon(int rippleQuantity, float rippleDelay)
    //await Task.Delay((int)(rippleDelay * 1000));
    #endregion

    IEnumerator RippleWeapon(int rippleQuantity, float rippleDelay)
    {
        for (int i = 0; i < rippleQuantity; i++)
        {
            selectedHardpoints[i].Attatchment.Fire();
            if((OrdinanceBase) selectedHardpoints[i].Attatchment == null)
            {
                selectedHardpoints.RemoveAt(i);
            }
            yield return new WaitForSeconds(rippleDelay);
        }
    }
}
