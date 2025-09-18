using System;
using BlockoutRoguelike.Core;
using UnityEngine;

namespace BlockoutRoguelike
{
    public class ProjectController: MonoBehaviour
    {
        private ProjectController _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitStats();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

           
        }

        private void InitStats()
        {
            new Stats().ClearStats();
        }
    }
}