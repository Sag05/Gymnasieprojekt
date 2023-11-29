using System;
using Assets.Scripts.Components;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class Entity : MonoBehaviour
    {
        public Entity()
        {
            this.EntityComponents = new ComponentManager(this);
        }

        public ComponentManager EntityComponents { get; set; }
        public Rigidbody EntityBody { get; private set; } 

        void Start(){
            if (this.gameObject.GetComponent<Rigidbody>() is not null)
            {
                this.EntityBody = this.gameObject.GetComponent<Rigidbody>();
            }
            else
            {
                this.EntityBody = this.gameObject.AddComponent<Rigidbody>();
            }
        }
    }
}