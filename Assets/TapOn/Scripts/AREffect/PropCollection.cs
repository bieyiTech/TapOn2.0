using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AREffect
{
    public class PropCollection : MonoBehaviour
    {
        public static PropCollection Instance;
        public List<Templet> Templets = new List<Templet>();

        private void Awake()
        {
            Instance = this;
        }

        [Serializable]
        public class Templet
        {
            public GameObject Object;
            public Sprite Icon;
        }
    }
}