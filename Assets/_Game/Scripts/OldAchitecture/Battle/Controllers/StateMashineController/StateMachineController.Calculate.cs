using System;
using UnityEngine;

namespace NothingAwaits.Battle.Controllers
{
    public partial class StateMachineController: MonoBehaviour
    {
        //TODO: более менее рабочее, будет Ability после
        public int FindBestSlotForProtection(int[] allhs, int[] at, int protectionAdd)
        {
            int bestSlot = -1;
            int bestScore = Int32.MaxValue;
            
            for (int i = 0; i < allhs.Length; i++)
            {
                var allHp = allhs[i];
                //если нет моба не лечим
                if (allHp <= 0) continue;
                var attack = at[i];
                //--------------------
                
                //количество исцеленного hp
                var lostHp = allHp + protectionAdd - attack;
                //остаток hp<0 не лечим
                if(lostHp <0) continue;
                if (lostHp == 0) return i;
                var score = lostHp+attack*1000;
                
                //--------------------
                
                if (score<bestScore)
                {
                    bestScore = score;
                    bestSlot = i;
                }
            }

            return bestSlot;
        }

        
        //TODO: более менее рабочее, будет Ability после
        public int FindBestSlotForHP(int[] allhs, int[] curhs, int[] maxhs, int[] ps, int hpAdd)
        {
            int bestSlot = -1;
            int bestScore = -Int32.MaxValue;
            
            for (int i = 0; i < allhs.Length; i++)
            {
                var allHp = allhs[i];
                //если нет моба не лечим
                if (allHp <= 0) continue;
                var attack = ps[i];

                //--------------------
                
                var maxHp = maxhs[i];
                var curHp = curhs[i];
                //нет смысла лечить с полным hp
                if(curHp == maxHp) continue;
                
                //количество исцеленного hp
                var incHp = Math.Min(hpAdd, maxHp-curHp);
                var lostHp = allHp + incHp - attack;
                //остаток hp<0 не лечим
                if(lostHp <0) continue;
                if (lostHp == 0) return i;
                var score = incHp*100+lostHp+attack*1000;
                
                //--------------------
                if (score>bestScore)
                {
                    bestScore = score;
                    bestSlot = i;
                }
            }

            return bestSlot;
        }
    }
}