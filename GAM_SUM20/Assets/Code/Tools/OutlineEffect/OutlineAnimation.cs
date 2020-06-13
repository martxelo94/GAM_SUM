using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

namespace cakeslice
{
    [RequireComponent(typeof(OutlineEffect))]
    public class OutlineAnimation : MonoBehaviour
    {
        public static float animationSpeed = 2f;
        bool pingPong = false;
        OutlineEffect effect;

        // Use this for initialization
        void Start()
        {
            effect = GetComponent<OutlineEffect>();
        }

        // Update is called once per frame
        void Update()
        {
            Color c = GetComponent<OutlineEffect>().lineColor0;

            if(pingPong)
            {
                c.a += Time.deltaTime * animationSpeed;

                if(c.a >= 1)
                    pingPong = false;
            }
            else
            {
                c.a -= Time.deltaTime * animationSpeed;

                if(c.a <= 0)
                    pingPong = true;
            }

            c.a = Mathf.Clamp01(c.a);
            effect.lineColor0 = c;
            effect.UpdateMaterialsPublicProperties();
        }
    }
}