using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NothingAwaits.Core.Actor.Views
{
    public class BaseSacrifice: BaseActor
    {
        [SerializeField] protected TMP_Text _nameText;
        [SerializeField] protected Image[] _bloodObject;
        protected int _hp = 3;
    }
}