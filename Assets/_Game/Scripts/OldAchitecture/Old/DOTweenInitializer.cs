using UnityEngine;
using DG.Tweening;

namespace NothingAwaits.Core
{
    public class DOTweenInitializer : MonoBehaviour
    {
        private void Start()
        {
            DOTween.Init(recycleAllByDefault: true, useSafeMode: true)
                .SetCapacity(200, 10);
        
            DOTween.defaultEaseType = Ease.OutQuad;
            DOTween.defaultAutoPlay = AutoPlay.All;
        }
    
        private void OnApplicationQuit()
        {
            DOTween.Clear();
        }
    }
}